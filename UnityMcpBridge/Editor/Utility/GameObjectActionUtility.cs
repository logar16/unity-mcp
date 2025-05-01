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

            var props = new Dictionary<string, object>();
            var type = comp.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (var prop in type.GetProperties(flags).Where(p => p.CanRead && p.GetIndexParameters().Length == 0))
            {
                try { props[prop.Name] = prop.GetValue(comp); } catch { }
            }
            foreach (var field in type.GetFields(flags))
            {
                try { props[field.Name] = field.GetValue(comp); } catch { }
            }
            return new GameObjectComponentDetails
            {
                type_name = type.FullName,
                properties = props
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
            var type = comp.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (var kvp in properties)
            {
                var prop = type.GetProperty(kvp.Key, flags);
                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        object value = Convert.ChangeType(kvp.Value, prop.PropertyType);
                        prop.SetValue(comp, value);
                        continue;
                    }
                    catch { }
                }
                var field = type.GetField(kvp.Key, flags);
                if (field != null)
                {
                    try
                    {
                        object value = Convert.ChangeType(kvp.Value, field.FieldType);
                        field.SetValue(comp, value);
                    }
                    catch { }
                }
            }
        }
    }
}
