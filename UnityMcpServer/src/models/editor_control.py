from typing import Literal
from pydantic import Field, ConfigDict
from .common import BaseActionRequest

class ExecuteMenuItemRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["execute_menu_item"] = "execute_menu_item"
    menu_path: str = Field(..., description="Menu path to execute")

class PlayRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["play"] = "play"

class PauseRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["pause"] = "pause"

class StopRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["stop"] = "stop"

class GetStateRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["get_state"] = "get_state"
