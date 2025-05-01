namespace UnityMcp.Editor.Models
{
    // CREATE SCRIPT
    public class CreateScriptRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        [SchemaDocumentation(
            "Optional. Relative to the Assets folder. Used to identify or specify the location for script creation.",
            Notes = "If omitted, defaults to 'Scripts'."
        )]
        public string path { get; set; }

        [Newtonsoft.Json.JsonProperty("contents")]
        [SchemaDocumentation(
            "Full script text. If omitted, a default template is generated."
        )]
        public string contents { get; set; }

        [Newtonsoft.Json.JsonProperty("script_type")]
        [SchemaDocumentation(
            "Optional. Determines the script template and base class.",
            Notes = "Examples: MonoBehaviour, ScriptableObject, EditorWindow."
        )]
        public string script_type { get; set; }

        [Newtonsoft.Json.JsonProperty("namespace")]
        [SchemaDocumentation(
            "Optional. Wraps the script in a C# namespace.",
            Notes = "Ignored if not provided."
        )]
        public string @namespace { get; set; }
    }

    public class CreateScriptResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }
    }

    // READ SCRIPT
    public class ReadScriptRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        [SchemaDocumentation(
            "Optional. Relative to the Assets folder. Used to identify the location of the script.",
            Notes = "If omitted, defaults to 'Scripts'."
        )]
        public string path { get; set; }
    }

    public class ReadScriptResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("contents")]
        public string contents { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }
    }

    // UPDATE SCRIPT
    public class UpdateScriptRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        [SchemaDocumentation(
            "Optional. Relative to the Assets folder. Used to identify the location of the script.",
            Notes = "If omitted, defaults to 'Scripts'."
        )]
        public string path { get; set; }

        [Newtonsoft.Json.JsonProperty("contents")]
        [SchemaDocumentation(
            "Full script text to overwrite the file."
        )]
        public string contents { get; set; }
    }

    public class UpdateScriptResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }
    }

    // DELETE SCRIPT
    public class DeleteScriptRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        [SchemaDocumentation(
            "Optional. Relative to the Assets folder. Used to identify the location of the script.",
            Notes = "If omitted, defaults to 'Scripts'."
        )]
        public string path { get; set; }
    }

    public class DeleteScriptResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }
    }
}
