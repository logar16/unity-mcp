// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class DeleteGameObjectActionHandler
    {
        static DeleteGameObjectActionHandler()
        {
            ActionRegistry.RegisterAction<DeleteGameObjectRequest>(
                "delete_gameobject",
                HandleDeleteGameObject
            );
        }

        public static DeleteGameObjectResponse HandleDeleteGameObject(DeleteGameObjectRequest request)
        {
            var response = new DeleteGameObjectResponse();

            if (request == null || string.IsNullOrWhiteSpace(request.target))
            {
                response.success = false;
                response.message = "Missing required 'target' parameter.";
                return response;
            }

            GameObject go = FindTarget(request.target);
            if (go == null)
            {
                response.success = false;
                response.message = $"GameObject '{request.target}' not found.";
                return response;
            }

            try
            {
                Undo.DestroyObjectImmediate(go);
                response.success = true;
                response.message = $"GameObject '{request.target}' deleted successfully.";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = $"Error deleting GameObject: {ex.Message}";
            }

            return response;
        }

        private static GameObject FindTarget(string target)
        {
            var go = GameObject.Find(target);
            if (go != null) return go;

            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            go = all.FirstOrDefault(g => GetHierarchyPath(g.transform) == target);
            if (go != null) return go;

            if (int.TryParse(target, out int id))
                return all.FirstOrDefault(g => g.GetInstanceID() == id);

            return null;
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
