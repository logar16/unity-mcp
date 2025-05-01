from typing import Literal
from typing import Annotated
from pydantic import Field, ConfigDict, BaseModel
from .common import BaseActionRequest

class CountValue(BaseModel):
    HasValue: bool
    Value: int

class GetConsoleLogsRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["get_console_logs"] = "get_console_logs"
    id: str | None = None
    types: list[str] | None = Field(
        default=None,
        description="Filters the returned log entries by log type. Allowed values: 'error', 'warning', 'log'."
    )
    filter_text: str | None = Field(
        default=None,
        description="Filters the returned log entries by substring match in the message."
    )
    count: CountValue | None = Field(
        default=None,
        description="Limits the maximum number of log entries returned."
    )

class ClearConsoleRequest(BaseActionRequest):
    model_config = ConfigDict(extra='forbid')
    action: Literal["clear_console"] = "clear_console"
    id: str | None = None
