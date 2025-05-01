// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class GetActiveSceneActionHandler
    {
        static GetActiveSceneActionHandler()
        {
            ActionRegistry.RegisterAction<GetActiveSceneRequest>(
                "get_active_scene",
                HandleGetActiveScene
            );
        }

        public static GetActiveSceneResponse HandleGetActiveScene(GetActiveSceneRequest request)
        {
            var response = new GetActiveSceneResponse();

            try
            {
                var activeScene = EditorSceneManager.GetActiveScene();
                if (!activeScene.IsValid())
                {
                    response.success = false;
                    response.message = "No active scene found.";
                    return response;
                }

                response.success = true;
                response.message = "Retrieved active scene information.";
                response.name = activeScene.name;
                response.path = activeScene.path;
                response.build_index = activeScene.buildIndex;
                response.is_loaded = activeScene.isLoaded;
                response.is_dirty = activeScene.isDirty;
                response.root_count = activeScene.rootCount;
            }
            catch (Exception e)
            {
                response.success = false;
                response.message = $"Error getting active scene info: {e.Message}";
            }

            return response;
        }
    }
}
