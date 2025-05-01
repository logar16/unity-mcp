from typing import Any, Annotated, Optional
from pydantic import Field
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
from models.schema import GetActionSchemasRequest
import uuid

def register_get_action_schemas_tool(mcp: FastMCP):
    """Registers the get_action_schemas tool with the MCP server."""

    @mcp.tool(name="get_action_schemas")
    async def get_action_schemas(
        ctx: Context,
        action_names: Annotated[
            list[str] | None,
            Field(description="Optional list of action names to retrieve schemas for. If null or empty, returns all. Example: [\"create_gameobject\", \"load_scene\"]")
        ] = None,
    ) -> dict[str, Any]:
        """
        Gets schemas for Unity Editor actions.
        """
        request = GetActionSchemasRequest(
            action_names=action_names,
        )
        unity_conn = get_unity_connection()
        response = unity_conn.send_request(request)
        return response
