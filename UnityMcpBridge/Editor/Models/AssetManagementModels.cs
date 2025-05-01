// Copyright (c) Vibraint. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace UnityMcp.Editor.Models
{
    // Base response class (assumed to exist in CommonModels)
    public class AssetActionRequest: BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("path")]
        public string path { get; set; }
    }

    // CREATE ASSET
    public class CreateAssetRequest : AssetActionRequest
    {
        [Newtonsoft.Json.JsonProperty("asset_type")]
        [SchemaDocumentation(
            "Type of asset to create.",
            Notes = "Supported values: \"folder\", \"material\", \"scriptableobject\". Determines creation logic."
        )]
        public string asset_type { get; set; }
        [Newtonsoft.Json.JsonProperty("properties")]
        [SchemaDocumentation(
            "Additional properties for asset creation.",
            Notes = "For \"material\": key-value pairs for material properties. For \"scriptableobject\": must include \"scriptClass\" (C# type name), plus any fields to set."
        )]
        public Dictionary<string, object> properties { get; set; }
    }
    public class CreateAssetResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("asset_path")]
        public string asset_path { get; set; }
        [Newtonsoft.Json.JsonProperty("guid")]
        public string guid { get; set; }
    }

    // MODIFY ASSET
    public class ModifyAssetRequest : AssetActionRequest
    {
        [Newtonsoft.Json.JsonProperty("properties")]
        [SchemaDocumentation(
            "Properties to modify on the asset.",
            Notes = "Key-value pairs for fields to update. For materials: property names/values. For scriptable objects: field names/values."
        )]
        public Dictionary<string, object> properties { get; set; }
    }
    public class ModifyAssetResponse : BaseActionResponse { }

    // MOVE ASSET
    public class MoveAssetRequest : AssetActionRequest
    {
        [Newtonsoft.Json.JsonProperty("destination")]
        [SchemaDocumentation(
            "Destination path for the asset.",
            Notes = "Must be a valid Unity asset path (e.g., \"Assets/NewFolder/Asset.asset\")."
        )]
        public string destination { get; set; }
    }
    public class MoveAssetResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("new_path")]
        public string new_path { get; set; }
        [Newtonsoft.Json.JsonProperty("guid")]
        public string guid { get; set; }
    }

    // DELETE ASSET
    public class DeleteAssetRequest : AssetActionRequest { }
    public class DeleteAssetResponse : BaseActionResponse { }

    // IMPORT ASSET
    public class ImportAssetRequest : AssetActionRequest { }
    public class ImportAssetResponse : BaseActionResponse { }

    // GET ASSET INFO
    public class GetAssetInfoRequest : AssetActionRequest { }
    public class GetAssetInfoResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("asset_type")]
        public string asset_type { get; set; }
        [Newtonsoft.Json.JsonProperty("asset_path")]
        public string asset_path { get; set; }
        [Newtonsoft.Json.JsonProperty("guid")]
        public string guid { get; set; }
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }
    }

    // SEARCH ASSETS
    public class SearchAssetsRequest: BaseActionRequest
    {
        [Newtonsoft.Json.JsonProperty("search_pattern")]
        [SchemaDocumentation(
            "Search filter string.",
            Notes = "Uses Unity's search syntax (e.g., \"t:Prefab MyAsset\"). See AssetDatabase.FindAssets documentation."
        )]
        public string search_pattern { get; set; }
        [Newtonsoft.Json.JsonProperty("search_folders")]
        [SchemaDocumentation(
            "Folders to restrict the search to.",
            Notes = "List of folder paths (e.g., [\"Assets/Prefabs\"]). If null or empty, searches all assets."
        )]
        public List<string> search_folders { get; set; }
    }
    public class SearchAssetsResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("assets")]
        public List<AssetInfo> assets { get; set; }
    }
    public class AssetInfo
    {
        [Newtonsoft.Json.JsonProperty("asset_path")]
        public string asset_path { get; set; }
        [Newtonsoft.Json.JsonProperty("guid")]
        public string guid { get; set; }
        [Newtonsoft.Json.JsonProperty("asset_type")]
        public string asset_type { get; set; }
        [Newtonsoft.Json.JsonProperty("name")]
        public string name { get; set; }
    }

    // DUPLICATE ASSET
    public class DuplicateAssetRequest : AssetActionRequest
    {
        [Newtonsoft.Json.JsonProperty("destination")]
        [SchemaDocumentation(
            "Destination path for the duplicated asset.",
            Notes = "Must be a valid Unity asset path (e.g., \"Assets/Copy/Asset.asset\")."
        )]
        public string destination { get; set; }
    }
    public class DuplicateAssetResponse : BaseActionResponse
    {
        [Newtonsoft.Json.JsonProperty("new_path")]
        public string new_path { get; set; }
        [Newtonsoft.Json.JsonProperty("guid")]
        public string guid { get; set; }
    }
}
