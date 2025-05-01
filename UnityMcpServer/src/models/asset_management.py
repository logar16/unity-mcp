from typing import Any, Literal

from pydantic import Field, ConfigDict
from .common import BaseActionRequest

class AssetActionRequest(BaseActionRequest):
    path: str = Field(..., description="Asset path (relative to Assets/).")

class CreateAssetRequest(AssetActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["create_asset"] = "create_asset"
    asset_type: str = Field(
        ...,
        description="Type of asset to create.",
        json_schema_extra={"x-notes": "Supported values: \"folder\", \"material\", \"scriptableobject\". Determines creation logic."}
    )
    properties: dict[str, Any] = Field(
        ...,
        description="Additional properties for asset creation.",
        json_schema_extra={"x-notes": "For \"material\": key-value pairs for material properties. For \"scriptableobject\": must include \"scriptClass\" (C# type name), plus any fields to set."}
    )

class ModifyAssetRequest(AssetActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["modify_asset"] = "modify_asset"
    properties: dict[str, Any] = Field(
        ...,
        description="Properties to modify on the asset.",
        json_schema_extra={"x-notes": "Key-value pairs for fields to update. For materials: property names/values. For scriptable objects: field names/values."}
    )

class MoveAssetRequest(AssetActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["move_asset"] = "move_asset"
    destination: str = Field(
        ...,
        description="Destination path for the asset.",
        json_schema_extra={"x-notes": "Must be a valid Unity asset path (e.g., \"Assets/NewFolder/Asset.asset\")."}
    )

class DeleteAssetRequest(AssetActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["delete_asset"] = "delete_asset"

class ImportAssetRequest(AssetActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["import_asset"] = "import_asset"

class GetAssetInfoRequest(AssetActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["get_asset_info"] = "get_asset_info"

class SearchAssetsRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["search_assets"] = "search_assets"
    search_pattern: str = Field(
        ...,
        description="Search filter string.",
        json_schema_extra={"x-notes": "Uses Unity's search syntax (e.g., \"t:Prefab MyAsset\"). See AssetDatabase.FindAssets documentation."}
    )
    search_folders: list[str] = Field(
        ...,
        description="Folders to restrict the search to.",
        json_schema_extra={"x-notes": "List of folder paths (e.g., [\"Assets/Prefabs\"]). If null or empty, searches all assets."}
    )

class DuplicateAssetRequest(AssetActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["duplicate_asset"] = "duplicate_asset"
    destination: str = Field(
        ...,
        description="Destination path for the duplicated asset.",
        json_schema_extra={"x-notes": "Must be a valid Unity asset path (e.g., \"Assets/Copy/Asset.asset\")."}
    )
