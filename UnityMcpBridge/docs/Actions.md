# MCP Actions

This document lists all available actions supported by the Unity MCP bridge, along with a brief description of each. It also describes how to add new actions to the system.

## Action Registration

- Each action handler is registered as a delegate (function) with the `ActionRegistry`.
- This is done using a static constructor and the `[InitializeOnLoad]` attribute, ensuring registration occurs on editor load.
- Example:

  ```csharp
  [InitializeOnLoad]
  public static class ExecuteMenuItemActionHandler
  {
      static ExecuteMenuItemAction()
      {
          ActionRegistry.RegisterAction<MenuItemRequest>(
              "execute_menu_item",
              HandleExecuteMenuItem
          );
      }
      public static BaseActionResponse HandleExecuteMenuItem(MenuItemRequest request) { ... }
  }
  ```

## Adding New Actions

1. **Define DTOs:**
   - Create request and response DTOs (request inherits from `BaseActionRequest` and response must inherit from `BaseActionResponse`). Note that each request should provide an `action` and `id` field and the response should copy those so we can keep things clearly organized. An easy way to do this is to use the `BaseActionResponse.Success()` (or `Fail()`) like so: `return new YourActionResponse { FieldName1 = result, FieldName2 = extra }.Success(request);`. See `unity-mcp\Editor\Models\CommonModels.cs` for details on the base classes.
2. **Implement Handler:**
   - Implement a handler function that takes your request type and returns a response.
3. **Register Handler:**
   - Add `[InitializeOnLoad]` (requires `using UnityEditor`) and a static constructor to register the handler delegate with the `ActionRegistry`.

**Example:**

```csharp
[InitializeOnLoad]
public static class ExecuteMenuItemActionHandler
{
    static ExecuteMenuItemAction()
    {
        ActionRegistry.RegisterAction<MenuItemRequest>(
            "execute_menu_item",
            HandleExecuteMenuItem
        );
    }
    public static BaseActionResponse HandleExecuteMenuItem(MenuItemRequest request) { ... }
    // Should return something like this:
    // return new MenuItemResponse { SomeField = output }.Success(request, "Menu item executed.");
}
```

Refer to the main [Architecture.md](Architecture.md) for architectural details and model documentation standards.

## Available Actions

### Prefab Management

#### get_prefab_details

Retrieves information about a prefab asset, from a summary to a full hierarchy breakdown with component types or all serialized properties.

- **Implementation Notes:**
  - Uses `PrefabUtility.LoadPrefabContents` and `PrefabUtility.UnloadPrefabContents` for safe inspection.
  - Relies on helper methods in `PrefabActionUtility` for summary and property extraction.
  - Does not modify the prefab.

#### add_prefab_child

Adds a new child GameObject to a prefab, optionally under a specific parent in the hierarchy. Supports initial configuration of the new child.

- **Implementation Notes:**
  - Loads the prefab, finds the parent (if specified), and creates the new child.
  - Saves and unloads the prefab after modification.
  - Fails if the parent path does not exist.

#### remove_prefab_child

Removes a child GameObject from a prefab by hierarchy path.

- **Implementation Notes:**
  - Loads the prefab, finds the child by path, destroys it, then saves and unloads the prefab.
  - Uses `PrefabActionUtility.FindChildByPath` for path resolution.
  - Fails if the specified child does not exist.

#### rename_prefab_child

Renames a child GameObject within a prefab.

- **Implementation Notes:**
  - Loads the prefab, finds the child, sets its name, then saves and unloads the prefab.
  - Fails if the child path does not exist.

#### modify_prefab_child

Modifies properties (transform, tag, layer, etc.) of a child GameObject in a prefab. Supports updating multiple properties in one call.

- **Implementation Notes:**
  - Loads the prefab, finds the child, applies property changes, then saves and unloads the prefab.
  - Fails if the child path does not exist.

#### add_prefab_component

Adds a component to a child GameObject in a prefab. Can set initial property values on the new component.

- **Implementation Notes:**
  - Loads the prefab, finds the child, adds the component, then saves and unloads the prefab.
  - Fails if the component type is invalid or the child path does not exist.

#### remove_prefab_component

Removes a component from a child GameObject in a prefab.

- **Implementation Notes:**
  - Loads the prefab, finds the child, removes the component, then saves and unloads the prefab.
  - Fails if the component type is not present or the child path does not exist.

#### modify_prefab_component

Modifies properties of a component on a child GameObject in a prefab. Supports updating multiple properties at once.

- **Implementation Notes:**
  - Loads the prefab, finds the child, modifies the component, then saves and unloads the prefab.
  - For best results, use `get_prefab_details` with `"full"` detail to discover available properties.
  - Fails if the component type is not present or the child path does not exist.

### Asset Management

#### create_asset

Creates a new asset (folder, material, ScriptableObject, or prefab) at the given path. Automatically creates parent directories if needed. For ScriptableObjects, specify the class name. Prefab creation is minimal (empty GameObject).

- **Implementation Notes:**
  - Uses `AssetDatabase.CreateAsset`, `AssetDatabase.CreateFolder`, and `PrefabUtility.SaveAsPrefabAsset`.
  - Material and ScriptableObject properties are set via helper methods.
  - Only the listed asset types are supported.

#### delete_asset

Deletes an asset at the specified path.

- **Implementation Notes:**
  - Uses `AssetDatabase.DeleteAsset`.
  - Fails if the asset does not exist or cannot be deleted.

#### duplicate_asset

Duplicates an asset to a new path.

- **Implementation Notes:**
  - Uses `AssetDatabase.CopyAsset`.
  - Fails if the source asset does not exist or the destination is invalid.

#### get_asset_info

Retrieves information about an asset (type, path, GUID, name). Optionally, can include all visible serialized properties if requested.

- **Implementation Notes:**
  - Uses `AssetDatabase.LoadAssetAtPath` and `SerializedObject` for property extraction.
  - Iterates properties with `GetIterator()` and `NextVisible()`, extracting values by `SerializedPropertyType`.
  - Only visible serialized properties are included in `"full_serialized"` mode.

#### import_asset

Forces reimport of an asset at the specified path.

- **Implementation Notes:**
  - Uses `AssetDatabase.ImportAsset` with `ImportAssetOptions.ForceUpdate`.
  - Fails if the asset does not exist or cannot be imported.

#### modify_asset

Modifies properties of an existing asset (material, ScriptableObject, or some importers).

- **Implementation Notes:**
  - Uses helper methods to apply properties to materials and ScriptableObjects.
  - For other asset types, attempts to use `AssetImporter` to set properties.
  - Only certain asset types support property modification.

#### move_asset

Moves or renames an asset.

- **Implementation Notes:**
  - Uses `AssetDatabase.MoveAsset`.
  - Fails if the destination is invalid or already exists.

#### search_assets

Searches for assets by pattern and folders. Use Unity's search filter syntax (e.g., `t:Prefab`). Useful for finding all assets of a type in a folder.

- **Implementation Notes:**
  - Uses `AssetDatabase.FindAssets` for searching.
  - Pattern must be valid.

### Scene Management

#### create_scene

Creates a new scene with optional initial setup. Supports custom paths and immediate saving of the created scene.

- **Implementation Notes:**
  - Uses `EditorSceneManager.NewScene` with `NewSceneSetup.EmptyScene`.
  - Automatically creates directories in the specified path.
  - Saves scene immediately after creation.

#### get_active_scene

Retrieves detailed information about the currently active scene, including path, build index, and modification state.

- **Implementation Notes:**
  - Uses `EditorSceneManager.GetActiveScene` to get scene details.
  - Returns scene properties including name, path, build index, and dirty state.
  - Fails if no valid scene is active.

#### get_scene_hierarchy

Retrieves the complete hierarchy of GameObjects in the active scene, including transform data and object properties.

- **Implementation Notes:**
  - Recursively traverses scene using `GetRootGameObjects` and child transforms.
  - Captures detailed object state including transforms, tags, and layers.
  - Builds a complete tree structure of the scene hierarchy.

#### load_scene

Loads a scene by path, name, or build index. Supports loading modes and checks for unsaved changes.

- **Implementation Notes:**
  - Uses `EditorSceneManager.OpenScene` with `OpenSceneMode.Single`.
  - Accepts scene path, name, or build index.
  - Prevents loading if current scene has unsaved changes.

#### save_scene

Saves the currently active scene, optionally to a new path ("Save As" functionality).

- **Implementation Notes:**
  - Uses `EditorSceneManager.SaveScene` for both new and existing scenes.
  - Creates directories if needed when saving to new path.
  - Requires explicit path/name for untitled scenes.

### GameObject Management

#### get_gameobject_details

Retrieves a hierarchical representation of a specified GameObject in the current scene, including its children and component information.

- **Implementation Notes:**
  - Similar to `get_prefab_details` but operates on scene GameObjects instead of prefab assets
  - Uses `GameObjectActionUtility.FindGameObject` to locate target by name, path, or instance ID
  - Builds a complete hierarchy tree with transform, tag, layer, and component information
  - Supports optional inclusion of child GameObjects and detailed component properties
  - Response contains a `hierarchy` field with a `GameObjectHierarchyNode` object containing:
    - Basic properties (name, path, tag, layer, instance ID)
    - List of component type names
    - Optional detailed component information
    - Optional recursive child hierarchy

#### get_gameobject_info

Retrieves detailed information about a GameObject in the scene, with customizable detail levels.

- **Implementation Notes:**
  - Uses `GameObjectActionUtility` for querying GameObject properties.
  - Supports three detail levels: summary, detailed, and component-specific details.
  - Can find GameObjects by name, path, or instance ID.

#### create_gameobject

Creates a new GameObject in the scene with extensive configuration options.

- **Implementation Notes:**
  - Supports primitive types (Cube, Sphere, etc.) or empty GameObjects.
  - Can set initial transform, tag, layer, and parent.
  - Automatically creates non-existent tags.
  - Supports adding components by type name with error handling.

#### delete_gameobject

Deletes a GameObject from the scene with proper undo support.

- **Implementation Notes:**
  - Uses `Undo.DestroyObjectImmediate` for undo/redo support.
  - Can find target by name, hierarchy path, or instance ID.
  - Includes child objects in deletion.

#### find_gameobject

Finds GameObjects in the scene by name, tag, or hierarchy path.

- **Implementation Notes:**
  - Supports finding single or multiple objects.
  - Can include/exclude inactive objects in search.
  - Returns detailed object information including transforms and hierarchy paths.
  - Uses efficient search methods based on criteria type.

#### modify_gameobject

Modifies properties and components of an existing GameObject.

- **Implementation Notes:**
  - Supports changing name, tag, layer, parent, transform, and active state.
  - Can add/remove components by type name.
  - Allows setting component properties via reflection.
  - Includes undo support for all modifications.
  - Automatically creates non-existent tags.

### Editor Management

#### execute_menu_item

Executes a Unity Editor menu item specified by its full menu path.

- **Implementation Notes:**
  - Uses `EditorApplication.ExecuteMenuItem` to trigger menu actions.
  - Provides validation of menu path existence.
  - Returns success/failure status with detailed error messages.
  - Particularly useful for automation of common Editor tasks.

#### get_state

Retrieves comprehensive information about the current Unity Editor state.

- **Implementation Notes:**
  - Provides detailed editor state including play mode, pause state, compilation status.
  - Accesses multiple `EditorApplication` properties: `isPlaying`, `isPaused`, `isCompiling`, `isUpdating`.
  - Includes editor paths and startup time information.
  - Safe error handling with fallback to default state values.

#### pause

Toggles pause state during play mode in the Unity Editor.

- **Implementation Notes:**
  - Uses `EditorApplication.isPaused` to control game execution.
  - Only operates when editor is in play mode.
  - Returns current pause state after toggle.

#### play

Initiates play mode in the Unity Editor.

- **Implementation Notes:**
  - Controls play mode via `EditorApplication.isPlaying`.
  - Safely handles requests when already in play mode.
  - Provides status feedback for state changes.

#### stop

Exits play mode in the Unity Editor.

- **Implementation Notes:**
  - Safely terminates play mode using `EditorApplication.isPlaying`.
  - Handles cases where editor is already stopped.
  - Includes error handling for clean play mode exit.

### Script Management

#### create_script

Creates a new C# script file with code generation and template support.

- **Implementation Notes:**
  - Supports different script types (`MonoBehaviour`, `ScriptableObject`, `Editor`, `EditorWindow`).
  - Generates appropriate using statements and base class inheritance.
  - Includes basic syntax validation and namespace support.
  - Creates necessary directories in the Assets folder.

#### delete_script

Safely removes a C# script from the project.

- **Implementation Notes:**
  - Uses `AssetDatabase.MoveAssetToTrash` for safe deletion with recovery option.
  - Handles script path resolution relative to Assets folder.
  - Refreshes AssetDatabase after deletion.

#### read_script

Retrieves the contents of an existing C# script file.

- **Implementation Notes:**
  - Provides direct access to script content using File I/O.
  - Handles path resolution for scripts in different project locations.
  - Returns both content and resolved asset path.

#### update_script

Modifies the contents of an existing C# script with basic validation.

- **Implementation Notes:**
  - Includes simple syntax validation (brace matching).
  - Automatically imports and refreshes the asset after update.
  - Preserves file path and structure during updates.

### Console Log

#### clear_console

Clears all entries from the Unity Editor console.

- **Implementation Notes:**
  - Uses reflection to access Unity's internal LogEntries API
  - Handles errors gracefully with detailed failure messages
  - Works synchronously to ensure console is cleared before continuing

#### get_console_logs

Retrieves Unity Editor console log entries with support for filtering and pagination.

- **Implementation Notes:**
  - Uses reflection to access Unity's internal LogEntries and LogEntry types
  - Supports filtering by log type (error, warning, log)
  - Enables text-based filtering of log messages
  - Returns entries with full message and stacktrace information
  - Results are ordered chronologically
  - Includes count limiting for performance with large logs

### Schema Discovery

#### get_action_schemas

Retrieves JSON schemas for Unity Editor actions, including type information and documentation. Essential for API discoverability and client-side validation.

- **Implementation Notes:**
  - Uses reflection to generate detailed JSON schemas for action request/response types
  - Incorporates XML documentation comments and SchemaDocumentation attributes
  - Implements efficient schema caching based on assembly modification time
  - Supports filtering to get schemas for specific actions only
  - Handles complex type hierarchies with recursion protection
