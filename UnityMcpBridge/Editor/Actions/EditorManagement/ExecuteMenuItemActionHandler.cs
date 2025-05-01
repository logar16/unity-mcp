// Copyright (c) 2025 Logar16. All rights reserved.

using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class ExecuteMenuItemActionHandler
    {
        static ExecuteMenuItemActionHandler()
        {
            ActionRegistry.RegisterAction<ExecuteMenuItemRequest>(
                "execute_menu_item",
                HandleExecuteMenuItem
            );
        }

        public static BaseActionResponse HandleExecuteMenuItem(ExecuteMenuItemRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.menu_path))
            {
                return new ExecuteMenuItemResponse
                {
                    success = false,
                    message = "Missing or empty 'menu_path'."
                };
            }

            try
            {
                bool result = EditorApplication.ExecuteMenuItem(request.menu_path);
                if (result)
                {
                    return new ExecuteMenuItemResponse
                    {
                        success = true,
                        message = $"Menu item '{request.menu_path}' executed successfully."
                    };
                }
                else
                {
                    return new ExecuteMenuItemResponse
                    {
                        success = false,
                        message = $"Menu item '{request.menu_path}' not found or could not be executed."
                    };
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ExecuteMenuItemActionHandler] Exception: {ex}");
                return new ExecuteMenuItemResponse
                {
                    success = false,
                    message = $"Exception occurred: {ex.Message}"
                };
            }
        }
    }
}
