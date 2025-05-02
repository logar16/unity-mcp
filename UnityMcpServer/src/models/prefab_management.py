from typing import Annotated, Literal
from pydantic import Field, ConfigDict, BaseModel
from .common import BaseActionRequest, Vector3Data

# Helper for set_active object in ModifyPrefabChildRequest
class SetActiveData(BaseModel):
    HasValue: bool = Field(..., description=None)
    Value: bool = Field(..., description=None)

class PrefabChildProperties(BaseModel):
    name: str
    position: Vector3Data | None = None
    rotation: Vector3Data | None = None
    scale: Vector3Data | None = None
    tag: str | None = None
    layer: str | None = None
    components_to_add: list[str] | None = None
    primitive_type: Annotated[
        str | None,
        "Type of Unity primitive to create (if specified). Notes: Allowed values: \"Cube\", \"Sphere\", \"Capsule\", \"Cylinder\", \"Plane\", \"Quad\"."
    ] = None

class AddPrefabChildRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["add_prefab_child"] = "add_prefab_child"
    prefab_path: Annotated[
        str,
        "Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\")."
    ]
    child_properties: Annotated[
        PrefabChildProperties,
        "Properties for the new child GameObject to add."
    ]
    parent_path: Annotated[
        str | None,
        "Path to the parent GameObject within the prefab (e.g., \"Root/Child\"). If null or empty, the child is added to the root."
    ] = None

class RemovePrefabChildRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["remove_prefab_child"] = "remove_prefab_child"
    prefab_path: Annotated[
        str,
        "Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\")."
    ]
    child_path: Annotated[
        str,
        "Path to the child GameObject within the prefab to remove (e.g., \"Root/Child/Gun\")."
    ]

class RenamePrefabChildRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["rename_prefab_child"] = "rename_prefab_child"
    prefab_path: Annotated[
        str,
        "Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\")."
    ]
    child_path: Annotated[
        str,
        "Path to the child GameObject within the prefab to rename (e.g., \"Root/Child/Gun\")."
    ]
    new_name: Annotated[
        str,
        "New name for the child GameObject."
    ]

class ModifyPrefabChildRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["modify_prefab_child"] = "modify_prefab_child"
    prefab_path: Annotated[
        str,
        "Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\")."
    ]
    child_path: Annotated[
        str,
        "Path to the child GameObject within the prefab to modify (e.g., \"Root/Child/Gun\")."
    ]
    position: Vector3Data | None = None
    rotation: Vector3Data | None = None
    scale: Vector3Data | None = None
    tag: str | None = None
    layer: str | None = None
    set_active: Annotated[
        SetActiveData | None,
        "Whether to set the GameObject active (true) or inactive (false). Notes: If omitted, the active state is unchanged."
    ] = None

class AddPrefabComponentRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["add_prefab_component"] = "add_prefab_component"
    prefab_path: Annotated[
        str,
        "Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\")."
    ]
    child_path: Annotated[
        str,
        "Path to the child GameObject within the prefab (e.g., \"Root/Child/Gun\")."
    ]
    component_type: Annotated[
        str,
        "Fully qualified type name of the component to add (e.g., \"UnityEngine.Rigidbody\")."
    ]
    component_properties: Annotated[
        dict[str, object] | None,
        "Optional dictionary of initial property values to set on the new component. Notes: Format: property name (string) -> value (object)."
    ] = None

class RemovePrefabComponentRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["remove_prefab_component"] = "remove_prefab_component"
    prefab_path: Annotated[
        str,
        "Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\")."
    ]
    child_path: Annotated[
        str,
        "Path to the child GameObject within the prefab (e.g., \"Root/Child/Gun\")."
    ]
    component_type: Annotated[
        str,
        "Fully qualified type name of the component to remove (e.g., \"UnityEngine.Rigidbody\")."
    ]

class ModifyPrefabComponentRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["modify_prefab_component"] = "modify_prefab_component"
    prefab_path: Annotated[
        str,
        "Path to the prefab asset, relative to the Assets folder (e.g., \"Prefabs/MyPrefab.prefab\")."
    ]
    child_path: Annotated[
        str,
        "Path to the child GameObject within the prefab (e.g., \"Root/Child/Gun\")."
    ]
    component_type: Annotated[
        str,
        "Fully qualified type name of the component to modify (e.g., \"UnityEngine.Rigidbody\")."
    ]
    component_properties: Annotated[
        dict[str, object],
        "Dictionary of property names and their new values. Notes: Format: property name (string) -> value (object)."
    ]


class GetPrefabDetailsRequest(BaseActionRequest):
    model_config = ConfigDict(extra="forbid")
    action: Literal["get_prefab_details"] = "get_prefab_details"
    prefab_path: Annotated[
        str,
        'Path to the prefab asset, relative to the Assets folder (e.g., "Prefabs/MyPrefab.prefab").'
    ]
    child_path: Annotated[
        str | None,
        'Path to a specific GameObject within the prefab to start the query from (e.g., "Root/Child/Gun"). If null or empty, starts at the prefab root.'
    ] = None
    include_children: Annotated[
        bool,
        "If true, recursively includes child GameObjects in the response hierarchy. If false, only the target node is returned."
    ] = True
    include_component_details: Annotated[
        bool,
        "If true, includes detailed property information for components on the returned GameObject(s)."
    ] = False
    # action and id are included in the schema, but action is set above, and id is handled by BaseActionRequest if needed.
