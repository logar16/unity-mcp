using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Core
{
    public delegate BaseActionResponse ActionHandlerDelegate(BaseActionRequest request);

    public static class ActionRegistry
    {
        private static readonly Dictionary<string, (Type requestType, ActionHandlerDelegate handler)> _actions = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Registers an action handler delegate and its associated request DTO.
        /// </summary>
        public static void RegisterAction<TRequest>(string actionName, Func<TRequest, BaseActionResponse> handler)
            where TRequest : BaseActionRequest, new()
        {
            if (string.IsNullOrWhiteSpace(actionName))
            {
                Debug.LogWarning("[ActionRegistry] Attempted to register an action with an empty name.");
                return;
            }
            _actions[actionName] = (typeof(TRequest), req => handler((TRequest)req));
        }

        public static (Type requestType, ActionHandlerDelegate handler)? GetAction(string actionName)
        {
            if (_actions.TryGetValue(actionName, out var info))
                return info;
            Debug.LogWarning($"[ActionRegistry] Action '{actionName}' not found in registry.");
            return null;
        }

        public static BaseActionRequest DeserializeParams(JObject request)
        {
            string actionName = request["action"]?.ToString();
            var info = GetAction(actionName);
            if (info == null)
                throw new ArgumentException($"Action '{actionName}' not registered.");
            try
            {
                return (BaseActionRequest)request.ToObject(info.Value.requestType);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ActionRegistry] Failed to deserialize params for action '{actionName}' into type '{info.Value.requestType.Name}': {ex.Message}\nJSON: {request}");
                throw new InvalidOperationException($"Failed to deserialize parameters for action '{actionName}'. Check JSON structure and DTO definition.", ex);
            }
        }
        /// <summary>
        /// Returns all registered actions with their request types and handlers.
        /// </summary>
        public static IReadOnlyDictionary<string, (Type requestType, ActionHandlerDelegate handler)> GetAllActions()
        {
            return _actions;
        }

        /// <summary>
        /// Runs an action by deserializing the request, invoking the handler, and returning the serialized JSON response.
        /// </summary>
        public static string RunAction(JObject request)
        {
            string actionName = request["action"]?.ToString();
            string id = request["id"]?.ToString();
            BaseActionResponse response = null;
            try
            {
                var actionInfo = GetAction(actionName);
                if (actionInfo == null)
                {
                    response = new BaseActionResponse
                    {
                        id = id,
                        action = actionName,
                        success = false,
                        message = $"Action '{actionName}' not registered."
                    };
                    return Newtonsoft.Json.JsonConvert.SerializeObject(response);
                }
                var reqObj = DeserializeParams(request);
                response = actionInfo.Value.handler(reqObj);
            }
            catch (Exception ex)
            {
                response = new BaseActionResponse
                {
                    id = id,
                    action = actionName,
                    success = false,
                    message = $"Internal error: {ex.Message}"
                };
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(response);
        }
    }
}
