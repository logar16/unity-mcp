using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions.ConsoleLog
{
    [InitializeOnLoad]
    public static class GetConsoleLogsActionHandler
    {
        static GetConsoleLogsActionHandler()
        {
            ActionRegistry.RegisterAction<GetConsoleLogsRequest>("get_console_logs", Handle);
        }

        public static GetConsoleLogsResponse Handle(GetConsoleLogsRequest request)
        {
            var response = new GetConsoleLogsResponse { entries = new List<ConsoleLogEntry>(), success = true };

            // Reflection setup for UnityEditor.LogEntries and LogEntry
            var logEntriesType = Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var logEntryType = Type.GetType("UnityEditor.LogEntry,UnityEditor.dll");
            if (logEntriesType == null || logEntryType == null)
            {
                response.success = false;
                response.message = "Unable to access UnityEditor.LogEntries or LogEntry via reflection.";
                return response;
            }

            var getCountMethod = logEntriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public);
            var getEntryMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var startGettingEntries = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var endGettingEntries = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (getCountMethod == null || getEntryMethod == null)
            {
                response.success = false;
                response.message = "Unable to access log entry methods via reflection.";
                return response;
            }

            int count = (int)getCountMethod.Invoke(null, null);
            if (count == 0)
                return response;

            // Prepare filters
            HashSet<string> typeFilter = null;
            if (request?.types != null && request.types.Count > 0)
                typeFilter = new HashSet<string>(request.types.Select(t => t.ToLowerInvariant()));

            string filterText = request?.filter_text;
            int maxCount = request?.count ?? count;

            // LogEntry fields
            var typeField = logEntryType.GetField("mode", BindingFlags.Instance | BindingFlags.Public);
            var messageField = logEntryType.GetField("message", BindingFlags.Instance | BindingFlags.Public);
            var stacktraceField = logEntryType.GetField("stackTrace", BindingFlags.Instance | BindingFlags.Public);

            // LogEntry type mapping
            Func<object, string> getTypeString = (entry) =>
            {
                int mode = (int)typeField.GetValue(entry);
                // 0 = log, 1 = error, 2 = warning (Unity internal: 1=error, 2=assert, 4=warning, 16=log)
                if ((mode & 1) != 0) return "error";
                if ((mode & 4) != 0) return "warning";
                return "log";
            };

            // Begin reading entries
            startGettingEntries?.Invoke(null, null);

            int added = 0;
            for (int i = count - 1; i >= 0 && added < maxCount; i--)
            {
                var entry = Activator.CreateInstance(logEntryType);
                object[] args = new object[] { i, entry };
                getEntryMethod.Invoke(null, args);

                string typeStr = getTypeString(entry);
                string msg = messageField.GetValue(entry) as string;
                string stack = stacktraceField.GetValue(entry) as string;

                if (typeFilter != null && !typeFilter.Contains(typeStr))
                    continue;
                if (!string.IsNullOrEmpty(filterText) && (msg == null || !msg.IndexOf(filterText, StringComparison.OrdinalIgnoreCase).Equals(-1)))
                    if (msg == null || msg.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                response.entries.Add(new ConsoleLogEntry
                {
                    type = typeStr,
                    message = msg,
                    stacktrace = stack
                });
                added++;
            }

            endGettingEntries?.Invoke(null, null);

            // Reverse to chronological order
            response.entries.Reverse();
            return response;
        }
    }
}
