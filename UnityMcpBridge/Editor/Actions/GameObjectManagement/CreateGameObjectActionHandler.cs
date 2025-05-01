// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions
{
    [InitializeOnLoad]
    public static class CreateGameObjectActionHandler
    {
        static CreateGameObjectActionHandler()
        {
            ActionRegistry.RegisterAction<CreateGameObjectRequest>(
                "create_gameobject",
                HandleCreateGameObject
            );
        }

        public static CreateGameObjectResponse HandleCreateGameObject(CreateGameObjectRequest request)
        {
            var response = new CreateGameObjectResponse();

            if (request == null || string.IsNullOrWhiteSpace(request.name))
            {
                response.success = false;
                response.message = "Missing required 'name' parameter.";
                return response;
            }

            GameObject newGo = null;
            try
            {
                // Primitive or empty
                if (!string.IsNullOrEmpty(request.primitive_type))
                {
                    if (Enum.TryParse(request.primitive_type, true, out PrimitiveType primitiveType))
                    {
                        newGo = GameObject.CreatePrimitive(primitiveType);
                        newGo.name = request.name;
                    }
                    else
                    {
                        response.success = false;
                        response.message = $"Invalid primitive type: '{request.primitive_type}'.";
                        return response;
                    }
                }
                else
                {
                    newGo = new GameObject(request.name);
                }

                Undo.RegisterCreatedObjectUndo(newGo, $"Create GameObject '{newGo.name}'");

                // Parent
                if (!string.IsNullOrEmpty(request.parent))
                {
                    var parentGo = GameObject.Find(request.parent);
                    if (parentGo != null)
                        newGo.transform.SetParent(parentGo.transform, true);
                    else
                    {
                        UnityEngine.Object.DestroyImmediate(newGo);
                        response.success = false;
                        response.message = $"Parent GameObject '{request.parent}' not found.";
                        return response;
                    }
                }

                // Transform
                if (request.position != null)
                    newGo.transform.localPosition = new Vector3(request.position.x, request.position.y, request.position.z);
                if (request.rotation != null)
                    newGo.transform.localEulerAngles = new Vector3(request.rotation.x, request.rotation.y, request.rotation.z);
                if (request.scale != null)
                    newGo.transform.localScale = new Vector3(request.scale.x, request.scale.y, request.scale.z);

                // Tag
                if (!string.IsNullOrEmpty(request.tag))
                {
                    try { newGo.tag = request.tag; }
                    catch
                    {
                        InternalEditorUtility.AddTag(request.tag);
                        newGo.tag = request.tag;
                    }
                }

                // Layer
                if (!string.IsNullOrEmpty(request.layer))
                {
                    int layerId = LayerMask.NameToLayer(request.layer);
                    if (layerId != -1)
                        newGo.layer = layerId;
                }

                // Components
                if (request.components_to_add != null)
                {
                    foreach (var typeName in request.components_to_add)
                    {
                        var type = Type.GetType(typeName) ?? AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);
                        if (type != null && typeof(Component).IsAssignableFrom(type))
                        {
                            newGo.AddComponent(type);
                        }
                    }
                }

                EditorUtility.SetDirty(newGo);

                response.success = true;
                response.message = $"GameObject '{newGo.name}' created successfully.";
                response.created_object = new GameObjectIdentifier
                {
                    instance_id = newGo.GetInstanceID(),
                    name = newGo.name,
                    path = newGo.transform.GetHierarchyPath(),
                    tag = newGo.tag,
                    layer = newGo.layer,
                    active_self = newGo.activeSelf,
                    transform = new TransformData
                    {
                        position = new Vector3Data { x = newGo.transform.localPosition.x, y = newGo.transform.localPosition.y, z = newGo.transform.localPosition.z },
                        rotation = new Vector3Data { x = newGo.transform.localEulerAngles.x, y = newGo.transform.localEulerAngles.y, z = newGo.transform.localEulerAngles.z },
                        scale = new Vector3Data { x = newGo.transform.localScale.x, y = newGo.transform.localScale.y, z = newGo.transform.localScale.z }
                    }
                };
            }
            catch (Exception ex)
            {
                if (newGo != null)
                    UnityEngine.Object.DestroyImmediate(newGo);
                response.success = false;
                response.message = $"Error creating GameObject: {ex.Message}";
            }

            return response;
        }

        // Helper to get hierarchy path
        private static string GetHierarchyPath(this Transform transform)
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
