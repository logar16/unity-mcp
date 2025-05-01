// Copyright (c) 2025 Logar16. All rights reserved.

using System.Collections.Generic;

namespace UnityMcp.Editor.Models
{
    public class GetPrefabInfoRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("detail_level")]
        [SchemaDocumentation("Level of detail to return.", Notes = "Allowed values: \"summary\", \"detailed\", \"component_details\".")]
        public string detail_level { get; set; }

        [Newtonsoft.Json.JsonProperty("child_path")]
        [SchemaDocumentation("Path to a child GameObject within the prefab (e.g., \"Root/Child/Gun\"). Required for \"component_details\".")]
        public string child_path { get; set; }

        [Newtonsoft.Json.JsonProperty("component_type")]
        [SchemaDocumentation("Fully qualified type name or name of the component (e.g., \"UnityEngine.Rigidbody\"). Required for \"component_details\".")]
        public string component_type { get; set; }
    }

    public class PrefabInfoSummary
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        public string prefab_path { get; set; }
        [Newtonsoft.Json.JsonProperty("root_name")]
        public string root_name { get; set; }
        [Newtonsoft.Json.JsonProperty("root_tag")]
        public string root_tag { get; set; }
        [Newtonsoft.Json.JsonProperty("root_layer")]
        public int root_layer { get; set; }
        [Newtonsoft.Json.JsonProperty("root_component_count")]
        public int root_component_count { get; set; }
    }

    public class PrefabHierarchyNode
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }
        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }
        [Newtonsoft.Json.JsonProperty("tag")]
        public string tag { get; set; }
        [Newtonsoft.Json.JsonProperty("layer")]
        public int layer { get; set; }
        [Newtonsoft.Json.JsonProperty("component_types")]
        public List<string> component_types { get; set; }
        [Newtonsoft.Json.JsonProperty("children")]
        public List<PrefabHierarchyNode> children { get; set; }
    }

    public class PrefabInfoDetailed : PrefabInfoSummary
    {
        [Newtonsoft.Json.JsonProperty("hierarchy")]
        public PrefabHierarchyNode hierarchy { get; set; }
    }

    public class PrefabComponentDetails
    {
        [Newtonsoft.Json.JsonProperty("child_path")]
        public string child_path { get; set; }
        [Newtonsoft.Json.JsonProperty("type_name")]
        public string type_name { get; set; }
        [Newtonsoft.Json.JsonProperty("properties")]
        public Dictionary<string, object> properties { get; set; }
    }

    public class GetPrefabInfoResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("summary")]
        public PrefabInfoSummary summary { get; set; }
        [Newtonsoft.Json.JsonProperty("detailed")]
        public PrefabInfoDetailed detailed { get; set; }
        [Newtonsoft.Json.JsonProperty("component_details")]
        public PrefabComponentDetails component_details { get; set; }
    }

    // --- CHILD MANIPULATION ---

    public class PrefabChildProperties
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }
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
        public List<string> components_to_add { get; set; }
        [Newtonsoft.Json.JsonProperty("primitive_type")]
        [SchemaDocumentation("Type of Unity primitive to create (if specified).", Notes = "Allowed values: \"Cube\", \"Sphere\", \"Capsule\", \"Cylinder\", \"Plane\", \"Quad\".")]
        public string primitive_type { get; set; }
    }

    public class AddPrefabChildRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("child_properties")]
        [SchemaDocumentation("Properties for the new child GameObject to add.")]
        public PrefabChildProperties child_properties { get; set; }

        [Newtonsoft.Json.JsonProperty("parent_path")]
        [SchemaDocumentation("Path to the parent GameObject within the prefab (e.g., \"Root/Child\"). If null or empty, the child is added to the root.")]
        public string parent_path { get; set; }
    }

    public class AddPrefabChildResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("added_child_path")]
        public string added_child_path { get; set; }
    }

    public class RemovePrefabChildRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("child_path")]
        [SchemaDocumentation("Path to the child GameObject within the prefab to remove (e.g., \"Root/Child/Gun\").")]
        public string child_path { get; set; }
    }

    public class RemovePrefabChildResponse : BaseActionResponse
    {
    }

    public class RenamePrefabChildRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("child_path")]
        [SchemaDocumentation("Path to the child GameObject within the prefab to rename (e.g., \"Root/Child/Gun\").")]
        public string child_path { get; set; }

        [Newtonsoft.Json.JsonProperty("new_name")]
        [SchemaDocumentation("New name for the child GameObject.")]
        public string new_name { get; set; }
    }

    public class RenamePrefabChildResponse : BaseActionResponse
    {
    }

    public class ModifyPrefabChildRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("child_path")]
        [SchemaDocumentation("Path to the child GameObject within the prefab to modify (e.g., \"Root/Child/Gun\").")]
        public string child_path { get; set; }

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

        [Newtonsoft.Json.JsonProperty("set_active")]
        [SchemaDocumentation("Whether to set the GameObject active (true) or inactive (false).", Notes = "If omitted, the active state is unchanged.")]
        public bool? set_active { get; set; }
    }

    public class ModifyPrefabChildResponse : BaseActionResponse
    {
    }
    // --- COMPONENT MANIPULATION ---

    public class AddPrefabComponentRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("child_path")]
        [SchemaDocumentation("Path to the child GameObject within the prefab (e.g., \"Root/Child/Gun\").")]
        public string child_path { get; set; }

        [Newtonsoft.Json.JsonProperty("component_type")]
        [SchemaDocumentation("Fully qualified type name of the component to add (e.g., \"UnityEngine.Rigidbody\").")]
        public string component_type { get; set; }

        [Newtonsoft.Json.JsonProperty("component_properties")]
        [SchemaDocumentation("Optional dictionary of initial property values to set on the new component.", Notes = "Format: property name (string) -> value (object).")]
        public Dictionary<string, object> component_properties { get; set; }
    }

    public class AddPrefabComponentResponse : BaseActionResponse
    {
    }

    public class RemovePrefabComponentRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("child_path")]
        [SchemaDocumentation("Path to the child GameObject within the prefab (e.g., \"Root/Child/Gun\").")]
        public string child_path { get; set; }

        [Newtonsoft.Json.JsonProperty("component_type")]
        [SchemaDocumentation("Fully qualified type name of the component to remove (e.g., \"UnityEngine.Rigidbody\").")]
        public string component_type { get; set; }
    }

    public class RemovePrefabComponentResponse : BaseActionResponse
    {
    }

    public class ModifyPrefabComponentRequest : BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("prefab_path")]
        [SchemaDocumentation("Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\").")]
        public string prefab_path { get; set; }

        [Newtonsoft.Json.JsonProperty("child_path")]
        [SchemaDocumentation("Path to the child GameObject within the prefab (e.g., \"Root/Child/Gun\").")]
        public string child_path { get; set; }

        [Newtonsoft.Json.JsonProperty("component_type")]
        [SchemaDocumentation("Fully qualified type name of the component to modify (e.g., \"UnityEngine.Rigidbody\").")]
        public string component_type { get; set; }

        [Newtonsoft.Json.JsonProperty("component_properties")]
        [SchemaDocumentation("Dictionary of property names and their new values.", Notes = "Format: property name (string) -> value (object).")]
        public Dictionary<string, object> component_properties { get; set; }
    }

    public class ModifyPrefabComponentResponse : BaseActionResponse
    {
    }
}
