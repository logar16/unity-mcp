from mcp.server.fastmcp import FastMCP
from typing import Annotated
from pydantic import Field
from UnityMcpServer.src.unity_connection import get_unity_connection
from UnityMcpServer.src.models.script_management import (
    ReadScriptRequest,
    DeleteScriptRequest,
    CreateScriptRequest,
    UpdateScriptRequest,
)


def register_manage_script_tools(mcp: FastMCP):

    @mcp.tool(name="read_script")
    def read_script(
        name: str,
        path: Annotated[
            str | None,
            Field(
                description="Optional. Relative to the Assets folder. Used to identify the location of the script. If omitted, defaults to 'Scripts'."
            ),
        ] = None,
        id: str | None = None,
    ):
        req = ReadScriptRequest(name=name, path=path, id=id, action="read_script")
        return get_unity_connection().send_request(req)

    @mcp.tool(name="delete_script")
    def delete_script(
        name: str,
        path: Annotated[
            str | None,
            Field(
                description="Optional. Relative to the Assets folder. Used to identify the location of the script. If omitted, defaults to 'Scripts'."
            ),
        ] = None,
        id: str | None = None,
    ):
        req = DeleteScriptRequest(name=name, path=path, id=id, action="delete_script")
        return get_unity_connection().send_request(req)

    @mcp.tool(name="create_script")
    def create_script(
        name: str,
        path: Annotated[
            str | None,
            Field(
                description="Optional. Relative to the Assets folder. Used to identify or specify the location for script creation. If omitted, defaults to 'Scripts'."
            ),
        ] = None,
        contents: Annotated[
            str | None, Field(description="Full script text. If omitted, a default template is generated.")
        ] = None,
        script_type: Annotated[
            str | None,
            Field(
                description="Optional. Determines the script template and base class. Examples: MonoBehaviour, ScriptableObject, EditorWindow."
            ),
        ] = None,
        namespace: Annotated[
            str | None, Field(description="Optional. Wraps the script in a C# namespace. Ignored if not provided.")
        ] = None,
        id: str | None = None,
    ):
        req = CreateScriptRequest(
            name=name,
            path=path,
            contents=contents,
            script_type=script_type,
            namespace=namespace,
            id=id,
            action="create_script",
        )
        return get_unity_connection().send_request(req)

    @mcp.tool(name="update_script")
    def update_script(
        name: str,
        path: Annotated[
            str | None,
            Field(
                description="Optional. Relative to the Assets folder. Used to identify the location of the script. If omitted, defaults to 'Scripts'."
            ),
        ] = None,
        contents: Annotated[str, Field(description="Full script text to overwrite the file.")] = "",
        id: str | None = None,
    ):
        req = UpdateScriptRequest(name=name, path=path, contents=contents, id=id, action="update_script")
        return get_unity_connection().send_request(req)
