# Adding or Updating Unity MCP Server Actions

This document outlines the standard procedure for adding new actions or updating existing ones in the Unity MCP Server based on the `schema.json`.

1. **Locate the Schema**: The source of truth for action definitions is `UnityMcpServer/src/models/schema.json`. Refer to this file for action names, descriptions, notes, and expected input/output models.

2. **Check for Existing Action**: Determine if the action you need to implement already exists in the codebase or if it's entirely new.

3. **Verify/Update Models**:
    * Check the corresponding Pydantic models defined in `UnityMcpServer/src/models/`. Ensure the model accurately reflects the input parameters defined for the action in `schema.json`.
    * Update the model if necessary, adding or modifying fields to match the schema. Use modern Python typing (e.g., `str | None` instead of `Optional[str]`, `list` instead of `List`).

4. **Locate/Create Tool Module**:
    * Find the relevant tool module under `UnityMcpServer/src/tools/` (e.g., `manage_prefab.py` for prefab-related actions).
    * If a suitable module doesn't exist for the action's category, create a new Python file for it.

5. **Implement and Register the Action**:
    * Ensure there's a registration function in the tool module (e.g., `def register_manage_prefab_tools(mcp: FastMCP):`).
    * Define a Python function for the action.
    * Decorate the function with `@mcp.tool(name="ActionNameFromJsonSchema")`. The `name` should match the action name in `schema.json`.
    * The function signature should accept parameters corresponding to the action's input model.
    * The function body should typically call `UnityConnect.send_request()` using the connection instance obtained via `from unity_connection import get_unity_connection`. Pass the action name and arguments derived from the function parameters.
    * Return the result from `send_request()`.

6. **Annotate Parameters**:
    * Use `typing.Annotated` and (`pydatnic.Field`) to add descriptions to the function parameters based on the `description` and `notes` fields from the `schema.json` for the corresponding action input properties.
    * Example: `param_name: Annotated[str, Field(description="Description from schema")]`
    * Only add annotations for fields that have descriptions or notes in the schema. Skip obvious fields if no description is provided.

7. **Modern Typing**: Ensure all type hints use modern Python syntax (e.g., `|` for unions (`type | None` for optional), lowercase `list`, `dict`).

8. **Code Quality**: Refactor to avoid code duplication, especially among similar actions within the same module. Ensure the code is clean and follows project conventions.
