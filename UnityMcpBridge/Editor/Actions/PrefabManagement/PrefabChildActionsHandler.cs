// Copyright (c) 2025 Logar16. All rights reserved.

using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Utility;

namespace UnityMcp.Editor.Actions.PrefabManagement
{
    [InitializeOnLoad]
    public static class PrefabChildActionsHandler
    {
        static PrefabChildActionsHandler()
        {
            ActionRegistry.RegisterAction<AddPrefabChildRequest>("add_prefab_child", HandleAddPrefabChild);
            ActionRegistry.RegisterAction<RemovePrefabChildRequest>("remove_prefab_child", HandleRemovePrefabChild);
            ActionRegistry.RegisterAction<RenamePrefabChildRequest>("rename_prefab_child", HandleRenamePrefabChild);
            ActionRegistry.RegisterAction<ModifyPrefabChildRequest>("modify_prefab_child", HandleModifyPrefabChild);
        }

        public static AddPrefabChildResponse HandleAddPrefabChild(AddPrefabChildRequest request)
        {
            var response = new AddPrefabChildResponse();

            if (request == null || string.IsNullOrEmpty(request.prefab_path) || request.child_properties == null)
            {
                response.success = false;
                response.message = "Request, prefab_path, or child_properties is null.";
                return response;
            }

            string assetPath = null;
            GameObject prefabRoot = null;
            try
            {
                prefabRoot = PrefabActionUtility.SafeLoadPrefabContents(request.prefab_path, out assetPath);
                if (prefabRoot == null)
                {
                    response.success = false;
                    response.message = $"Prefab asset '{request.prefab_path}' not found or could not be loaded for editing.";
                    return response;
                }

                GameObject parent = prefabRoot;
                if (!string.IsNullOrEmpty(request.parent_path))
                {
                    parent = PrefabActionUtility.FindChildByPath(prefabRoot, request.parent_path);
                    if (parent == null)
                    {
                        response.success = false;
                        response.message = $"Parent GameObject '{request.parent_path}' not found in prefab.";
                        return response;
                    }
                }

                GameObject child = PrefabActionUtility.CreateChildGameObject(parent, request.child_properties);
                response.added_child_path = GameObjectActionUtility.GetHierarchyPath(child.transform);
                response.success = true;
                response.message = $"Child '{child.name}' added to prefab.";

                PrefabActionUtility.SaveAndUnloadPrefabContents(prefabRoot, assetPath);
            }
            catch (System.Exception ex)
            {
                response.success = false;
                response.message = $"Error adding child: {ex.Message}";
                return response;
            }

            return response;
        }

        public static RemovePrefabChildResponse HandleRemovePrefabChild(RemovePrefabChildRequest request)
        {
            var response = new RemovePrefabChildResponse();

            if (request == null || string.IsNullOrEmpty(request.prefab_path) || string.IsNullOrEmpty(request.child_path))
            {
                response.success = false;
                response.message = "Request, prefab_path, or child_path is null.";
                return response;
            }

            string assetPath = null;
            GameObject prefabRoot = null;
            try
            {
                prefabRoot = PrefabActionUtility.SafeLoadPrefabContents(request.prefab_path, out assetPath);
                if (prefabRoot == null)
                {
                    response.success = false;
                    response.message = $"Prefab asset '{request.prefab_path}' not found or could not be loaded for editing.";
                    return response;
                }

                GameObject child = PrefabActionUtility.FindChildByPath(prefabRoot, request.child_path);
                if (child == null)
                {
                    response.success = false;
                    response.message = $"Child GameObject '{request.child_path}' not found in prefab.";
                    return response;
                }

                Object.DestroyImmediate(child, true);
                response.success = true;
                response.message = $"Child '{request.child_path}' removed from prefab.";

                PrefabActionUtility.SaveAndUnloadPrefabContents(prefabRoot, assetPath);
            }
            catch (System.Exception ex)
            {
                response.success = false;
                response.message = $"Error removing child: {ex.Message}";
                return response;
            }

            return response;
        }

        public static RenamePrefabChildResponse HandleRenamePrefabChild(RenamePrefabChildRequest request)
        {
            var response = new RenamePrefabChildResponse();

            if (request == null || string.IsNullOrEmpty(request.prefab_path) ||
                string.IsNullOrEmpty(request.child_path) || string.IsNullOrEmpty(request.new_name))
            {
                response.success = false;
                response.message = "Request, prefab_path, child_path, or new_name is null.";
                return response;
            }

            string assetPath = null;
            GameObject prefabRoot = null;
            try
            {
                prefabRoot = PrefabActionUtility.SafeLoadPrefabContents(request.prefab_path, out assetPath);
                if (prefabRoot == null)
                {
                    response.success = false;
                    response.message = $"Prefab asset '{request.prefab_path}' not found or could not be loaded for editing.";
                    return response;
                }

                GameObject child = PrefabActionUtility.FindChildByPath(prefabRoot, request.child_path);
                if (child == null)
                {
                    response.success = false;
                    response.message = $"Child GameObject '{request.child_path}' not found in prefab.";
                    return response;
                }

                child.name = request.new_name;
                response.success = true;
                response.message = $"Child '{request.child_path}' renamed to '{request.new_name}'.";

                PrefabActionUtility.SaveAndUnloadPrefabContents(prefabRoot, assetPath);
            }
            catch (System.Exception ex)
            {
                response.success = false;
                response.message = $"Error renaming child: {ex.Message}";
                return response;
            }

            return response;
        }

        public static ModifyPrefabChildResponse HandleModifyPrefabChild(ModifyPrefabChildRequest request)
        {
            var response = new ModifyPrefabChildResponse();

            if (request == null || string.IsNullOrEmpty(request.prefab_path) || string.IsNullOrEmpty(request.child_path))
            {
                response.success = false;
                response.message = "Request, prefab_path, or child_path is null.";
                return response;
            }

            string assetPath = null;
            GameObject prefabRoot = null;
            try
            {
                prefabRoot = PrefabActionUtility.SafeLoadPrefabContents(request.prefab_path, out assetPath);
                if (prefabRoot == null)
                {
                    response.success = false;
                    response.message = $"Prefab asset '{request.prefab_path}' not found or could not be loaded for editing.";
                    return response;
                }

                GameObject child = PrefabActionUtility.FindChildByPath(prefabRoot, request.child_path);
                if (child == null)
                {
                    response.success = false;
                    response.message = $"Child GameObject '{request.child_path}' not found in prefab.";
                    return response;
                }

                PrefabActionUtility.SetGameObjectProperties(child, request);
                response.success = true;
                response.message = $"Child '{request.child_path}' modified in prefab.";

                PrefabActionUtility.SaveAndUnloadPrefabContents(prefabRoot, assetPath);
            }
            catch (System.Exception ex)
            {
                response.success = false;
                response.message = $"Error modifying child: {ex.Message}";
                return response;
            }

            return response;
        }
    }
}
