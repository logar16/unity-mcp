// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class GetSceneHierarchyActionHandler
    {
        static GetSceneHierarchyActionHandler()
        {
            ActionRegistry.RegisterAction<GetSceneHierarchyRequest>(
                "get_scene_hierarchy",
                HandleGetSceneHierarchy
            );
        }

        public static GetSceneHierarchyResponse HandleGetSceneHierarchy(GetSceneHierarchyRequest request)
        {
            var response = new GetSceneHierarchyResponse();

            try
            {
                var activeScene = EditorSceneManager.GetActiveScene();
                if (!activeScene.IsValid() || !activeScene.isLoaded)
                {
                    response.success = false;
                    response.message = "No valid and loaded scene is active to get hierarchy from.";
                    return response;
                }

                GameObject[] rootObjects = activeScene.GetRootGameObjects();
                var hierarchy = new List<SceneHierarchyNode>();
                foreach (var go in rootObjects)
                {
                    hierarchy.Add(BuildNode(go));
                }

                response.success = true;
                response.message = $"Retrieved hierarchy for scene '{activeScene.name}'.";
                response.hierarchy = hierarchy;
            }
            catch (Exception e)
            {
                response.success = false;
                response.message = $"Error getting scene hierarchy: {e.Message}";
            }

            return response;
        }

        private static SceneHierarchyNode BuildNode(GameObject go)
        {
            var node = new SceneHierarchyNode
            {
                name = go.name,
                active_self = go.activeSelf,
                active_in_hierarchy = go.activeInHierarchy,
                tag = go.tag,
                layer = go.layer,
                is_static = go.isStatic,
                instance_id = go.GetInstanceID(),
                transform = new TransformData
                {
                    position = new Vector3Data
                    {
                        x = go.transform.localPosition.x,
                        y = go.transform.localPosition.y,
                        z = go.transform.localPosition.z
                    },
                    rotation = new Vector3Data
                    {
                        x = go.transform.localRotation.eulerAngles.x,
                        y = go.transform.localRotation.eulerAngles.y,
                        z = go.transform.localRotation.eulerAngles.z
                    },
                    scale = new Vector3Data
                    {
                        x = go.transform.localScale.x,
                        y = go.transform.localScale.y,
                        z = go.transform.localScale.z
                    }
                },
                children = new List<SceneHierarchyNode>()
            };

            foreach (Transform child in go.transform)
            {
                node.children.Add(BuildNode(child.gameObject));
            }

            return node;
        }
    }
}
