// Copyright (c) 2025 Logar16. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class FindGameObjectActionHandler
    {
        static FindGameObjectActionHandler()
        {
            ActionRegistry.RegisterAction<FindGameObjectRequest>(
                "find_gameobject",
                HandleFindGameObject
            );
        }

        public static FindGameObjectResponse HandleFindGameObject(FindGameObjectRequest request)
        {
            var response = new FindGameObjectResponse
            {
                found_objects = new List<GameObjectIdentifier>()
            };

            if (request == null)
            {
                response.success = false;
                response.message = "Request is null.";
                return response;
            }

            IEnumerable<GameObject> candidates = null;

            // Search by tag
            if (!string.IsNullOrEmpty(request.tag))
            {
                candidates = GameObject.FindGameObjectsWithTag(request.tag);
            }
            // Search by name
            else if (!string.IsNullOrEmpty(request.name))
            {
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                candidates = all.Where(go => go.name == request.name);
            }
            // Search by path
            else if (!string.IsNullOrEmpty(request.path))
            {
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                candidates = all.Where(go => GetHierarchyPath(go.transform) == request.path);
            }
            else
            {
                response.success = false;
                response.message = "No search criteria provided (name, tag, or path required).";
                return response;
            }

            // Filter inactive if needed
            if (!request.search_inactive)
                candidates = candidates.Where(go => go.hideFlags == HideFlags.None && go.activeInHierarchy);

            var found = request.find_all ? candidates.ToList() : candidates.Take(1).ToList();

            foreach (var go in found)
            {
                response.found_objects.Add(new GameObjectIdentifier
                {
                    instance_id = go.GetInstanceID(),
                    name = go.name,
                    path = GetHierarchyPath(go.transform),
                    tag = go.tag,
                    layer = go.layer,
                    active_self = go.activeSelf,
                    transform = new TransformData
                    {
                        position = Vector3Data.FromVector3(go.transform.localPosition),
                        rotation = Vector3Data.FromVector3(go.transform.localEulerAngles),
                        world_position = Vector3Data.FromVector3(go.transform.position),
                        world_rotation = Vector3Data.FromVector3(go.transform.eulerAngles),
                        scale = Vector3Data.FromVector3(go.transform.localScale)
                    }
                });
            }

            response.success = true;
            response.message = $"Found {response.found_objects.Count} GameObject(s).";
            return response;
        }

        private static string GetHierarchyPath(Transform transform)
        {
            var path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
    }
}
