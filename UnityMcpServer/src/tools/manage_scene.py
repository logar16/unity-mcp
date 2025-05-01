from typing import Annotated
from pydantic import Field
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
from models.scene_management import (
    GetSceneHierarchyInput,
    GetActiveSceneInput,
    LoadSceneInput,
    CreateSceneInput,
    SaveSceneInput,
)


def register_manage_scene_tools(mcp: FastMCP):
    """Register all scene management tools with the MCP server."""

    @mcp.tool(name="get_scene_hierarchy")
    def get_scene_hierarchy(
        ctx: Context,
    ) -> dict:
        req = GetSceneHierarchyInput(action="get_scene_hierarchy")
        return get_unity_connection().send_request(req)

    @mcp.tool(name="get_active_scene")
    def get_active_scene(
        ctx: Context,
    ) -> dict:
        req = GetActiveSceneInput(action="get_active_scene")
        return get_unity_connection().send_request(req)

    @mcp.tool(name="load_scene")
    def load_scene(
        ctx: Context,
        name: Annotated[
            str | None,
            Field(
                description="Name of the scene to load. One way to identify the scene. If not provided, use 'path' or 'build_index'."
            ),
        ] = None,
        path: Annotated[
            str | None,
            Field(
                description="Relative path (from Assets) to the scene file. One way to identify the scene. If not provided, use 'name' or 'build_index'."
            ),
        ] = None,
        build_index: Annotated[
            dict | None,
            Field(
                description="Build index of the scene to load. Alternative way to identify the scene. If not provided, use 'name' or 'path'."
            ),
        ] = None,
    ) -> dict:
        req = LoadSceneInput(
            action="load_scene",
            name=name,
            path=path,
            build_index=build_index,
        )
        return get_unity_connection().send_request(req)

    @mcp.tool(name="create_scene")
    def create_scene(
        ctx: Context,
        name: str | None = None,
        path: Annotated[
            str | None,
            Field(
                description="Optional directory (relative to Assets) where the scene will be created. Defaults to 'Assets/Scenes' if not specified."
            ),
        ] = None,
    ) -> dict:
        req = CreateSceneInput(
            action="create_scene",
            name=name,
            path=path,
        )
        return get_unity_connection().send_request(req)

    @mcp.tool(name="save_scene")
    def save_scene(
        ctx: Context,
        name: Annotated[
            str | None,
            Field(
                description="Name to use when saving the scene. Required if saving an untitled scene or using 'Save As'."
            ),
        ] = None,
        path: Annotated[
            str | None,
            Field(
                description="Optional directory (relative to Assets) where the scene will be saved. If not specified, saves to the current scene's path."
            ),
        ] = None,
    ) -> dict:
        req = SaveSceneInput(
            action="save_scene",
            name=name,
            path=path,
        )
        return get_unity_connection().send_request(req)
