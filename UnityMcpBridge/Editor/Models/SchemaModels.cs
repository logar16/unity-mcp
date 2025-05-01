using System.Collections.Generic;

namespace UnityMcp.Editor.Models
{
    /// <summary>
    /// Request DTO for retrieving action schemas. Optionally specify action names to filter.
    /// </summary>
    public class GetActionSchemasRequest: BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("action_names")]
        [SchemaDocumentation(
            "Optional list of action names to retrieve schemas for. If null or empty, returns all.",
            Notes = "Example: [\"create_gameobject\", \"load_scene\"]"
        )]
        public List<string> ActionNames { get; set; }
    }

    /// <summary>
    /// Response DTO mapping action names to their input/output JSON schemas.
    /// </summary>
    public class GetActionSchemasResponse: BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("schemas")]
        public Dictionary<string, ActionSchemaInfo> Schemas { get; set; }
    }

    /// <summary>
    /// Contains input and output schema for an action.
    /// </summary>
    public class ActionSchemaInfo
    {
        /// <summary>
        /// JSON Schema for the action's request DTO.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("inputSchema")]
        public object InputSchema { get; set; }

        // TODO: OutputSchema generation is disabled until the registry provides response type info.
        // [Newtonsoft.Json.JsonProperty("outputSchema")]
        // public object OutputSchema { get; set; }
    }
}
