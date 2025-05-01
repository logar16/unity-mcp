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
    public static class PrefabActionUtility
    {
        public static GameObject LoadPrefabRoot(string prefabPath)
        {
            if (string.IsNullOrEmpty(prefabPath))
                return null;
            string assetPath = "Assets/" + prefabPath.TrimStart('/');
            return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        }

        // --- NEW: Safe load/save/unload for editing prefab contents ---
        public static GameObject SafeLoadPrefabContents(string prefabPath, out string assetPath)
        {
            assetPath = "Assets/" + prefabPath.TrimStart('/');
            return PrefabUtility.LoadPrefabContents(assetPath);
        }

        public static void SaveAndUnloadPrefabContents(GameObject prefabRoot, string assetPath)
        {
            try
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        public static PrefabInfoSummary GetSummary(GameObject prefabRoot, string prefabPath)
        {
            if (prefabRoot == null) return null;
            return new PrefabInfoSummary
            {
                prefab_path = prefabPath,
                root_name = prefabRoot.name,
                root_tag = prefabRoot.tag,
                root_layer = prefabRoot.layer,
                root_component_count = prefabRoot.GetComponents<Component>().Length
            };
        }

        public static PrefabHierarchyNode BuildHierarchy(GameObject go, string pathPrefix = "")
        {
            if (go == null) return null;
            string path = string.IsNullOrEmpty(pathPrefix) ? go.name : pathPrefix + "/" + go.name;
            var node = new PrefabHierarchyNode
            {
                name = go.name,
                path = path,
                tag = go.tag,
                layer = go.layer,
                component_types = go.GetComponents<Component>().Select(c => c.GetType().FullName).ToList(),
                children = new List<PrefabHierarchyNode>()
            };
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i).gameObject;
                node.children.Add(BuildHierarchy(child, path));
            }
            return node;
        }

        public static GameObject FindChildByPath(GameObject root, string childPath)
        {
            if (root == null || string.IsNullOrEmpty(childPath)) return null;
            var segments = childPath.Split('/');
            GameObject current = root;
            if (current.name != segments[0]) return null;
            for (int i = 1; i < segments.Length; i++)
            {
                bool found = false;
                for (int j = 0; j < current.transform.childCount; j++)
                {
                    var child = current.transform.GetChild(j).gameObject;
                    if (child.name == segments[i])
                    {
                        current = child;
                        found = true;
                        break;
                    }
                }
                if (!found) return null;
            }
            return current;
        }

        public static PrefabComponentDetails GetComponentDetails(GameObject go, string childPath, string componentType)
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
            return new PrefabComponentDetails
            {
                child_path = childPath,
                type_name = type.FullName,
                properties = props
            };
        }

        // --- NEW: Create and configure a child GameObject under a parent ---
        public static GameObject CreateChildGameObject(GameObject parent, PrefabChildProperties props)
        {
            GameObject child;
            if (!string.IsNullOrEmpty(props.primitive_type))
            {
                var primitiveType = GetPrimitiveType(props.primitive_type);
                child = GameObject.CreatePrimitive(primitiveType);
                child.name = props.name ?? primitiveType.ToString();
            }
            else
            {
                child = new GameObject(props.name ?? "New GameObject");
            }

            child.transform.SetParent(parent.transform, false);

            SetGameObjectProperties(child, props);

            // Add components
            if (props.components_to_add != null)
            {
                foreach (var compType in props.components_to_add)
                {
                    var type = Type.GetType(compType) ?? AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.FullName == compType || t.Name == compType);
                    if (type != null && typeof(Component).IsAssignableFrom(type))
                    {
                        child.AddComponent(type);
                    }
                }
            }

            return child;
        }

        // --- NEW: Set transform/tag/layer/active for GameObject ---
        public static void SetGameObjectProperties(GameObject go, PrefabChildProperties props)
        {
            if (props.position != null)
                go.transform.localPosition = props.position.ToVector3();
            if (props.rotation != null)
                go.transform.localEulerAngles = props.rotation.ToVector3();
            if (props.scale != null)
                go.transform.localScale = props.scale.ToVector3();
            if (!string.IsNullOrEmpty(props.tag))
                go.tag = props.tag;
            if (!string.IsNullOrEmpty(props.layer))
            {
                int layerIndex = LayerMask.NameToLayer(props.layer);
                go.layer = layerIndex >= 0 ? layerIndex : go.layer;
            }
        }

        // --- NEW: Set transform/tag/layer/active for GameObject (for modify) ---
        public static void SetGameObjectProperties(GameObject go, ModifyPrefabChildRequest req)
        {
            if (req.position != null)
                go.transform.localPosition = req.position.ToVector3();
            if (req.rotation != null)
                go.transform.localEulerAngles = req.rotation.ToVector3();
            if (req.scale != null)
                go.transform.localScale = req.scale.ToVector3();
            if (!string.IsNullOrEmpty(req.tag))
                go.tag = req.tag;
            if (!string.IsNullOrEmpty(req.layer))
            {
                int layerIndex = LayerMask.NameToLayer(req.layer);
                go.layer = layerIndex >= 0 ? layerIndex : go.layer;
            }
            if (req.set_active.HasValue)
                go.SetActive(req.set_active.Value);
        }

        private static PrimitiveType GetPrimitiveType(string primitiveType)
        {
            if (Enum.TryParse(primitiveType, true, out PrimitiveType type))
                return type;
            return PrimitiveType.Cube;
        }
    }
}
