// Copyright (c) 2025 Logar16. All rights reserved.

using UnityEditor;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Utility;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class GetGameObjectInfoActionHandler
    {
        static GetGameObjectInfoActionHandler()
        {
            ActionRegistry.RegisterAction<GetGameObjectInfoRequest>(
                "get_gameobject_info",
                HandleGetGameObjectInfo
            );
        }

        public static GetGameObjectInfoResponse HandleGetGameObjectInfo(GetGameObjectInfoRequest request)
        {
            var response = new GetGameObjectInfoResponse();

            if (request == null || string.IsNullOrEmpty(request.target))
            {
                response.success = false;
                response.message = "Request or target is null.";
                return response;
            }

            var go = GameObjectActionUtility.FindGameObject(request.target);
            if (go == null)
            {
                response.success = false;
                response.message = $"GameObject '{request.target}' not found.";
                return response;
            }

            string level = (request.detail_level ?? "summary").ToLowerInvariant();
            switch (level)
            {
                case "summary":
                    response.summary = GameObjectActionUtility.GetSummary(go);
                    response.success = true;
                    response.message = "Summary info retrieved.";
                    break;
                case "detailed":
                    response.detailed = GameObjectActionUtility.GetDetailed(go);
                    response.success = true;
                    response.message = "Detailed info retrieved.";
                    break;
                case "component_details":
                    if (string.IsNullOrEmpty(request.component_type))
                    {
                        response.success = false;
                        response.message = "component_type must be specified for component_details.";
                        break;
                    }
                    var compDetails = GameObjectActionUtility.GetComponentDetails(go, request.component_type);
                    if (compDetails == null)
                    {
                        response.success = false;
                        response.message = $"Component '{request.component_type}' not found on GameObject.";
                        break;
                    }
                    response.component_details = compDetails;
                    response.success = true;
                    response.message = "Component details retrieved.";
                    break;
                default:
                    response.success = false;
                    response.message = $"Unknown detail_level '{request.detail_level}'.";
                    break;
            }

            return response;
        }
    }
}
