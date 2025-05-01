from typing import List, Optional
from pydantic import Field
from .common import BaseActionRequest

from typing import Annotated

class GetActionSchemasRequest(BaseActionRequest):
    action_names: Annotated[
        list[str] | None,
        "Optional list of action names to retrieve schemas for. If null or empty, returns all. Example: [\"create_gameobject\", \"load_scene\"]"
    ] = None
    action: str = "get_action_schemas"
