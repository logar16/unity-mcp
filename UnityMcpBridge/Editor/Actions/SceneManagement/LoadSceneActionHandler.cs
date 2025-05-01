// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class LoadSceneActionHandler
    {
        static LoadSceneActionHandler()
        {
            ActionRegistry.RegisterAction<LoadSceneRequest>(
                "load_scene",
                HandleLoadScene
            );
        }

        public static LoadSceneResponse HandleLoadScene(LoadSceneRequest request)
        {
            var response = new LoadSceneResponse();

            if (request == null)
            {
                response.success = false;
                response.message = "Request is null.";
                return response;
            }

            string relPath = null;
            if (!string.IsNullOrWhiteSpace(request.path))
            {
                relPath = request.path.Replace('\\', '/').Trim('/');
                if (!relPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                    relPath = "Assets/" + relPath;
            }
            else if (!string.IsNullOrWhiteSpace(request.name))
            {
                relPath = $"Assets/Scenes/{request.name}.unity";
            }

            // Try by path/name first
            if (!string.IsNullOrEmpty(relPath))
            {
                string absPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length), relPath);
                if (!File.Exists(absPath))
                {
                    response.success = false;
                    response.message = $"Scene file not found at '{relPath}'.";
                    return response;
                }

                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    response.success = false;
                    response.message = "Current scene has unsaved changes. Please save or discard changes before loading a new scene.";
                    return response;
                }

                try
                {
                    var scene = EditorSceneManager.OpenScene(relPath, OpenSceneMode.Single);
                    response.success = true;
                    response.message = $"Scene '{relPath}' loaded successfully.";
                    response.scene_path = relPath;
                    response.scene_name = Path.GetFileNameWithoutExtension(relPath);
                    response.build_index = scene.buildIndex;
                    return response;
                }
                catch (Exception e)
                {
                    response.success = false;
                    response.message = $"Error loading scene '{relPath}': {e.Message}";
                    return response;
                }
            }
            // Try by build index
            else if (request.build_index.HasValue)
            {
                int idx = request.build_index.Value;
                if (idx < 0 || idx >= SceneManager.sceneCountInBuildSettings)
                {
                    response.success = false;
                    response.message = $"Invalid build index: {idx}.";
                    return response;
                }

                if (EditorSceneManager.GetActiveScene().isDirty)
                {
                    response.success = false;
                    response.message = "Current scene has unsaved changes. Please save or discard changes before loading a new scene.";
                    return response;
                }

                try
                {
                    string scenePath = SceneUtility.GetScenePathByBuildIndex(idx);
                    var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    response.success = true;
                    response.message = $"Scene at build index {idx} ('{scenePath}') loaded successfully.";
                    response.scene_path = scenePath;
                    response.scene_name = Path.GetFileNameWithoutExtension(scenePath);
                    response.build_index = idx;
                    return response;
                }
                catch (Exception e)
                {
                    response.success = false;
                    response.message = $"Error loading scene with build index {idx}: {e.Message}";
                    return response;
                }
            }
            else
            {
                response.success = false;
                response.message = "Either 'name'/'path' or 'build_index' must be provided.";
                return response;
            }
        }
    }
}
