using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Core;

namespace UnityMcp.Editor.Actions
{
    /// <summary>
    /// Action handler for retrieving JSON schemas for all registered actions, including XML doc comments.
    /// </summary>
    [InitializeOnLoad]
    public static class GetActionSchemasActionHandler
    {
        private static Dictionary<string, ActionSchemaInfo> _schemaCache;
        private static DateTime _lastAssemblyWriteTime;

        static GetActionSchemasActionHandler()
        {
            Debug.Log("[UnityMcp] Registering 'get_action_schemas' action handler.");
            ActionRegistry.RegisterAction<GetActionSchemasRequest>("get_action_schemas", Handle);
        }

        public static BaseActionResponse Handle(GetActionSchemasRequest request)
        {
            Debug.Log("[UnityMcp] Handling 'get_action_schemas' request.");
            EnsureSchemaCache();

            var registry = ActionRegistry.GetAllActions();
            Debug.Log($"[UnityMcp] Registry count: {registry.Count}, keys: [{string.Join(", ", registry.Keys)}]");

            var result = new Dictionary<string, ActionSchemaInfo>();

            IEnumerable<string> actionNames = registry.Keys;
            if (request?.ActionNames != null && request.ActionNames.Count > 0)
            {
                Debug.Log($"[UnityMcp] Filtering action names. Request.ActionNames: [{string.Join(", ", request.ActionNames)}]");
                actionNames = actionNames.Intersect(request.ActionNames);
            }
            else
            {
                Debug.Log("[UnityMcp] No ActionNames filter provided, using all registry keys.");
            }

            Debug.Log($"[UnityMcp] Action names to process: [{string.Join(", ", actionNames)}]");
            Debug.Log($"[UnityMcp] _schemaCache count: {_schemaCache?.Count ?? -1}");

            foreach (var actionName in actionNames)
            {
                if (_schemaCache.TryGetValue(actionName, out var schema))
                    result[actionName] = schema;
                else
                    Debug.LogWarning($"[UnityMcp] No schema found in cache for action: {actionName}");
            }

            Debug.Log($"[UnityMcp] Returning {result.Count} schemas.");
            return new GetActionSchemasResponse { Schemas = result }.Success(request);
        }

        private static void EnsureSchemaCache()
        {
            var assemblyPath = typeof(GetActionSchemasActionHandler).Assembly.Location;
            var writeTime = System.IO.File.GetLastWriteTimeUtc(assemblyPath);

            if (_schemaCache != null && writeTime == _lastAssemblyWriteTime)
                return;

            _lastAssemblyWriteTime = writeTime;
            _schemaCache = new Dictionary<string, ActionSchemaInfo>();

            var registry = ActionRegistry.GetAllActions();
            var xmlDocPath = assemblyPath.Replace(".dll", ".xml");

            var xmlDocs = XmlDocCommentProvider.TryLoad(xmlDocPath);

            foreach (var kvp in registry)
            {
                var requestType = kvp.Value.requestType;
                // Assume output schema is BaseActionResponse or use a convention if available
                var outputType = typeof(BaseActionResponse);

                var inputSchema = JsonSchemaGenerator.GenerateSchema(requestType, xmlDocs);
                // TODO: We don't provide output schema for now, as the registry doesn't provide response type info.
                // var outputSchema = JsonSchemaGenerator.GenerateSchema(outputType, xmlDocs);

                _schemaCache[kvp.Key] = new ActionSchemaInfo
                {
                    InputSchema = inputSchema,
                    // OutputSchema = outputSchema
                };
            }
        }
    }
}
