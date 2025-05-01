using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions.ScriptManagement
{
    [InitializeOnLoad]
    public static class DeleteScriptActionHandler
    {
        static DeleteScriptActionHandler()
        {
            ActionRegistry.RegisterAction<DeleteScriptRequest>("delete_script", Handle);
        }

        public static DeleteScriptResponse Handle(DeleteScriptRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.name))
            {
                return new DeleteScriptResponse
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
                return new DeleteScriptResponse
                {
                    success = false,
                    message = $"Script not found at '{relativePath}'.",
                    path = relativePath
                };
            }

            try
            {
                bool deleted = AssetDatabase.MoveAssetToTrash(relativePath);
                AssetDatabase.Refresh();

                if (deleted)
                {
                    return new DeleteScriptResponse
                    {
                        success = true,
                        message = $"Script '{scriptFileName}' moved to trash successfully.",
                        path = relativePath
                    };
                }
                else
                {
                    return new DeleteScriptResponse
                    {
                        success = false,
                        message = $"Failed to move script '{relativePath}' to trash. It might be locked or in use.",
                        path = relativePath
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteScriptResponse
                {
                    success = false,
                    message = $"Error deleting script '{relativePath}': {e.Message}",
                    path = relativePath
                };
            }
        }
    }
}
