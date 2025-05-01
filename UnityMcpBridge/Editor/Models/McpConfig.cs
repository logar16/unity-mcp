using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UnityMcp.Editor.Models
{
    public enum McpTypes
    {
        ClaudeDesktop,
        Cursor,
        Cline,
        Roo,
    }

    public enum McpStatus
    {
        NotConfigured, // Not set up yet
        Configured, // Successfully configured
        Running, // Service is running
        Connected, // Successfully connected
        IncorrectPath, // Configuration has incorrect paths
        CommunicationError, // Connected but communication issues
        NoResponse, // Connected but not responding
        MissingConfig, // Config file exists but missing required elements
        UnsupportedOS, // OS is not supported
        Error, // General error state
    }

    public class McpClient
    {
        public string name;
        public string windowsConfigPath;
        public string linuxConfigPath;
        public McpTypes mcpType;
        public string configStatus;
        public McpStatus status = McpStatus.NotConfigured;

        // Helper method to convert the enum to a display string
        public string GetStatusDisplayString()
        {
            return status switch
            {
                McpStatus.NotConfigured => "Not Configured",
                McpStatus.Configured => "Configured",
                McpStatus.Running => "Running",
                McpStatus.Connected => "Connected",
                McpStatus.IncorrectPath => "Incorrect Path",
                McpStatus.CommunicationError => "Communication Error",
                McpStatus.NoResponse => "No Response",
                McpStatus.UnsupportedOS => "Unsupported OS",
                McpStatus.MissingConfig => "Missing UnityMCP Config",
                McpStatus.Error => configStatus.StartsWith("Error:") ? configStatus : "Error",
                _ => "Unknown",
            };
        }

        // Helper method to set both status enum and string for backward compatibility
        public void SetStatus(McpStatus newStatus, string errorDetails = null)
        {
            status = newStatus;

            if (newStatus == McpStatus.Error && !string.IsNullOrEmpty(errorDetails))
            {
                configStatus = $"Error: {errorDetails}";
            }
            else
            {
                configStatus = GetStatusDisplayString();
            }
        }
    }


    [Serializable]
    public class ServerConfig
    {
        [JsonProperty("unity_host")]
        public string unityHost = "localhost";

        [JsonProperty("unity_port")]
        public int unityPort;

        [JsonProperty("mcp_port")]
        public int mcpPort;

        [JsonProperty("connection_timeout")]
        public float connectionTimeout;

        [JsonProperty("buffer_size")]
        public int bufferSize;

        [JsonProperty("log_level")]
        public string logLevel;

        [JsonProperty("log_format")]
        public string logFormat;

        [JsonProperty("max_retries")]
        public int maxRetries;

        [JsonProperty("retry_delay")]
        public float retryDelay;
    }

    [Serializable]
    public class McpConfigServer
    {
        [JsonProperty("command")]
        public string command;

        [JsonProperty("args")]
        public string[] args;
    }

    [Serializable]
    public class McpConfigServers
    {
        [JsonProperty("unityMCP")]
        public McpConfigServer unityMCP;
    }

    [Serializable]
    public class McpConfig
    {
        [JsonProperty("mcpServers")]
        public McpConfigServers mcpServers;
    }
}
