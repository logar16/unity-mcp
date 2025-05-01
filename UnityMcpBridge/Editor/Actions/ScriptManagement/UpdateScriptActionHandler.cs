using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions.ScriptManagement
{
    [InitializeOnLoad]
    public static class UpdateScriptActionHandler
    {
        static UpdateScriptActionHandler()
        {
            ActionRegistry.RegisterAction<UpdateScriptRequest>("update_script", Handle);
        }

        public static UpdateScriptResponse Handle(UpdateScriptRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.name))
            {
                return new UpdateScriptResponse
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
                return new UpdateScriptResponse
                {
                    success = false,
                    message = $"Script not found at '{relativePath}'."
                };
            }

            if (string.IsNullOrEmpty(request.contents))
            {
                return new UpdateScriptResponse
                {
                    success = false,
                    message = "Contents are required for update."
                };
            }

            if (!ValidateScriptSyntax(request.contents))
            {
                Debug.LogWarning($"Potential syntax error in script being updated: {request.name}");
            }

            try
            {
                File.WriteAllText(fullPath, request.contents);
                AssetDatabase.ImportAsset(relativePath);
                AssetDatabase.Refresh();

                return new UpdateScriptResponse
                {
                    success = true,
                    message = $"Script '{scriptFileName}' updated successfully at '{relativePath}'.",
                    path = relativePath
                };
            }
            catch (Exception e)
            {
                return new UpdateScriptResponse
                {
                    success = false,
                    message = $"Failed to update script '{relativePath}': {e.Message}"
                };
            }
        }

        private static bool ValidateScriptSyntax(string contents)
        {
            if (string.IsNullOrEmpty(contents))
                return true;
            int braceBalance = 0;
            foreach (char c in contents)
            {
                if (c == '{') braceBalance++;
                else if (c == '}') braceBalance--;
            }
            return braceBalance == 0;
        }
    }
}
