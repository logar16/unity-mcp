import uuid
from typing import Dict, Any, Optional, Annotated
from pydantic import Field
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
from models.prefab_management import (
    AddPrefabChildRequest,
    RemovePrefabChildRequest,
    RenamePrefabChildRequest,
    ModifyPrefabChildRequest,
    AddPrefabComponentRequest,
    RemovePrefabComponentRequest,
    ModifyPrefabComponentRequest,
    PrefabChildProperties,
)
from models.common import Vector3Data


def register_manage_prefab_tools(mcp: FastMCP):
    """Register all prefab management tools with the MCP server."""

    @mcp.tool(name="add_prefab_child")
    def add_prefab_child(
        ctx: Context,
        prefab_path: Annotated[
            str,
            Field(
                description='Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
            ),
        ],
        child_properties: Annotated[dict, Field(description="Properties for the new child GameObject to add.")],
        parent_path: Annotated[
            str | None,
            Field(
                description='Path to the parent GameObject within the prefab (e.g., "Root/Child"). If null or empty, the child is added to the root.'
            ),
        ] = None,
    ) -> dict:
        """
        Adds a new child GameObject to a prefab.
        """
        from models.prefab_management import PrefabChildProperties

        child_props = PrefabChildProperties(**child_properties)
        request = AddPrefabChildRequest(
            prefab_path=prefab_path,
            child_properties=child_props,
            parent_path=parent_path,
        )
        conn = get_unity_connection()
        return conn.send_request(request)

    @mcp.tool(name="remove_prefab_child")
    def remove_prefab_child(
        ctx: Context,
        prefab_path: Annotated[
            str,
            Field(
                description='Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
            ),
        ],
        child_path: Annotated[
            str, Field(description='Path to the child GameObject within the prefab to remove (e.g., "Root/Child/Gun").')
        ],
    ) -> dict:
        """
        Removes a child GameObject from a prefab.
        """
        request = RemovePrefabChildRequest(
            prefab_path=prefab_path,
            child_path=child_path,
        )
        conn = get_unity_connection()
        return conn.send_request(request)

    @mcp.tool(name="rename_prefab_child")
    def rename_prefab_child(
        ctx: Context,
        prefab_path: Annotated[
            str,
            Field(
                description='Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
            ),
        ],
        child_path: Annotated[
            str, Field(description='Path to the child GameObject within the prefab to rename (e.g., "Root/Child/Gun").')
        ],
        new_name: Annotated[str, Field(description="New name for the child GameObject.")],
    ) -> dict:
        """
        Renames a child GameObject in a prefab.
        """
        request = RenamePrefabChildRequest(
            prefab_path=prefab_path,
            child_path=child_path,
            new_name=new_name,
        )
        conn = get_unity_connection()
        return conn.send_request(request)

    @mcp.tool(name="modify_prefab_child")
    def modify_prefab_child(
        ctx: Context,
        prefab_path: Annotated[
            str,
            Field(
                description='Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
            ),
        ],
        child_path: Annotated[
            str, Field(description='Path to the child GameObject within the prefab to modify (e.g., "Root/Child/Gun").')
        ],
        position: dict[str, float] | None = None,
        rotation: dict[str, float] | None = None,
        scale: dict[str, float] | None = None,
        tag: str | None = None,
        layer: str | None = None,
        set_active: Annotated[
            dict | None,
            Field(
                description="Whether to set the GameObject active (true) or inactive (false). Notes: If omitted, the active state is unchanged."
            ),
        ] = None,
    ) -> dict:
        """
        Modifies properties of a child GameObject in a prefab.
        """
        from models.prefab_management import SetActiveData

        request = ModifyPrefabChildRequest(
            prefab_path=prefab_path,
            child_path=child_path,
            position=Vector3Data(**position) if position else None,
            rotation=Vector3Data(**rotation) if rotation else None,
            scale=Vector3Data(**scale) if scale else None,
            tag=tag,
            layer=layer,
            set_active=SetActiveData(**set_active) if set_active else None,
        )
        conn = get_unity_connection()
        return conn.send_request(request)

    @mcp.tool()
    def add_prefab_component(
        ctx: Context,
        prefab_path: Annotated[
            str,
            Field(
                description='Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
            ),
        ],
        child_path: Annotated[
            str, Field(description='Path to the child GameObject within the prefab (e.g., "Root/Child/Gun").')
        ],
        component_type: Annotated[
            str, Field(description='Fully qualified type name of the component to add (e.g., "UnityEngine.Rigidbody").')
        ],
        component_properties: Annotated[
            Optional[Dict[str, Any]],
            Field(
                description="Optional dictionary of initial property values to set on the new component. Notes: Format: property name (string) -> value (object)."
            ),
        ] = None,
    ) -> Dict[str, Any]:
        """
        Adds a component to a child GameObject in a prefab.
        """
        request = AddPrefabComponentRequest(
            prefab_path=prefab_path,
            child_path=child_path,
            component_type=component_type,
            component_properties=component_properties,
        )
        conn = get_unity_connection()
        return conn.send_request(request)

    @mcp.tool()
    def remove_prefab_component(
        ctx: Context,
        prefab_path: Annotated[
            str,
            Field(
                description='Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
            ),
        ],
        child_path: Annotated[
            str, Field(description='Path to the child GameObject within the prefab (e.g., "Root/Child/Gun").')
        ],
        component_type: Annotated[
            str,
            Field(description='Fully qualified type name of the component to remove (e.g., "UnityEngine.Rigidbody").'),
        ],
    ) -> Dict[str, Any]:
        """
        Removes a component from a child GameObject in a prefab.
        """
        request = RemovePrefabComponentRequest(
            prefab_path=prefab_path,
            child_path=child_path,
            component_type=component_type,
        )
        conn = get_unity_connection()
        return conn.send_request(request)

    @mcp.tool()
    def modify_prefab_component(
        ctx: Context,
        prefab_path: Annotated[
            str,
            Field(
                description='Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
            ),
        ],
        child_path: Annotated[
            str, Field(description='Path to the child GameObject within the prefab (e.g., "Root/Child/Gun").')
        ],
        component_type: Annotated[
            str,
            Field(description='Fully qualified type name of the component to modify (e.g., "UnityEngine.Rigidbody").'),
        ],
        component_properties: Annotated[
            Dict[str, Any],
            Field(
                description="Dictionary of property names and their new values. Notes: Format: property name (string) -> value (object)."
            ),
        ],
    ) -> Dict[str, Any]:
        """
        Modifies properties of a component on a child GameObject in a prefab.
        """
        request = ModifyPrefabComponentRequest(
            prefab_path=prefab_path,
            child_path=child_path,
            component_type=component_type,
            component_properties=component_properties,
        )
        conn = get_unity_connection()
        return conn.send_request(request)
