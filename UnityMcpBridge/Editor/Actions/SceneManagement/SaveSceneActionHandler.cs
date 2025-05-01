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
    public static class SaveSceneActionHandler
    {
        static SaveSceneActionHandler()
        {
            ActionRegistry.RegisterAction<SaveSceneRequest>(
                "save_scene",
                HandleSaveScene
            );
        }

        public static SaveSceneResponse HandleSaveScene(SaveSceneRequest request)
        {
            var response = new SaveSceneResponse();

            try
            {
                var currentScene = EditorSceneManager.GetActiveScene();
                if (!currentScene.IsValid())
                {
                    response.success = false;
                    response.message = "No valid scene is currently active to save.";
                    return response;
                }

                string relPath = null;
                string fullPath = null;

                if (!string.IsNullOrWhiteSpace(request?.path) && !string.IsNullOrWhiteSpace(request?.name))
                {
                    relPath = request.path.Replace('\\', '/').Trim('/');
                    if (!relPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                        relPath = "Assets/" + relPath;
                    relPath = relPath + "/" + request.name + ".unity";
                    fullPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length), relPath);

                    string dir = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    bool saved = EditorSceneManager.SaveScene(currentScene, relPath);
                    if (saved)
                    {
                        AssetDatabase.Refresh();
                        response.success = true;
                        response.message = $"Scene '{request.name}' saved successfully to '{relPath}'.";
                        response.scene_path = relPath;
                        response.scene_name = request.name;
                    }
                    else
                    {
                        response.success = false;
                        response.message = $"Failed to save scene '{request.name}'.";
                    }
                }
                else
                {
                    // Save (overwrite existing or save untitled)
                    if (string.IsNullOrEmpty(currentScene.path))
                    {
                        response.success = false;
                        response.message = "Cannot save an untitled scene without providing a 'name' and 'path'. Use Save As functionality.";
                        return response;
                    }
                    bool saved = EditorSceneManager.SaveScene(currentScene);
                    if (saved)
                    {
                        AssetDatabase.Refresh();
                        response.success = true;
                        response.message = $"Scene '{currentScene.name}' saved successfully to '{currentScene.path}'.";
                        response.scene_path = currentScene.path;
                        response.scene_name = currentScene.name;
                    }
                    else
                    {
                        response.success = false;
                        response.message = $"Failed to save scene '{currentScene.name}'.";
                    }
                }
            }
            catch (Exception e)
            {
                response.success = false;
                response.message = $"Error saving scene: {e.Message}";
            }

            return response;
        }
    }
}
