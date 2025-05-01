// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class ModifyGameObjectActionHandler
    {
        static ModifyGameObjectActionHandler()
        {
            ActionRegistry.RegisterAction<ModifyGameObjectRequest>(
                "modify_gameobject",
                HandleModifyGameObject
            );
        }

        public static ModifyGameObjectResponse HandleModifyGameObject(ModifyGameObjectRequest request)
        {
            var response = new ModifyGameObjectResponse();

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
                Undo.RecordObject(go, "Modify GameObject");
                Undo.RecordObject(go.transform, "Modify GameObject Transform");

                // Name
                if (!string.IsNullOrEmpty(request.name))
                    go.name = request.name;

                // Tag
                if (!string.IsNullOrEmpty(request.tag))
                {
                    try { go.tag = request.tag; }
                    catch
                    {
                        InternalEditorUtility.AddTag(request.tag);
                        go.tag = request.tag;
                    }
                }

                // Layer
                if (!string.IsNullOrEmpty(request.layer))
                {
                    int layerId = LayerMask.NameToLayer(request.layer);
                    if (layerId != -1)
                        go.layer = layerId;
                }

                // Parent
                if (!string.IsNullOrEmpty(request.parent))
                {
                    var parentGo = GameObject.Find(request.parent);
                    if (parentGo != null)
                        go.transform.SetParent(parentGo.transform, true);
                }

                // Transform
                if (request.position != null)
                    go.transform.localPosition = new Vector3(request.position.x, request.position.y, request.position.z);
                if (request.rotation != null)
                    go.transform.localEulerAngles = new Vector3(request.rotation.x, request.rotation.y, request.rotation.z);
                if (request.scale != null)
                    go.transform.localScale = new Vector3(request.scale.x, request.scale.y, request.scale.z);

                // Set Active
                if (request.set_active.HasValue)
                    go.SetActive(request.set_active.Value);

                // Add Components
                if (request.components_to_add != null)
                {
                    foreach (var typeName in request.components_to_add)
                    {
                        var type = Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
                        if (type != null && typeof(Component).IsAssignableFrom(type))
                        {
                            go.AddComponent(type);
                        }
                    }
                }

                // Remove Components
                if (request.components_to_remove != null)
                {
                    foreach (var typeName in request.components_to_remove)
                    {
                        var type = Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
                        if (type != null && typeof(Component).IsAssignableFrom(type))
                        {
                            var comp = go.GetComponent(type);
                            if (comp != null)
                                UnityEngine.Object.DestroyImmediate(comp);
                        }
                    }
                }

                // Modify Component Properties
                if (request.component_properties != null)
                {
                    foreach (var kvp in request.component_properties)
                    {
                        var typeName = kvp.Key;
                        var propDict = kvp.Value;
                        var type = Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
                        if (type != null && typeof(Component).IsAssignableFrom(type))
                        {
                            var comp = go.GetComponent(type);
                            if (comp != null)
                            {
                                foreach (var prop in propDict)
                                {
                                    var pi = type.GetProperty(prop.Key, BindingFlags.Public | BindingFlags.Instance);
                                    if (pi != null && pi.CanWrite)
                                    {
                                        pi.SetValue(comp, Convert.ChangeType(prop.Value, pi.PropertyType));
                                    }
                                    else
                                    {
                                        var fi = type.GetField(prop.Key, BindingFlags.Public | BindingFlags.Instance);
                                        if (fi != null)
                                            fi.SetValue(comp, Convert.ChangeType(prop.Value, fi.FieldType));
                                    }
                                }
                            }
                        }
                    }
                }

                EditorUtility.SetDirty(go);

                response.success = true;
                response.message = $"GameObject '{go.name}' modified successfully.";
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = $"Error modifying GameObject: {ex.Message}";
            }

            return response;
        }

        private static GameObject FindTarget(string target)
        {
            // Try by name, then by path, then by instance id
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
