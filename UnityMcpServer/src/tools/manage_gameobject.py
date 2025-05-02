from typing import Annotated
from pydantic import Field
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
from models.game_object_management import (
    CreateGameObjectRequest,
    ModifyGameObjectRequest,
    DeleteGameObjectRequest,
    FindGameObjectRequest,
    GetGameObjectDetailsRequest,
)


def register_manage_gameobject_tools(mcp: FastMCP):
    """Register all GameObject management tools with the MCP server."""

    @mcp.tool(name="create_gameobject")
    def create_gameobject(
        ctx: Context,
        name: str,
        parent: Annotated[
            str | None,
            Field(description="Parent GameObject. Can be specified by name, hierarchy path, or instance ID."),
        ] = None,
        position: dict | None = None,
        rotation: dict | None = None,
        scale: dict | None = None,
        tag: str | None = None,
        layer: str | None = None,
        components_to_add: Annotated[
            list[str] | None,
            Field(
                description="List of component type names to add to the GameObject. Each entry should be a fully qualified type name or short type name."
            ),
        ] = None,
        primitive_type: Annotated[
            str | None,
            Field(
                description="Primitive type to create (optional). Allowed values: Cube, Sphere, Capsule, Cylinder, Plane, Quad."
            ),
        ] = None,
    ) -> dict:
        """Creates a new GameObject in the Unity scene."""
        unity_connection = get_unity_connection()
        request = CreateGameObjectRequest(
            name=name,
            parent=parent,
            position=position,
            rotation=rotation,
            scale=scale,
            tag=tag,
            layer=layer,
            components_to_add=components_to_add,
            primitive_type=primitive_type,
        )
        return unity_connection.send_request(request)

    @mcp.tool(name="modify_gameobject")
    def modify_gameobject(
        ctx: Context,
        target: Annotated[
            str, Field(description="Target GameObject. Can be specified by name, hierarchy path, or instance ID.")
        ],
        name: str | None = None,
        tag: str | None = None,
        layer: str | None = None,
        parent: Annotated[
            str | None,
            Field(description="Parent GameObject. Can be specified by name, hierarchy path, or instance ID."),
        ] = None,
        position: dict | None = None,
        rotation: dict | None = None,
        scale: dict | None = None,
        set_active: Annotated[
            dict | None,
            Field(
                description="Whether to set the GameObject active or inactive. If true, sets the GameObject active; if false, sets it inactive."
            ),
        ] = None,
        components_to_add: Annotated[
            list[str] | None,
            Field(
                description="List of component type names to add to the GameObject. Each entry should be a fully qualified type name or short type name."
            ),
        ] = None,
        components_to_remove: Annotated[
            list[str] | None,
            Field(
                description="List of component type names to remove from the GameObject. Each entry should be a fully qualified type name or short type name."
            ),
        ] = None,
        component_properties: Annotated[
            dict | None,
            Field(
                description="Properties to set on components, grouped by component type. Dictionary format: { component_type: { property_name: value } }"
            ),
        ] = None,
    ) -> dict:
        """Modifies properties and components of an existing GameObject."""
        unity_connection = get_unity_connection()
        request = ModifyGameObjectRequest(
            target=target,
            name=name,
            tag=tag,
            layer=layer,
            parent=parent,
            position=position,
            rotation=rotation,
            scale=scale,
            set_active=set_active,
            components_to_add=components_to_add,
            components_to_remove=components_to_remove,
            component_properties=component_properties,
        )
        return unity_connection.send_request(request)

    @mcp.tool(name="delete_gameobject")
    def delete_gameobject(
        ctx: Context,
        target: Annotated[
            str, Field(description="Target GameObject. Can be specified by name, hierarchy path, or instance ID.")
        ],
    ) -> dict:
        """Deletes a GameObject from the Unity scene."""
        unity_connection = get_unity_connection()
        request = DeleteGameObjectRequest(
            target=target,
        )
        return unity_connection.send_request(request)

    @mcp.tool(name="find_gameobject")
    def find_gameobject(
        ctx: Context,
        name: str | None = None,
        tag: str | None = None,
        path: str | None = None,
        find_all: Annotated[
            bool,
            Field(
                description="Whether to return all matching GameObjects or only the first match. If true, returns all matches; if false, returns only the first match. Defaults to false."
            ),
        ] = False,
        search_inactive: Annotated[
            bool,
            Field(
                description="Whether to include inactive GameObjects in the search. If true, includes inactive objects; if false, only active objects are considered. Defaults to false."
            ),
        ] = False,
    ) -> dict:
        """Finds GameObjects in the Unity scene by name, tag, or path."""
        unity_connection = get_unity_connection()
        request = FindGameObjectRequest(
            name=name,
            tag=tag,
            path=path,
            find_all=find_all,
            search_inactive=search_inactive,
        )
        return unity_connection.send_request(request)

    @mcp.tool(name="get_gameobject_details")
    def get_gameobject_details(
        ctx: Context,
        target: Annotated[
            str,
            Field(description="Target GameObject. Can be specified by name, hierarchy path, or instance ID."),
        ],
        include_children: Annotated[
            bool,
            Field(
                description="If true, recursively includes child GameObjects in the response hierarchy. If false, only the target node is returned."
            ),
        ] = True,
        include_component_details: Annotated[
            bool,
            Field(
                description="If true, includes detailed property information for components on the returned GameObject(s)."
            ),
        ] = False,
    ) -> dict:
        """
        Retrieves details about a GameObject in the Unity scene.
        """
        request = GetGameObjectDetailsRequest(
            target=target,
            include_children=include_children,
            include_component_details=include_component_details,
        )
        conn = get_unity_connection()
        return conn.send_request(request)
