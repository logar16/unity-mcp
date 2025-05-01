"""
Defines the read_console tool for accessing Unity Editor console messages using Pydantic models and UnityConnection.
"""

from typing import List, Optional
from uuid import uuid4
from pydantic import Field
from mcp.server.fastmcp import FastMCP, Context
from unity_connection import get_unity_connection
from models.console_log import GetConsoleLogsRequest, ClearConsoleRequest


def register_read_console_tools(mcp: FastMCP):
    """Registers the read_console tools with the MCP server."""

    from typing import Annotated
    from models.console_log import CountValue

    @mcp.tool(name="get_console_logs")
    def get_console_logs(
        ctx: Context,
        types: Annotated[
            list[str] | None,
            Field(
                description="Filters the returned log entries by log type. Allowed values: 'error', 'warning', 'log'."
            ),
        ] = None,
        filter_text: Annotated[
            str | None, Field(description="Filters the returned log entries by substring match in the message.")
        ] = None,
        count: Annotated[
            CountValue | None, Field(description="Limits the maximum number of log entries returned.")
        ] = None,
    ) -> dict:
        """
        Retrieves Unity Editor console log entries, optionally filtered.
        """
        req = GetConsoleLogsRequest(
            id=str(uuid4()),
            types=types,
            filter_text=filter_text,
            count=count,
        )
        unity = get_unity_connection()
        return unity.send_request(req)

    @mcp.tool(name="clear_console")
    def clear_console(
        ctx: Context,
    ) -> dict:
        """
        Clears all entries from the Unity Editor console.
        """
        req = ClearConsoleRequest(id=str(uuid4()))
        unity = get_unity_connection()
        return unity.send_request(req)
