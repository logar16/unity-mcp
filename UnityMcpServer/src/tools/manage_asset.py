"""
Defines the manage_asset tool for interacting with Unity assets.
"""

from typing import Annotated, Any
from pydantic import Field

from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
from models.asset_management import (
    CreateAssetRequest,
    ModifyAssetRequest,
    MoveAssetRequest,
    DeleteAssetRequest,
    ImportAssetRequest,
    GetAssetInfoRequest,
    DuplicateAssetRequest,
)


def register_manage_asset_tools(mcp: FastMCP):
    """Registers the manage_asset tools with the MCP server."""

    @mcp.tool(name="create_asset")
    async def create_asset(
        ctx: Context,
        path: str,
        asset_type: Annotated[
            str,
            Field(
                description='Type of asset to create. Supported values: "folder", "material", "scriptableobject". Determines creation logic.'
            ),
        ],
        properties: Annotated[
            dict[str, Any],
            Field(
                description='Additional properties for asset creation. For "material": key-value pairs for material properties. For "scriptableobject": must include "scriptClass" (C# type name), plus any fields to set.'
            ),
        ],
    ) -> dict[str, Any]:
        """
        Creates a new asset in Unity.
        """
        req = CreateAssetRequest(
            path=path,
            asset_type=asset_type,
            properties=properties,
        )
        connection = get_unity_connection()
        return connection.send_request(req)

    @mcp.tool(name="search_assets")
    async def search_assets(
        ctx: Context,
        search_pattern: Annotated[
            str,
            Field(
                description='Search filter string. Uses Unity\'s search syntax (e.g., "t:Prefab MyAsset"). See AssetDatabase.FindAssets documentation.'
            ),
        ],
        search_folders: Annotated[
            list[str] | None,
            Field(
                description='Folders to restrict the search to. List of folder paths (e.g., ["Assets/Prefabs"]). If null or empty, searches all assets.'
            ),
        ] = None,
    ) -> dict[str, Any]:
        """
        Searches for assets in Unity using the specified pattern and folders.
        """
        from models.asset_management import SearchAssetsRequest

        req = SearchAssetsRequest(
            search_pattern=search_pattern,
            search_folders=search_folders or [],
        )
        connection = get_unity_connection()
        return connection.send_request(req)

    @mcp.tool(name="modify_asset")
    async def modify_asset(
        ctx: Context,
        path: str,
        properties: Annotated[
            dict[str, Any],
            Field(
                description="Properties to modify on the asset. Key-value pairs for fields to update. For materials: property names/values. For scriptable objects: field names/values."
            ),
        ],
    ) -> dict[str, Any]:
        """
        Modifies an existing asset in Unity.
        """
        req = ModifyAssetRequest(
            path=path,
            properties=properties,
        )
        connection = get_unity_connection()
        return connection.send_request(req)

    @mcp.tool(name="move_asset")
    async def move_asset(
        ctx: Context,
        path: str,
        destination: Annotated[
            str,
            Field(
                description='Destination path for the asset. Must be a valid Unity asset path (e.g., "Assets/NewFolder/Asset.asset").'
            ),
        ],
    ) -> dict[str, Any]:
        """
        Moves an asset to a new location in Unity.
        """
        req = MoveAssetRequest(
            path=path,
            destination=destination,
        )
        connection = get_unity_connection()
        return connection.send_request(req)

    @mcp.tool(name="delete_asset")
    async def delete_asset(
        ctx: Context,
        path: str,
    ) -> dict[str, Any]:
        """
        Deletes an asset in Unity.
        """
        req = DeleteAssetRequest(
            path=path,
        )
        connection = get_unity_connection()
        return connection.send_request(req)

    @mcp.tool(name="import_asset")
    async def import_asset(
        ctx: Context,
        path: str,
    ) -> dict[str, Any]:
        """
        Imports an asset into Unity.
        """
        req = ImportAssetRequest(
            path=path,
        )
        connection = get_unity_connection()
        return connection.send_request(req)

    @mcp.tool(name="get_asset_info")
    async def get_asset_info(
        ctx: Context,
        path: str,
    ) -> dict[str, Any]:
        """
        Retrieves information about an asset in Unity.
        """
        req = GetAssetInfoRequest(
            path=path,
        )
        connection = get_unity_connection()
        return connection.send_request(req)

    @mcp.tool(name="duplicate_asset")
    async def duplicate_asset(
        ctx: Context,
        path: str,
        destination: Annotated[
            str,
            Field(
                description='Destination path for the duplicated asset. Must be a valid Unity asset path (e.g., "Assets/Copy/Asset.asset").'
            ),
        ],
    ) -> dict[str, Any]:
        """
        Duplicates an asset in Unity.
        """
        req = DuplicateAssetRequest(
            path=path,
            destination=destination,
        )
        connection = get_unity_connection()
        return connection.send_request(req)
