// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Utility
{
    public static class GameObjectActionUtility
    {
        public static GameObject FindGameObject(string target)
        {
            if (string.IsNullOrEmpty(target))
                return null;

            // Try by instance ID
            if (int.TryParse(target, out int instanceId))
            {
                var all = Resources.FindObjectsOfTypeAll<GameObject>();
                return all.FirstOrDefault(go => go.GetInstanceID() == instanceId);
            }

            // Try by path
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            var byPath = allObjects.FirstOrDefault(go => GetHierarchyPath(go.transform) == target);
            if (byPath != null)
                return byPath;

            // Try by name (first match)
            return allObjects.FirstOrDefault(go => go.name == target);
        }

        public static string GetHierarchyPath(Transform transform)
        {
            var path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

        public static GameObjectInfoSummary GetSummary(GameObject go)
        {
            if (go == null) return null;
            var parent = go.transform.parent ? go.transform.parent.gameObject : null;
            return new GameObjectInfoSummary
            {
                instance_id = go.GetInstanceID(),
                name = go.name,
                tag = go.tag,
                layer = go.layer,
                active_self = go.activeSelf,
                parent_instance_id = parent?.GetInstanceID(),
                parent_name = parent?.name,
                child_count = go.transform.childCount
            };
        }

        public static GameObjectInfoDetailed GetDetailed(GameObject go)
        {
            if (go == null) return null;
            var summary = GetSummary(go);
            var detailed = new GameObjectInfoDetailed
            {
                instance_id = summary.instance_id,
                name = summary.name,
                tag = summary.tag,
                layer = summary.layer,
                active_self = summary.active_self,
                parent_instance_id = summary.parent_instance_id,
                parent_name = summary.parent_name,
                child_count = summary.child_count,
                local_transform = new TransformData
                {
                    position = Vector3Data.FromVector3(go.transform.localPosition),
                    rotation = Vector3Data.FromVector3(go.transform.localEulerAngles),
                    world_position = null,
                    world_rotation = null,
                    scale = Vector3Data.FromVector3(go.transform.localScale)
                },
                world_transform = new TransformData
                {
                    position = Vector3Data.FromVector3(go.transform.position),
                    rotation = Vector3Data.FromVector3(go.transform.eulerAngles),
                    world_position = Vector3Data.FromVector3(go.transform.position),
                    world_rotation = Vector3Data.FromVector3(go.transform.eulerAngles),
                    scale = Vector3Data.FromVector3(go.transform.lossyScale)
                },
                component_types = go.GetComponents<Component>().Select(c => c.GetType().FullName).ToList()
            };
            return detailed;
        }

        public static GameObjectComponentDetails GetComponentDetails(GameObject go, string componentType)
        {
            if (go == null || string.IsNullOrEmpty(componentType)) return null;
            var comp = go.GetComponents<Component>().FirstOrDefault(c =>
                c.GetType().FullName == componentType || c.GetType().Name == componentType);
            if (comp == null) return null;

            return new GameObjectComponentDetails
            {
                type_name = comp.GetType().FullName,
                properties = ComponentUtility.GetSerializableProperties(comp)
            };
        }
        public static Component AddComponentByType(GameObject go, string componentType, Dictionary<string, object> properties = null)
        {
            if (go == null || string.IsNullOrEmpty(componentType))
                return null;
            var type = Type.GetType(componentType);
            if (type == null)
                return null;
            var comp = go.AddComponent(type);
            if (comp != null && properties != null)
            {
                SetComponentProperties(comp, properties);
            }
            return comp;
        }

        public static bool RemoveComponentByType(GameObject go, string componentType)
        {
            if (go == null || string.IsNullOrEmpty(componentType))
                return false;
            var type = Type.GetType(componentType);
            if (type == null)
                return false;
            var comp = go.GetComponent(type);
            if (comp == null)
                return false;
#if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(comp, true);
#else
            UnityEngine.Object.Destroy(comp);
#endif
            return true;
        }

        public static bool ModifyComponentProperties(GameObject go, string componentType, Dictionary<string, object> properties)
        {
            if (go == null || string.IsNullOrEmpty(componentType) || properties == null)
                return false;
            var type = Type.GetType(componentType);
            if (type == null)
                return false;
            var comp = go.GetComponent(type);
            if (comp == null)
                return false;
            SetComponentProperties(comp, properties);
            return true;
        }

        private static void SetComponentProperties(Component comp, Dictionary<string, object> properties)
        {
            ComponentUtility.SetComponentProperties(comp, properties);
        }

        public static GameObjectHierarchyNode BuildGameObjectHierarchy(GameObject go, string pathPrefix = "", bool includeChildren = true, bool includeComponentDetails = false)
        {
            if (go == null)
                return null;

            // Build the full hierarchy path
            string path = string.IsNullOrEmpty(pathPrefix)
                ? GetHierarchyPath(go.transform)
                : pathPrefix + "/" + go.name;

            // Create and populate the node
            var node = new GameObjectHierarchyNode
            {
                name = go.name,
                path = path,
                tag = go.tag,
                layer = go.layer,
                instance_id = go.GetInstanceID(),
                component_types = go.GetComponents<Component>()
                    .Select(c => c?.GetType().FullName ?? "null")
                    .ToList()
            };

            // Add detailed component information if requested
            if (includeComponentDetails)
            {
                node.components = new List<DetailedComponentInfo>();
                foreach (var component in go.GetComponents<Component>())
                {
                    if (component != null)
                    {
                        node.components.Add(new DetailedComponentInfo
                        {
                            type_name = component.GetType().FullName,
                            properties = ComponentUtility.GetSerializableProperties(component)
                        });
                    }
                }
            }

            // Process children if requested
            if (includeChildren && go.transform.childCount > 0)
            {
                node.children = new List<GameObjectHierarchyNode>();
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var childGo = go.transform.GetChild(i).gameObject;
                    var childNode = BuildGameObjectHierarchy(childGo, path, includeChildren, includeComponentDetails);
                    if (childNode != null)
                    {
                        node.children.Add(childNode);
                    }
                }
            }

            return node;
        }
    }
}
