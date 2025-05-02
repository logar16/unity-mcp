// Copyright (c) 2025 Logar16. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Utility
{
    /// <summary>
    /// Utility class for handling Unity Component operations, particularly property serialization
    /// </summary>
    public static class ComponentUtility
    {
        private static readonly Dictionary<Type, List<FieldInfo>> _serializableFieldsCache = new Dictionary<Type, List<FieldInfo>>();

        /// <summary>
        /// Gets serializable properties from a component using Unity's serialization rules ([SerializeField] and public fields)
        /// </summary>
        /// <param name="component">The component to get properties from</param>
        /// <returns>Dictionary of property names and their serialized values</returns>
        public static Dictionary<string, object> GetSerializableProperties(Component component)
        {
            if (component == null)
                return new Dictionary<string, object>();

            var result = new Dictionary<string, object>();
            var type = component.GetType();

            // Handle fields
            if (!_serializableFieldsCache.TryGetValue(type, out var fields))
            {
                fields = GetSerializableFields(type);
                _serializableFieldsCache[type] = fields;
            }

            // Get both fields and properties that Unity would serialize
            var members = new List<(string name, object value)>();

            // Add fields
            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(component);
                    members.Add((field.Name, value));
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error getting field {field.Name} on {type.Name}: {ex.Message}");
                }
            }

            // Add properties that Unity would serialize (common component properties)
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

            foreach (var prop in props)
            {
                try
                {
                    // Only include specific Unity component properties we know are safe
                    if (IsUnityComponentProperty(type, prop.Name))
                    {
                        var value = prop.GetValue(component);
                        members.Add((prop.Name, value));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error getting property {prop.Name} on {type.Name}: {ex.Message}");
                }
            }

            // Serialize all collected members
            foreach (var (name, value) in members)
            {
                try
                {
                    result[name] = SerializeValue(value);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error serializing {name} on {type.Name}: {ex.Message}");
                }
            }

            return result;
        }

        private static List<FieldInfo> GetSerializableFields(Type type)
        {
            var result = new List<FieldInfo>();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in type.GetFields(flags))
            {
                // Skip backing fields and obsolete fields
                if (field.Name.EndsWith("k__BackingField") ||
                    Attribute.IsDefined(field, typeof(ObsoleteAttribute)))
                    continue;

                // Include if public or has SerializeField attribute
                if (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))
                {
                    // Extra validation - check if the type is something Unity might serialize
                    if (IsPotentiallySerializableByUnity(field.FieldType))
                    {
                        result.Add(field);
                    }
                }
            }

            return result;
        }

        private static bool IsPotentiallySerializableByUnity(Type type)
        {
            if (type == null) return false;

            // Unity built-in types and primitives
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum ||
                typeof(UnityEngine.Object).IsAssignableFrom(type))
                return true;

            // Common Unity structs
            if (IsCommonUnityStruct(type))
                return true;

            // Arrays and Lists of serializable types
            if (type.IsArray)
                return IsPotentiallySerializableByUnity(type.GetElementType());

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return IsPotentiallySerializableByUnity(type.GetGenericArguments()[0]);

            // Custom serializable classes
            return Attribute.IsDefined(type, typeof(SerializableAttribute));
        }

        private static bool IsCommonUnityStruct(Type type)
        {
            return type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4) ||
                   type == typeof(Quaternion) || type == typeof(Color) || type == typeof(Rect) ||
                   type == typeof(Bounds) || type == typeof(Matrix4x4);
        }

        private static object SerializeValue(object value)
        {
            if (value == null) return null;
            var type = value.GetType();

            // Handle primitives, strings, and enums directly
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
                return value;

            // Handle Unity structs
            if (value is Vector2 v2)
                return new Dictionary<string, float> { { "x", v2.x }, { "y", v2.y } };
            if (value is Vector3 v3)
                return new Dictionary<string, float> { { "x", v3.x }, { "y", v3.y }, { "z", v3.z } };
            if (value is Vector4 v4)
                return new Dictionary<string, float> { { "x", v4.x }, { "y", v4.y }, { "z", v4.z }, { "w", v4.w } };
            if (value is Quaternion q)
                return new Dictionary<string, float> { { "x", q.x }, { "y", q.y }, { "z", q.z }, { "w", q.w } };
            if (value is Color c)
                return new Dictionary<string, float> { { "r", c.r }, { "g", c.g }, { "b", c.b }, { "a", c.a } };
            if (value is Rect r)
                return new Dictionary<string, float> { { "x", r.x }, { "y", r.y }, { "width", r.width }, { "height", r.height } };

            // Handle UnityEngine.Object references
            if (value is UnityEngine.Object unityObj)
                return new Dictionary<string, object> {
                    { "instanceID", unityObj.GetInstanceID() },
                    { "name", unityObj.name },
                    { "type", unityObj.GetType().FullName }
                };

            // Handle arrays
            if (type.IsArray)
            {
                var array = (Array)value;
                var result = new List<object>(array.Length);
                for (int i = 0; i < array.Length; i++)
                    result.Add(SerializeValue(array.GetValue(i)));
                return result;
            }

            // Handle Lists
            if (value is IList list)
            {
                var result = new List<object>(list.Count);
                foreach (var item in list)
                    result.Add(SerializeValue(item));
                return result;
            }

            // For other serializable types, return type info only
            return $"[Serializable: {type.Name}]";
        }

        public static DetailedComponentInfo GetDetailedComponentInfo(Component component)
        {
            if (component == null)
                return null;

            return new DetailedComponentInfo
            {
                type_name = component.GetType().FullName,
                properties = GetSerializableProperties(component)
            };
        }

        /// <summary>
        /// Sets serializable properties on a component using Unity's serialization rules
        /// </summary>
        /// <param name="component">The component to modify</param>
        /// <param name="properties">Dictionary of property names and values to set</param>
        public static void SetComponentProperties(Component component, Dictionary<string, object> properties)
        {
            if (component == null || properties == null)
                return;

            var type = component.GetType();

            // Get cached or compute serializable fields
            if (!_serializableFieldsCache.TryGetValue(type, out var fields))
            {
                fields = GetSerializableFields(type);
                _serializableFieldsCache[type] = fields;
            }

            // Set each field that exists in the properties dictionary
            foreach (var field in fields)
            {
                if (properties.TryGetValue(field.Name, out var value))
                {
                    try
                    {
                        // Convert value to the correct type
                        var convertedValue = ConvertValue(value, field.FieldType);
                        field.SetValue(component, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to set field {field.Name} on {type.Name}: {ex.Message}");
                    }
                }
            }
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null;

            // Handle Unity struct conversions
            if (targetType == typeof(Vector2) && value is Dictionary<string, float> v2Dict)
                return new Vector2(v2Dict.GetValueOrDefault("x"), v2Dict.GetValueOrDefault("y"));

            if (targetType == typeof(Vector3) && value is Dictionary<string, float> v3Dict)
                return new Vector3(v3Dict.GetValueOrDefault("x"), v3Dict.GetValueOrDefault("y"), v3Dict.GetValueOrDefault("z"));

            if (targetType == typeof(Vector4) && value is Dictionary<string, float> v4Dict)
                return new Vector4(v4Dict.GetValueOrDefault("x"), v4Dict.GetValueOrDefault("y"),
                                 v4Dict.GetValueOrDefault("z"), v4Dict.GetValueOrDefault("w"));

            if (targetType == typeof(Quaternion) && value is Dictionary<string, float> qDict)
                return new Quaternion(qDict.GetValueOrDefault("x"), qDict.GetValueOrDefault("y"),
                                    qDict.GetValueOrDefault("z"), qDict.GetValueOrDefault("w"));

            if (targetType == typeof(Color) && value is Dictionary<string, float> cDict)
                return new Color(cDict.GetValueOrDefault("r"), cDict.GetValueOrDefault("g"),
                               cDict.GetValueOrDefault("b"), cDict.GetValueOrDefault("a"));

            // For arrays and lists, handle element-wise conversion
            if (targetType.IsArray && value is IList sourceList)
            {
                var elementType = targetType.GetElementType();
                var array = Array.CreateInstance(elementType, sourceList.Count);
                for (int i = 0; i < sourceList.Count; i++)
                {
                    array.SetValue(ConvertValue(sourceList[i], elementType), i);
                }
                return array;
            }

            // For simple type conversion
            return Convert.ChangeType(value, targetType);
        }

        private static bool IsUnityComponentProperty(Type componentType, string propertyName)
        {
            // Transform properties
            if (componentType == typeof(Transform))
            {
                return propertyName is "position" or "localPosition" or
                                      "rotation" or "localRotation" or
                                      "eulerAngles" or "localEulerAngles" or
                                      "localScale";
            }

            // Collider properties
            if (typeof(Collider).IsAssignableFrom(componentType))
            {
                return propertyName is "isTrigger" or "enabled" or "center" or
                                      "size" or "radius" or "height" or
                                      "direction" or "bounds";
            }

            // Renderer properties
            if (typeof(Renderer).IsAssignableFrom(componentType))
            {
                return propertyName is "enabled" or "material" or "materials" or
                                      "sharedMaterial" or "sharedMaterials";
            }

            // MeshFilter properties
            if (componentType == typeof(MeshFilter))
            {
                return propertyName is "mesh" or "sharedMesh";
            }

            // Common MonoBehaviour properties
            if (typeof(MonoBehaviour).IsAssignableFrom(componentType))
            {
                return propertyName is "enabled";
            }

            // Add other component types as needed...

            return false;
        }
    }
}
