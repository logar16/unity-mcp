// Copyright (c) 2025 Logar16. All rights reserved.

using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Utility;

namespace UnityMcp.Editor.Actions.PrefabManagement
{
    [InitializeOnLoad]
    public static class PrefabComponentActionsHandler
    {
        static PrefabComponentActionsHandler()
        {
            ActionRegistry.RegisterAction<AddPrefabComponentRequest>("add_prefab_component", HandleAddPrefabComponent);
            ActionRegistry.RegisterAction<RemovePrefabComponentRequest>("remove_prefab_component", HandleRemovePrefabComponent);
            ActionRegistry.RegisterAction<ModifyPrefabComponentRequest>("modify_prefab_component", HandleModifyPrefabComponent);
        }

        public static AddPrefabComponentResponse HandleAddPrefabComponent(AddPrefabComponentRequest request)
        {
            var response = new AddPrefabComponentResponse();

            if (request == null || string.IsNullOrEmpty(request.prefab_path) || string.IsNullOrEmpty(request.child_path) || string.IsNullOrEmpty(request.component_type))
            {
                response.success = false;
                response.message = "Request, prefab_path, child_path, or component_type is null or empty.";
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

                Component comp = GameObjectActionUtility.AddComponentByType(child, request.component_type, request.component_properties);
                if (comp == null)
                {
                    response.success = false;
                    response.message = $"Component type '{request.component_type}' could not be added.";
                    return response;
                }

                response.success = true;

                PrefabActionUtility.SaveAndUnloadPrefabContents(prefabRoot, assetPath);
            }
            catch (System.Exception ex)
            {
                response.success = false;
                response.message = $"Error adding component: {ex.Message}";
                return response;
            }

            return response;
        }

        public static RemovePrefabComponentResponse HandleRemovePrefabComponent(RemovePrefabComponentRequest request)
        {
            var response = new RemovePrefabComponentResponse();

            if (request == null || string.IsNullOrEmpty(request.prefab_path) || string.IsNullOrEmpty(request.child_path) || string.IsNullOrEmpty(request.component_type))
            {
                response.success = false;
                response.message = "Request, prefab_path, child_path, or component_type is null or empty.";
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

                bool removed = GameObjectActionUtility.RemoveComponentByType(child, request.component_type);
                if (!removed)
                {
                    response.success = false;
                    response.message = $"Component type '{request.component_type}' not found or could not be removed.";
                    return response;
                }

                response.success = true;

                PrefabActionUtility.SaveAndUnloadPrefabContents(prefabRoot, assetPath);
            }
            catch (System.Exception ex)
            {
                response.success = false;
                response.message = $"Error removing component: {ex.Message}";
                return response;
            }

            return response;
        }

        public static ModifyPrefabComponentResponse HandleModifyPrefabComponent(ModifyPrefabComponentRequest request)
        {
            var response = new ModifyPrefabComponentResponse();

            if (request == null || string.IsNullOrEmpty(request.prefab_path) || string.IsNullOrEmpty(request.child_path) || string.IsNullOrEmpty(request.component_type))
            {
                response.success = false;
                response.message = "Request, prefab_path, child_path, or component_type is null or empty.";
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

                bool modified = GameObjectActionUtility.ModifyComponentProperties(child, request.component_type, request.component_properties);
                if (!modified)
                {
                    response.success = false;
                    response.message = $"Component type '{request.component_type}' not found or properties could not be set.";
                    return response;
                }

                response.success = true;

                PrefabActionUtility.SaveAndUnloadPrefabContents(prefabRoot, assetPath);
            }
            catch (System.Exception ex)
            {
                response.success = false;
                response.message = $"Error modifying component: {ex.Message}";
                return response;
            }

            return response;
        }
    }
}
