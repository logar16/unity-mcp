// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class CreateSceneActionHandler
    {
        static CreateSceneActionHandler()
        {
            ActionRegistry.RegisterAction<CreateSceneRequest>(
                "create_scene",
                HandleCreateScene
            );
        }

        public static CreateSceneResponse HandleCreateScene(CreateSceneRequest request)
        {
            var response = new CreateSceneResponse();

            if (request == null || string.IsNullOrWhiteSpace(request.name))
            {
                response.success = false;
                response.message = "Missing required 'name' parameter.";
                return response;
            }

            string relDir = request.path ?? "Assets/Scenes";
            relDir = relDir.Replace('\\', '/').Trim('/');
            if (!relDir.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                relDir = "Assets/" + relDir;

            string sceneFileName = request.name + ".unity";
            string fullPathDir = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length), relDir);
            string relPath = relDir + "/" + sceneFileName;
            string fullPath = Path.Combine(fullPathDir, sceneFileName);

            if (File.Exists(fullPath))
            {
                response.success = false;
                response.message = $"Scene already exists at '{relPath}'.";
                return response;
            }

            try
            {
                Directory.CreateDirectory(fullPathDir);
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                bool saved = EditorSceneManager.SaveScene(newScene, relPath);

                if (saved)
                {
                    AssetDatabase.Refresh();
                    response.success = true;
                    response.message = $"Scene '{sceneFileName}' created successfully at '{relPath}'.";
                    response.scene_path = relPath;
                }
                else
                {
                    response.success = false;
                    response.message = $"Failed to save new scene to '{relPath}'.";
                }
            }
            catch (Exception e)
            {
                response.success = false;
                response.message = $"Error creating scene '{relPath}': {e.Message}";
            }

            return response;
        }
    }
}
