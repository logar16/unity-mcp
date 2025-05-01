from mcp.server.fastmcp import FastMCP, Context
from uuid import uuid4
from unity_connection import get_unity_connection
from models.editor_control import PlayRequest, PauseRequest, StopRequest, GetStateRequest

def register_manage_editor_tools(mcp: FastMCP):
    """Register editor management tools using Pydantic request models."""

    @mcp.tool(name="play")
    def play(ctx: Context):
        """
        Enters play mode in the Unity Editor.
        """
        req = PlayRequest(id=str(uuid4()))
        return get_unity_connection().send_request(req)

    @mcp.tool(name="pause")
    def pause(ctx: Context):
        """
        Toggles pause/resume in play mode.
        """
        req = PauseRequest(id=str(uuid4()))
        return get_unity_connection().send_request(req)

    @mcp.tool(name="stop")
    def stop(ctx: Context):
        """
        Exits play mode in the Unity Editor.
        """
        req = StopRequest(id=str(uuid4()))
        return get_unity_connection().send_request(req)

    @mcp.tool(name="get_state")
    def get_state(ctx: Context):
        """
        Gets the current Unity Editor state (play, pause, compile, etc).
        """
        req = GetStateRequest(id=str(uuid4()))
        return get_unity_connection().send_request(req)
