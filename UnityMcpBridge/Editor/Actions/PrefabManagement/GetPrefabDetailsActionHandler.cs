// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Utility;

namespace UnityMcp.Editor.Actions.PrefabManagement
{
    /// <summary>
    /// Handler for the "get_prefab_details" MCP action, providing inspection of prefab assets at various detail levels.
    /// </summary>
    [InitializeOnLoad]
    public static class GetPrefabDetailsActionHandler
    {
        static GetPrefabDetailsActionHandler()
        {
            ActionRegistry.RegisterAction<GetPrefabDetailsRequest>(
                "get_prefab_details",
                HandleGetPrefabDetails
            );
        }

        /// <summary>
        /// Handles the "get_prefab_details" action, returning prefab information at the requested detail level.
        /// </summary>
        /// <param name="request">The request containing the prefab path and detail level.</param>
        /// <returns>A response with the requested prefab details.</returns>
        public static GetPrefabDetailsResponse HandleGetPrefabDetails(GetPrefabDetailsRequest request)
        {
            GameObject prefabRoot = null;
            string assetPath = $"Assets/{request.prefab_path}".Replace("\\", "/");
            try
            {
                prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
                if (prefabRoot == null)
                {
                    return new GetPrefabDetailsResponse().Fail(request, $"Could not load prefab at path: {assetPath}") as GetPrefabDetailsResponse;
                }

                // Find the starting GameObject based on child_path
                GameObject targetObject = prefabRoot;
                if (!string.IsNullOrEmpty(request.child_path))
                {
                    targetObject = PrefabActionUtility.FindChildByPath(prefabRoot, request.child_path);
                    if (targetObject == null)
                    {
                        return new GetPrefabDetailsResponse().Fail(request, $"Could not find child at path: {request.child_path}") as GetPrefabDetailsResponse;
                    }
                }

                var response = new GetPrefabDetailsResponse
                {
                    prefab_path = request.prefab_path,
                    hierarchy = PrefabActionUtility.BuildHierarchy(
                        targetObject,
                        "", // Empty string for root level paths, letting BuildHierarchy construct the full path
                        request.include_children,
                        request.include_component_details
                    )
                };

                return response.Success(request) as GetPrefabDetailsResponse;
            }
            catch (Exception ex)
            {
                return new GetPrefabDetailsResponse().Fail(request, $"Exception while loading prefab: {ex.Message}") as GetPrefabDetailsResponse;
            }
            finally
            {
                if (prefabRoot != null)
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }
    }
}
