using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityMcp.Editor.Actions
{
    /// <summary>
    /// Generates JSON Schema for C# types, including XML doc comments.
    /// </summary>
    internal static class JsonSchemaGenerator
    {
        public static object GenerateSchema(Type type, XmlDocCommentProvider xmlDocs)
        {
            var visited = new HashSet<Type>();
            return GenerateSchemaInternal(type, xmlDocs, visited);
        }

        private static object GenerateSchemaInternal(Type type, XmlDocCommentProvider xmlDocs, HashSet<Type> visited)
        {
            if (type == null)
                return null;

            // Handle primitives and enums first, before recursion check
            if (type.IsEnum)
            {
                return new
                {
                    type = "string",
                    @enum = Enum.GetNames(type),
                    description = xmlDocs?.GetTypeSummary(type)
                };
            }

            if (type == typeof(string))
                return new { type = "string", description = xmlDocs?.GetTypeSummary(type) };
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
                return new { type = "integer", description = xmlDocs?.GetTypeSummary(type) };
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return new { type = "number", description = xmlDocs?.GetTypeSummary(type) };
            if (type == typeof(bool))
                return new { type = "boolean", description = xmlDocs?.GetTypeSummary(type) };
            if (type == typeof(object))
                return new { type = "object", description = xmlDocs?.GetTypeSummary(type) };

            // Recursion check for complex types only
            if (visited.Contains(type))
                return new { type = "object", description = $"Recursive reference to {type.FullName}" };
            visited.Add(type);

            try
            {
                if (typeof(IDictionary).IsAssignableFrom(type) || IsGenericDictionary(type))
                {
                    var args = type.GetGenericArguments();
                    var keyType = args.Length > 0 ? args[0] : typeof(string);
                    var valueType = args.Length > 1 ? args[1] : typeof(object);
                    return new
                    {
                        type = "object",
                        additionalProperties = GenerateSchemaInternal(valueType, xmlDocs, visited),
                        description = xmlDocs?.GetTypeSummary(type)
                    };
                }

                if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
                {
                    var elemType = type.IsArray ? type.GetElementType() : type.GetGenericArguments().FirstOrDefault() ?? typeof(object);
                    return new
                    {
                        type = "array",
                        items = GenerateSchemaInternal(elemType, xmlDocs, visited),
                        description = xmlDocs?.GetTypeSummary(type)
                    };
                }

                // Complex object
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)
                    .ToArray();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                var required = new List<string>();
                var properties = new Dictionary<string, object>();

                foreach (var prop in props)
                {
                    var propType = prop.PropertyType;
                    var propSchema = GenerateSchemaInternal(propType, xmlDocs, visited);

                    // Check for SchemaDocumentationAttribute
                    var docAttr = prop.GetCustomAttribute<UnityMcp.Editor.Models.SchemaDocumentationAttribute>();
                    string desc = null;
                    string notes = null;
                    if (docAttr != null)
                    {
                        desc = docAttr.Description;
                        notes = docAttr.Notes;
                    }
                    else
                    {
                        desc = xmlDocs?.GetSummary(prop);
                    }

                    // Always set description, even for primitives/arrays
                    if (desc != null || notes != null)
                    {
                        IDictionary<string, object> dict = propSchema as IDictionary<string, object>;
                        if (dict == null)
                        {
                            // Wrap primitive/array schema in a dictionary to add description/x-notes
                            var schemaDict = new Dictionary<string, object>();
                            foreach (var kv in propSchema.GetType().GetProperties())
                            {
                                schemaDict[kv.Name] = kv.GetValue(propSchema);
                            }
                            dict = schemaDict;
                            propSchema = dict;
                        }
                        if (desc != null)
                        {
                            dict["description"] = desc;
                        }
                        else if (dict.ContainsKey("description"))
                        {
                            dict.Remove("description");
                        }
                        if (!string.IsNullOrEmpty(notes))
                            dict["x-notes"] = notes;
                    }
                    properties[prop.Name] = propSchema;

                    // [Required] attribute support
                    if (prop.GetCustomAttributes().Any(a => a.GetType().Name == "RequiredAttribute"))
                        required.Add(prop.Name);
                }
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    var fieldSchema = GenerateSchemaInternal(fieldType, xmlDocs, visited);

                    // Check for SchemaDocumentationAttribute
                    var docAttr = field.GetCustomAttribute<UnityMcp.Editor.Models.SchemaDocumentationAttribute>();
                    string desc = null;
                    string notes = null;
                    if (docAttr != null)
                    {
                        desc = docAttr.Description;
                        notes = docAttr.Notes;
                    }
                    else
                    {
                        desc = xmlDocs?.GetSummary(field);
                    }

                    // Always set description, even for primitives/arrays
                    if (desc != null || notes != null)
                    {
                        IDictionary<string, object> dict = fieldSchema as IDictionary<string, object>;
                        if (dict == null)
                        {
                            // Wrap primitive/array schema in a dictionary to add description/x-notes
                            var schemaDict = new Dictionary<string, object>();
                            foreach (var kv in fieldSchema.GetType().GetProperties())
                            {
                                schemaDict[kv.Name] = kv.GetValue(fieldSchema);
                            }
                            dict = schemaDict;
                            fieldSchema = dict;
                        }
                        if (desc != null)
                        {
                            dict["description"] = desc;
                        }
                        else if (dict.ContainsKey("description"))
                        {
                            dict.Remove("description");
                        }
                        if (!string.IsNullOrEmpty(notes))
                            dict["x-notes"] = notes;
                    }
                    properties[field.Name] = fieldSchema;

                    if (field.GetCustomAttributes().Any(a => a.GetType().Name == "RequiredAttribute"))
                        required.Add(field.Name);
                }

                var typeDesc = xmlDocs?.GetTypeSummary(type);

                var schema = new Dictionary<string, object>
                {
                    ["type"] = "object",
                    ["properties"] = properties
                };
                if (required.Count > 0)
                    schema["required"] = required;
                if (!string.IsNullOrEmpty(typeDesc))
                    schema["description"] = typeDesc;

                return schema;
            }
            finally
            {
                // Remove from visited so the same type can be expanded elsewhere
                visited.Remove(type);
            }
        }

        private static bool IsGenericDictionary(Type type)
        {
            return type.IsGenericType && (
                type.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            );
        }
    }
}
