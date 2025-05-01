using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions.ScriptManagement
{
    [InitializeOnLoad]
    public static class ReadScriptActionHandler
    {
        static ReadScriptActionHandler()
        {
            ActionRegistry.RegisterAction<ReadScriptRequest>("read_script", Handle);
        }

        public static ReadScriptResponse Handle(ReadScriptRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.name))
            {
                return new ReadScriptResponse
                {
                    success = false,
                    message = "Script name is required."
                };
            }

            string relativeDir = string.IsNullOrEmpty(request.path) ? "Scripts" : request.path.Replace('\\', '/').Trim('/');
            if (relativeDir.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                relativeDir = relativeDir.Substring("Assets/".Length).TrimStart('/');
            if (string.IsNullOrEmpty(relativeDir))
                relativeDir = "Scripts";

            string scriptFileName = $"{request.name}.cs";
            string fullPathDir = Path.Combine(Application.dataPath, relativeDir);
            string fullPath = Path.Combine(fullPathDir, scriptFileName);
            string relativePath = Path.Combine("Assets", relativeDir, scriptFileName).Replace('\\', '/');

            if (!File.Exists(fullPath))
            {
                return new ReadScriptResponse
                {
                    success = false,
                    message = $"Script not found at '{relativePath}'.",
                    path = relativePath
                };
            }

            try
            {
                string contents = File.ReadAllText(fullPath);
                return new ReadScriptResponse
                {
                    success = true,
                    message = $"Script '{scriptFileName}' read successfully.",
                    contents = contents,
                    path = relativePath
                };
            }
            catch (Exception e)
            {
                return new ReadScriptResponse
                {
                    success = false,
                    message = $"Failed to read script '{relativePath}': {e.Message}",
                    path = relativePath
                };
            }
        }
    }
}
