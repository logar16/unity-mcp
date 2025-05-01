"""
Defines the execute_menu_item tool for running Unity Editor menu commands.
"""

from typing import Annotated, Any
from pydantic import Field
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
from models.common import ExecuteMenuItemRequest


def register_execute_menu_item_tools(mcp: FastMCP):
    """Registers the execute_menu_item tool with the MCP server."""

    @mcp.tool(name="execute_menu_item")
    async def execute_menu_item(
        ctx: Context,
        menu_path: Annotated[
            str,
            Field(description="The full path of the Unity Editor menu item to execute. Example: 'File/Save Project'"),
        ],
        action: str | None = None,
        id: str | None = None,
    ) -> dict:
        """
        Executes a Unity Editor menu item by its full path.
        """
        unity_conn = get_unity_connection()
        request = ExecuteMenuItemRequest(menu_path=menu_path, action=action, id=id)
        return unity_conn.send_request(request)

    # (Retain get_available_menus as-is, unrelated to this update)
    @mcp.tool()
    async def get_available_menus(
        ctx: Context,
    ) -> dict[str, Any]:
        """
        Gets the list of available Unity Editor menu items (currently returns an empty list).
        """
        # There is no Pydantic model for get_available_menus, so send is not supported via send_request.
        # If needed, implement a request model in models/editor_control.py.
        raise NotImplementedError("get_available_menus is not supported with the current request model system.")
