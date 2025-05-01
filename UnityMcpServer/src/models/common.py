import uuid
from typing import Optional
from pydantic import BaseModel, Field

def make_guid() -> str:
    """Generates a unique identifier (GUID) for requests."""
    return str(uuid.uuid4())

class BaseActionRequest(BaseModel):
    action: str = Field(..., description="The specific action to perform.")
    id: str = Field(default_factory=make_guid, description="Unique identifier for the request.")

class BaseActionResponse(BaseModel):
    id: str = Field(..., description="Identifier matching the request.")
    action: str = Field(..., description="The action that was performed.")
    success: bool = Field(False, description="Indicates if the action was successful.")
    message: Optional[str] = Field(None, description="Optional message, often used for errors.")

class Vector3Data(BaseModel):
    x: float = Field(..., description="X component")
    y: float = Field(..., description="Y component")
    z: float = Field(..., description="Z component")

class TransformData(BaseModel):
    position: Vector3Data = Field(..., description="Local position")
    rotation: Vector3Data = Field(..., description="Local rotation")
    world_position: Vector3Data = Field(..., description="World position")
    world_rotation: Vector3Data = Field(..., description="World rotation")
    scale: Vector3Data = Field(..., description="Local scale")

# --- Added for execute_menu_item action ---
from typing import Annotated

class ExecuteMenuItemRequest(BaseActionRequest):
    menu_path: Annotated[
        str,
        "The full path of the Unity Editor menu item to execute. Example: 'File/Save Project'"
    ]
    action: str | None = None
    id: str | None = None
