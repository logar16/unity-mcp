// Copyright (c) 2025 Logar16. All rights reserved.

using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class EditorStateActionHandler
    {
        static EditorStateActionHandler()
        {
            ActionRegistry.RegisterAction<PlayRequest>("play", HandlePlay);
            ActionRegistry.RegisterAction<PauseRequest>("pause", HandlePause);
            ActionRegistry.RegisterAction<StopRequest>("stop", HandleStop);
            ActionRegistry.RegisterAction<GetStateRequest>("get_state", HandleGetState);
        }

        public static PlayResponse HandlePlay(PlayRequest request)
        {
            var response = new PlayResponse();
            try
            {
                if (!EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = true;
                    response.success = true;
                    response.message = "Entered play mode.";
                }
                else
                {
                    response.success = true;
                    response.message = "Already in play mode.";
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EditorStateActionHandler] Exception (Play): {ex}");
                response.success = false;
                response.message = $"Error entering play mode: {ex.Message}";
            }
            return response;
        }

        public static PauseResponse HandlePause(PauseRequest request)
        {
            var response = new PauseResponse();
            try
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPaused = !EditorApplication.isPaused;
                    response.success = true;
                    response.isPaused = EditorApplication.isPaused;
                    response.message = EditorApplication.isPaused ? "Game paused." : "Game resumed.";
                }
                else
                {
                    response.success = false;
                    response.isPaused = false;
                    response.message = "Cannot pause/resume: Not in play mode.";
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EditorStateActionHandler] Exception (Pause): {ex}");
                response.success = false;
                response.isPaused = EditorApplication.isPaused;
                response.message = $"Error pausing/resuming game: {ex.Message}";
            }
            return response;
        }

        public static StopResponse HandleStop(StopRequest request)
        {
            var response = new StopResponse();
            try
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                    response.success = true;
                    response.message = "Exited play mode.";
                }
                else
                {
                    response.success = true;
                    response.message = "Already stopped (not in play mode).";
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EditorStateActionHandler] Exception (Stop): {ex}");
                response.success = false;
                response.message = $"Error stopping play mode: {ex.Message}";
            }
            return response;
        }

        public static GetStateResponse HandleGetState(GetStateRequest request)
        {
            var response = new GetStateResponse();
            try
            {
                response.isPlaying = EditorApplication.isPlaying;
                response.isPaused = EditorApplication.isPaused;
                response.isCompiling = EditorApplication.isCompiling;
                response.isUpdating = EditorApplication.isUpdating;
                response.applicationPath = EditorApplication.applicationPath;
                response.applicationContentsPath = EditorApplication.applicationContentsPath;
                response.timeSinceStartup = EditorApplication.timeSinceStartup;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EditorStateActionHandler] Exception (GetState): {ex}");
                // Set defaults on error
                response.isPlaying = false;
                response.isPaused = false;
                response.isCompiling = false;
                response.isUpdating = false;
                response.applicationPath = "";
                response.applicationContentsPath = "";
                response.timeSinceStartup = 0;
            }
            return response;
        }
    }
}
