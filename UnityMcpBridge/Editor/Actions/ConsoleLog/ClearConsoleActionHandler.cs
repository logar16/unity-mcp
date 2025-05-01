using System;
using System.Reflection;
using UnityEditor;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions.ConsoleLog
{
    [InitializeOnLoad]
    public static class ClearConsoleActionHandler
    {
        static ClearConsoleActionHandler()
        {
            ActionRegistry.RegisterAction<ClearConsoleRequest>("clear_console", Handle);
        }

        public static ClearConsoleResponse Handle(ClearConsoleRequest request)
        {
            var response = new ClearConsoleResponse { success = true };

            // Use reflection to call UnityEditor.LogEntries.Clear()
            var logEntriesType = Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            if (logEntriesType == null)
            {
                response.success = false;
                response.message = "Unable to access UnityEditor.LogEntries via reflection.";
                return response;
            }

            var clearMethod = logEntriesType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (clearMethod == null)
            {
                response.success = false;
                response.message = "Unable to find LogEntries.Clear method via reflection.";
                return response;
            }

            try
            {
                clearMethod.Invoke(null, null);
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Exception clearing console: " + ex.Message;
            }

            return response;
        }
    }
}
