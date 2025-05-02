from typing import Optional, List, Dict, Any, Literal
from pydantic import Field, ConfigDict
from .common import BaseActionRequest, Vector3Data, TransformData


class ComponentPropertyData:
    type_name: str
    properties: Dict[str, Any]


class GameObjectIdentifier:
    instance_id: int
    name: str
    path: str
    tag: str
    layer: int
    active_self: bool
    transform: TransformData


class CreateGameObjectRequest(BaseActionRequest):
    model_config = ConfigDict(extra="forbid")
    action: str = "create_gameobject"
    name: str
    parent: str | None = Field(
        default=None, description="Parent GameObject. Can be specified by name, hierarchy path, or instance ID."
    )
    position: dict | None = None
    rotation: dict | None = None
    scale: dict | None = None
    tag: str | None = None
    layer: str | None = None
    components_to_add: list[str] | None = Field(
        default=None,
        description="List of component type names to add to the GameObject. Each entry should be a fully qualified type name or short type name.",
    )
    primitive_type: str | None = Field(
        default=None,
        description="Primitive type to create (optional). Allowed values: Cube, Sphere, Capsule, Cylinder, Plane, Quad.",
    )


class FindGameObjectRequest(BaseActionRequest):
    model_config = ConfigDict(extra="forbid")
    action: str = "find_gameobject"
    name: str | None = None
    tag: str | None = None
    path: str | None = None
    find_all: bool = Field(
        default=False,
        description="Whether to return all matching GameObjects or only the first match. If true, returns all matches; if false, returns only the first match.",
    )
    search_inactive: bool = Field(
        default=False,
        description="Whether to include inactive GameObjects in the search. If true, includes inactive objects; if false, only active objects are considered.",
    )


class ModifyGameObjectRequest(BaseActionRequest):
    model_config = ConfigDict(extra="forbid")
    action: str = "modify_gameobject"
    target: str = Field(..., description="Target GameObject. Can be specified by name, hierarchy path, or instance ID.")
    name: str | None = None
    tag: str | None = None
    layer: str | None = None
    parent: str | None = Field(
        default=None, description="Parent GameObject. Can be specified by name, hierarchy path, or instance ID."
    )
    position: dict | None = None
    rotation: dict | None = None
    scale: dict | None = None
    set_active: dict | None = Field(
        default=None,
        description="Whether to set the GameObject active or inactive. If true, sets the GameObject active; if false, sets it inactive.",
    )
    components_to_add: list[str] | None = Field(
        default=None,
        description="List of component type names to add to the GameObject. Each entry should be a fully qualified type name or short type name.",
    )
    components_to_remove: list[str] | None = Field(
        default=None,
        description="List of component type names to remove from the GameObject. Each entry should be a fully qualified type name or short type name.",
    )
    component_properties: dict | None = Field(
        default=None,
        description="Properties to set on components, grouped by component type. Dictionary format: { component_type: { property_name: value } }",
    )


class DeleteGameObjectRequest(BaseActionRequest):
    model_config = ConfigDict(extra="forbid")
    action: str = "delete_gameobject"
    target: str = Field(..., description="Target GameObject. Can be specified by name, hierarchy path, or instance ID.")


class GetGameObjectInfoRequest(BaseActionRequest):
    model_config = ConfigDict(extra="forbid")
    action: str = "get_gameobject_info"
    target: str = Field(..., description="Target GameObject. Can be specified by name, hierarchy path, or instance ID.")
    detail_level: str | None = Field(
        default=None,
        description='Level of detail for the response. Allowed values: "summary", "detailed", "component_details".',
    )
    component_type: str | None = Field(
        default=None,
        description='Component type name for use with detail_level="component_details". Required if detail_level is "component_details".',
    )


class GetGameObjectDetailsRequest(BaseActionRequest):
    model_config = ConfigDict(extra="forbid")
    action: str = "get_gameobject_details"
    target: str = Field(
        ...,
        description='Identifier for the root GameObject. Can be specified by instance ID (numeric string), name (exact match), or hierarchy path (e.g. "Parent/Child/GameObject").',
    )
    include_children: bool = Field(
        default=False,
        description="If true, recursively includes child GameObjects in the response hierarchy. If false, only returns the target GameObject's information. Defaults to false.",
    )
    include_component_details: bool = Field(
        default=False,
        description="If true, includes detailed property information for components on the returned GameObject(s). If false, only includes component type names. Defaults to false.",
    )
