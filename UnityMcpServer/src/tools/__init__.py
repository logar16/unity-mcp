from .get_schemas import register_get_action_schemas_tool
from .manage_prefab import register_manage_prefab_tools
from .manage_editor import register_manage_editor_tools
from .manage_gameobject import register_manage_gameobject_tools
from .manage_asset import register_manage_asset_tools
from .read_console import register_read_console_tools
from .execute_menu_item import register_execute_menu_item_tools

def register_all_tools(mcp):
    """Register all refactored tools with the MCP server."""
    print("Registering Unity MCP Server refactored tools...")
    register_get_action_schemas_tool(mcp)
    register_manage_prefab_tools(mcp)
    register_manage_editor_tools(mcp)
    register_manage_gameobject_tools(mcp)
    register_manage_asset_tools(mcp)
    register_read_console_tools(mcp)
    register_execute_menu_item_tools(mcp)
    print("Unity MCP Server tool registration complete.")
