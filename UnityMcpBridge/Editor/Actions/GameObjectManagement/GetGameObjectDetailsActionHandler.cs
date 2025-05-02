// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Utility;

namespace UnityMcp.Editor.Actions.GameObjectManagement
{
    /// <summary>
    /// Handler for the "get_gameobject_details" MCP action, providing inspection of scene GameObjects.
    /// </summary>
    [InitializeOnLoad]
    public static class GetGameObjectDetailsActionHandler
    {
        static GetGameObjectDetailsActionHandler()
        {
            ActionRegistry.RegisterAction<GetGameObjectDetailsRequest>(
                "get_gameobject_details",
                HandleGetGameObjectDetails
            );
        }

        /// <summary>
        /// Handles the "get_gameobject_details" action, returning GameObject information and its hierarchy.
        /// </summary>
        /// <param name="request">The request containing the target GameObject identifier and detail flags.</param>
        /// <returns>A response with the requested GameObject details.</returns>
        public static GetGameObjectDetailsResponse HandleGetGameObjectDetails(GetGameObjectDetailsRequest request)
        {
            try
            {
                GameObject targetObject = GameObjectActionUtility.FindGameObject(request.target);
                if (targetObject == null)
                {
                    return new GetGameObjectDetailsResponse().Fail(request, $"Could not find GameObject target: {request.target}") as GetGameObjectDetailsResponse;
                }

                GameObjectHierarchyNode hierarchy = GameObjectActionUtility.BuildGameObjectHierarchy(
                    targetObject,
                    "",
                    request.include_children,
                    request.include_component_details
                );

                var response = new GetGameObjectDetailsResponse { hierarchy = hierarchy };
                return response.Success(request) as GetGameObjectDetailsResponse;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new GetGameObjectDetailsResponse().Fail(request, $"Error getting GameObject details: {e.Message}") as GetGameObjectDetailsResponse;
            }
        }
    }
}
