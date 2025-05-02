using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;

namespace UnityMcp.Editor
{
    [InitializeOnLoad]
    public static class UnityMcpBridge
    {
        private static TcpListener _listener;
        private static Thread _listenerThread;
        private static bool _running;
        private const int Port = 6400;

        // Command queue for main-thread processing
        private class QueuedRequest
        {
            public JObject Request;
            public Action<string> SetResult;
            public Action<Exception> SetError;
        }
        private static readonly Queue<QueuedRequest> _commandQueue = new Queue<QueuedRequest>();
        private static readonly object _queueLock = new object();

        public static bool IsRunning => _running;

        static UnityMcpBridge()
        {
            EditorApplication.update += EnsureStarted;
        }

        private static void EnsureStarted()
        {
            EditorApplication.update -= EnsureStarted;
            Start();
        }

        public static void Start()
        {
            if (_running) return;
            _listener = new TcpListener(IPAddress.Loopback, Port);
            _listener.Start();
            _running = true;
            _listenerThread = new Thread(ListenLoop) { IsBackground = true };
            _listenerThread.Start();
            EditorApplication.update += ProcessCommands;
            Debug.Log($"[UnityMcp] Listening on port {Port}");
        }

        public static void Stop()
        {
            _running = false;
            try { _listener?.Stop(); } catch { }
            _listenerThread = null;
            EditorApplication.update -= ProcessCommands;
        }

        private static void ListenLoop()
        {
            while (_running)
            {
                try
                {
                    if (!_listener.Pending()) { Thread.Sleep(10); continue; }
                    var client = _listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[UnityMcpV2] Listener error: {ex}");
                }
            }
        }

        private static void HandleClient(object state)
        {
            using var client = (TcpClient)state;
            using var stream = client.GetStream();
            byte[] buffer = new byte[8192];
            try
            {
                while (_running && client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Client disconnected

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    // Special handling for ping (not as an action)
                    if (message == "ping")
                    {
                        var pong = "{\"status\":\"success\",\"result\":{\"message\":\"pong\"}}";
                        byte[] pongBytes = Encoding.UTF8.GetBytes(pong);
                        stream.Write(pongBytes, 0, pongBytes.Length);
                        continue;
                    }

                    Debug.Log($"[UnityMcp] Received request: {message}");

                    // Try to parse as JSON
                    JObject request = null;
                    try
                    {
                        request = JObject.Parse(message);
                    }
                    catch
                    {
                        var error = "{\"success\":false,\"message\":\"Invalid JSON format.\"}";
                        byte[] errorBytes = Encoding.UTF8.GetBytes(error);
                        stream.Write(errorBytes, 0, errorBytes.Length);
                        continue;
                    }

                    // Enqueue for main-thread processing and wait for result
                    string respJson = null;
                    Exception errorEx = null;
                    using (var waitHandle = new ManualResetEventSlim(false))
                    {
                        var queued = new QueuedRequest
                        {
                            Request = request,
                            SetResult = result =>
                            {
                                respJson = result;
                                waitHandle.Set();
                            },
                            SetError = ex =>
                            {
                                errorEx = ex;
                                waitHandle.Set();
                            }
                        };
                        lock (_queueLock)
                        {
                            _commandQueue.Enqueue(queued);
                        }
                        waitHandle.Wait();
                    }

                    if (errorEx != null)
                    {
                        var error = "{\"success\":false,\"message\":\"Internal error: " + errorEx.Message.Replace("\"", "\\\"") + "\"}";
                        byte[] errorBytes = Encoding.UTF8.GetBytes(error);
                        stream.Write(errorBytes, 0, errorBytes.Length);
                    }
                    else
                    {
                        Debug.Log($"[UnityMcp] Sending response: {respJson}");
                        byte[] respBytes = Encoding.UTF8.GetBytes(respJson);
                        stream.Write(respBytes, 0, respBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UnityMcp] Client handler error: {ex}");
            }
        }
        private static void ProcessCommands()
        {
            while (true)
            {
                QueuedRequest queued = null;
                lock (_queueLock)
                {
                    if (_commandQueue.Count > 0)
                        queued = _commandQueue.Dequeue();
                }
                if (queued == null)
                    break;

                try
                {
                    string result = ActionRegistry.RunAction(queued.Request);
                    queued.SetResult(result);
                }
                catch (Exception ex)
                {
                    queued.SetError(ex);
                }
            }
        }
    }
}
