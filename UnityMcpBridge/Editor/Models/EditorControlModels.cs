// Copyright (c) 2025 Logar16. All rights reserved.

namespace UnityMcp.Editor.Models
{
    // --- Menus ---

    public class ExecuteMenuItemRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("menu_path")]
        [SchemaDocumentation(
            "The full path of the Unity Editor menu item to execute",
            Notes = "Example: 'File/Save Project'"
        )]
        public string menu_path { get; set; }
    }

    public class ExecuteMenuItemResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("data")]
        public object data { get; set; }
    }

    // --- Play ---
    public class PlayRequest : BaseActionRequest
    {
        // Optionally: public bool wait_for_completion { get; set; }
    }

    public class PlayResponse : BaseActionResponse
    {
    }

    // --- Pause ---
    public class PauseRequest : BaseActionRequest
    {
    }

    public class PauseResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("is_paused")]
        public bool isPaused { get; set; }
    }

    // --- Stop ---
    public class StopRequest : BaseActionRequest
    {
    }

    public class StopResponse : BaseActionResponse
    {
    }

    // --- GetState ---
    public class GetStateRequest : BaseActionRequest
    {
    }

    public class GetStateResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("is_playing")]
        public bool isPlaying { get; set; }

        [Newtonsoft.Json.JsonProperty("is_paused")]
        public bool isPaused { get; set; }

        [Newtonsoft.Json.JsonProperty("is_compiling")]
        public bool isCompiling { get; set; }

        [Newtonsoft.Json.JsonProperty("is_updating")]
        public bool isUpdating { get; set; }

        [Newtonsoft.Json.JsonProperty("application_path")]
        public string applicationPath { get; set; }

        [Newtonsoft.Json.JsonProperty("application_contents_path")]
        public string applicationContentsPath { get; set; }

        [Newtonsoft.Json.JsonProperty("time_since_startup")]
        public double timeSinceStartup { get; set; }
    }
}
