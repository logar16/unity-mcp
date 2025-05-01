// Copyright (c) 2025 Logar16. All rights reserved.

using System.Collections.Generic;

namespace UnityMcp.Editor.Models
{
    // CREATE SCENE
    public class CreateSceneRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        [SchemaDocumentation(
            "Optional directory (relative to Assets) where the scene will be created.",
            Notes = "Defaults to 'Assets/Scenes' if not specified."
        )]
        public string path { get; set; }
    }

    public class CreateSceneResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("scene_path")]
        public string scene_path { get; set; }
    }

    // LOAD SCENE
    public class LoadSceneRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        [SchemaDocumentation(
            "Name of the scene to load.",
            Notes = "One way to identify the scene. If not provided, use 'path' or 'build_index'."
        )]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        [SchemaDocumentation(
            "Relative path (from Assets) to the scene file.",
            Notes = "One way to identify the scene. If not provided, use 'name' or 'build_index'."
        )]
        public string path { get; set; }

        [Newtonsoft.Json.JsonProperty("build_index")]
        [SchemaDocumentation(
            "Build index of the scene to load.",
            Notes = "Alternative way to identify the scene. If not provided, use 'name' or 'path'."
        )]
        public int? build_index { get; set; }
    }

    public class LoadSceneResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("scene_path")]
        public string scene_path { get; set; }

        [Newtonsoft.Json.JsonProperty("scene_name")]
        public string scene_name { get; set; }

        [Newtonsoft.Json.JsonProperty("build_index")]
        public int? build_index { get; set; }
    }

    // SAVE SCENE
    public class SaveSceneRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        [SchemaDocumentation(
            "Name to use when saving the scene.",
            Notes = "Required if saving an untitled scene or using 'Save As'."
        )]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        [SchemaDocumentation(
            "Optional directory (relative to Assets) where the scene will be saved.",
            Notes = "If not specified, saves to the current scene's path."
        )]
        public string path { get; set; }
    }

    public class SaveSceneResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("scene_path")]
        public string scene_path { get; set; }

        [Newtonsoft.Json.JsonProperty("scene_name")]
        public string scene_name { get; set; }
    }

    // GET ACTIVE SCENE
    public class GetActiveSceneRequest : BaseActionRequest
    {
        // No fields required
    }

    public class GetActiveSceneResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }

        [Newtonsoft.Json.JsonProperty("build_index")]
        public int build_index { get; set; }

        [Newtonsoft.Json.JsonProperty("is_loaded")]
        public bool is_loaded { get; set; }

        [Newtonsoft.Json.JsonProperty("is_dirty")]
        public bool is_dirty { get; set; }

        [Newtonsoft.Json.JsonProperty("root_count")]
        public int root_count { get; set; }
    }

    // GET SCENE HIERARCHY
    public class GetSceneHierarchyRequest : BaseActionRequest
    {
        // No fields required
    }

    public class SceneHierarchyNode
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("active_self")]
        public bool active_self { get; set; }

        [Newtonsoft.Json.JsonProperty("active_in_hierarchy")]
        public bool active_in_hierarchy { get; set; }

        [Newtonsoft.Json.JsonProperty("tag")]
        public string tag { get; set; }

        [Newtonsoft.Json.JsonProperty("layer")]
        public int layer { get; set; }

        [Newtonsoft.Json.JsonProperty("is_static")]
        public bool is_static { get; set; }

        [Newtonsoft.Json.JsonProperty("instance_id")]
        public int instance_id { get; set; }

        [Newtonsoft.Json.JsonProperty("transform")]
        public TransformData transform { get; set; }

        [Newtonsoft.Json.JsonProperty("children")]
        public List<SceneHierarchyNode> children { get; set; }
    }



    public class GetSceneHierarchyResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("hierarchy")]
        public List<SceneHierarchyNode> hierarchy { get; set; }
    }
}
