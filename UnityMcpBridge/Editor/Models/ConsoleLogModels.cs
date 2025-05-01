using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Models
{
    // DTO for a single console log entry
    [Serializable]
    public class ConsoleLogEntry
    {
        [JsonProperty("type")]
        public string type;

        [JsonProperty("message")]
        public string message;

        [JsonProperty("stacktrace")]
        public string stacktrace;
    }

    // Request DTO for get_console_logs
    [Serializable]
    public class GetConsoleLogsRequest : BaseActionRequest
    {
        [SchemaDocumentation(
            "Filters the returned log entries by log type.",
            Notes = "Allowed values: 'error', 'warning', 'log'."
        )]
        [JsonProperty("types")]
        public List<string> types;

        [SchemaDocumentation(
            "Filters the returned log entries by substring match in the message."
        )]
        [JsonProperty("filter_text")]
        public string filter_text;

        [SchemaDocumentation(
            "Limits the maximum number of log entries returned."
        )]
        [JsonProperty("count")]
        public int? count;
    }

    // Response DTO for get_console_logs
    [Serializable]
    public class GetConsoleLogsResponse : BaseActionResponse
    {
        [JsonProperty("entries")]
        public List<ConsoleLogEntry> entries;
    }

    // Request DTO for clear_console (empty)
    [Serializable]
    public class ClearConsoleRequest : BaseActionRequest
    {
    }

    // Response DTO for clear_console
    [Serializable]
    public class ClearConsoleResponse : BaseActionResponse
    {
    }
}
