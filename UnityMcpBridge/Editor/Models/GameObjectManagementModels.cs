// Copyright (c) 2025 Logar16. All rights reserved.

using System.Collections.Generic;
using UnityMcp.Editor.Core;

namespace UnityMcp.Editor.Models
{
    public class ComponentPropertyData
    {
        [Newtonsoft.Json.JsonProperty("type_name")]
        public string type_name { get; set; }

        [Newtonsoft.Json.JsonProperty("properties")]
        public Dictionary<string, object> properties { get; set; }
    }

    public class GameObjectIdentifier
    {
        [Newtonsoft.Json.JsonProperty("instance_id")]
        public int instance_id { get; set; }

        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }

        [Newtonsoft.Json.JsonProperty("tag")]
        public string tag { get; set; }

        [Newtonsoft.Json.JsonProperty("layer")]
        public int layer { get; set; }

        [Newtonsoft.Json.JsonProperty("active_self")]
        public bool active_self { get; set; }

        [Newtonsoft.Json.JsonProperty("transform")]
        public TransformData transform { get; set; }
    }

    // --- CREATE ---
    public class CreateGameObjectRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("parent")]
        [SchemaDocumentation("Parent GameObject. Can be specified by name, hierarchy path, or instance ID.")]
        public string parent { get; set; }

        [Newtonsoft.Json.JsonProperty("position")]
        public Vector3Data position { get; set; }

        [Newtonsoft.Json.JsonProperty("rotation")]
        public Vector3Data rotation { get; set; }

        [Newtonsoft.Json.JsonProperty("scale")]
        public Vector3Data scale { get; set; }

        [Newtonsoft.Json.JsonProperty("tag")]
        public string tag { get; set; }

        [Newtonsoft.Json.JsonProperty("layer")]
        public string layer { get; set; }

        [Newtonsoft.Json.JsonProperty("components_to_add")]
        [SchemaDocumentation("List of component type names to add to the GameObject.", Notes = "Each entry should be a fully qualified type name or short type name.")]
        public List<string> components_to_add { get; set; }

        [Newtonsoft.Json.JsonProperty("primitive_type")]
        [SchemaDocumentation("Primitive type to create (optional).", Notes = "Allowed values: Cube, Sphere, Capsule, Cylinder, Plane, Quad.")]
        public string primitive_type { get; set; }
    }

    public class CreateGameObjectResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("created_object")]
        public GameObjectIdentifier created_object { get; set; }
    }

    // --- FIND ---
    public class FindGameObjectRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("tag")]
        public string tag { get; set; }

        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }

        [Newtonsoft.Json.JsonProperty("find_all")]
        [SchemaDocumentation("Whether to return all matching GameObjects or only the first match.", Notes = "If true, returns all matches; if false, returns only the first match.")]
        public bool find_all { get; set; }

        [Newtonsoft.Json.JsonProperty("search_inactive")]
        [SchemaDocumentation("Whether to include inactive GameObjects in the search.", Notes = "If true, includes inactive objects; if false, only active objects are considered.")]
        public bool search_inactive { get; set; }
    }

    public class FindGameObjectResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("found_objects")]
        public List<GameObjectIdentifier> found_objects { get; set; }
    }

    // --- MODIFY ---
    /// <summary>
    /// Request to modify properties, transform, and components of a GameObject.
    /// </summary>
    public class ModifyGameObjectRequest : BaseActionRequest
    {
        /// <summary>Target GameObject identifier.</summary>
        [Newtonsoft.Json.JsonProperty("target")]
        [SchemaDocumentation("Target GameObject. Can be specified by name, hierarchy path, or instance ID.")]
        public string target { get; set; }

        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("tag")]
        public string tag { get; set; }

        [Newtonsoft.Json.JsonProperty("layer")]
        public string layer { get; set; }

        [Newtonsoft.Json.JsonProperty("parent")]
        [SchemaDocumentation("Parent GameObject. Can be specified by name, hierarchy path, or instance ID.")]
        public string parent { get; set; }

        [Newtonsoft.Json.JsonProperty("position")]
        public Vector3Data position { get; set; }

        [Newtonsoft.Json.JsonProperty("rotation")]
        public Vector3Data rotation { get; set; }

        [Newtonsoft.Json.JsonProperty("scale")]
        public Vector3Data scale { get; set; }

        [Newtonsoft.Json.JsonProperty("set_active")]
        [SchemaDocumentation("Whether to set the GameObject active or inactive.", Notes = "If true, sets the GameObject active; if false, sets it inactive.")]
        public bool? set_active { get; set; }

        [Newtonsoft.Json.JsonProperty("components_to_add")]
        [SchemaDocumentation("List of component type names to add to the GameObject.", Notes = "Each entry should be a fully qualified type name or short type name.")]
        public List<string> components_to_add { get; set; }

        [Newtonsoft.Json.JsonProperty("components_to_remove")]
        [SchemaDocumentation("List of component type names to remove from the GameObject.", Notes = "Each entry should be a fully qualified type name or short type name.")]
        public List<string> components_to_remove { get; set; }

        [Newtonsoft.Json.JsonProperty("component_properties")]
        [SchemaDocumentation("Properties to set on components, grouped by component type.", Notes = "Dictionary format: { component_type: { property_name: value } }")]
        public Dictionary<string, Dictionary<string, object>> component_properties { get; set; }
    }

    public class ModifyGameObjectResponse : BaseActionResponse
    {
    }

    // --- DELETE ---
    public class DeleteGameObjectRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("target")]
        [SchemaDocumentation("Target GameObject. Can be specified by name, hierarchy path, or instance ID.")]
        public string target { get; set; }
    }

    public class DeleteGameObjectResponse : BaseActionResponse
    {
    }
    // --- GET INFO ---
    public class GetGameObjectInfoRequest : BaseActionRequest
    {
        /// <summary>
        /// Target GameObject identifier (name, path, or instance ID as string).
        /// </summary>
        [Newtonsoft.Json.JsonProperty("target")]
        [SchemaDocumentation("Target GameObject. Can be specified by name, hierarchy path, or instance ID.")]
        public string target { get; set; }

        /// <summary>
        /// Level of detail: "summary", "detailed", or "component_details".
        /// </summary>
        [Newtonsoft.Json.JsonProperty("detail_level")]
        [SchemaDocumentation("Level of detail for the response.", Notes = "Allowed values: \"summary\", \"detailed\", \"component_details\".")]
        public string detail_level { get; set; }

        /// <summary>
        /// Component type/name for "component_details" level (optional).
        /// </summary>
        [Newtonsoft.Json.JsonProperty("component_type")]
        [SchemaDocumentation("Component type name for use with detail_level=\"component_details\".", Notes = "Required if detail_level is \"component_details\".")]
        public string component_type { get; set; }
    }

    public class GameObjectInfoSummary
    {
        [Newtonsoft.Json.JsonProperty("instance_id")]
        public int instance_id { get; set; }

        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }

        [Newtonsoft.Json.JsonProperty("tag")]
        public string tag { get; set; }

        [Newtonsoft.Json.JsonProperty("layer")]
        public int layer { get; set; }

        [Newtonsoft.Json.JsonProperty("active_self")]
        public bool active_self { get; set; }

        [Newtonsoft.Json.JsonProperty("parent_instance_id")]
        public int? parent_instance_id { get; set; }

        [Newtonsoft.Json.JsonProperty("parent_name")]
        public string parent_name { get; set; }

        [Newtonsoft.Json.JsonProperty("child_count")]
        public int child_count { get; set; }
    }

    public class GameObjectInfoDetailed : GameObjectInfoSummary
    {
        [Newtonsoft.Json.JsonProperty("local_transform")]
        public TransformData local_transform { get; set; }

        [Newtonsoft.Json.JsonProperty("world_transform")]
        public TransformData world_transform { get; set; }

        [Newtonsoft.Json.JsonProperty("component_types")]
        public List<string> component_types { get; set; }
    }

    public class GameObjectComponentDetails
    {
        [Newtonsoft.Json.JsonProperty("type_name")]
        public string type_name { get; set; }

        [Newtonsoft.Json.JsonProperty("properties")]
        public Dictionary<string, object> properties { get; set; }
    }

    public class GetGameObjectInfoResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("summary")]
        public GameObjectInfoSummary summary { get; set; }

        [Newtonsoft.Json.JsonProperty("detailed")]
        public GameObjectInfoDetailed detailed { get; set; }

        [Newtonsoft.Json.JsonProperty("component_details")]
        public GameObjectComponentDetails component_details { get; set; }
    }
}
