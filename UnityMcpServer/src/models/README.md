# Pydantic Models for Unity Communication

This directory contains Pydantic models used for serializing and validating data exchanged between the Python MCP server and the Unity Editor via JSON messages.

## File Structure

The Python models are organized to mirror the structure of the corresponding C# models found in the `Models/` directory of the Unity project.

```shell
UnityMcpServer/src/models/
├── __init__.py                 # Defines and exports AnyRequest, AnyResponse discriminated unions. Imports models from submodules.
├── common.py                   # Contains BaseActionRequest, BaseActionResponse, and common data types (Vector3Data, etc.).
├── asset_management.py         # Models corresponding to Models/AssetManagementModels.cs
├── console_log.py              # Models corresponding to Models/ConsoleLogModels.cs
├── editor_control.py           # Models corresponding to Models/EditorControlModels.cs
├── game_object_management.py   # Models corresponding to Models/GameObjectManagementModels.cs
├── prefab_management.py        # Models corresponding to Models/PrefabManagementModels.cs
├── scene_management.py         # Models corresponding to Models/SceneManagementModels.cs
├── schema.py                   # Models corresponding to Models/SchemaModels.cs
└── script_management.py        # Models corresponding to Models/ScriptManagementModels.cs
```

## Model Design Pattern ("Flat Structure")

We use a "Flat Structure" approach for defining specific request and response models:

1. **Base Models (`common.py`):**
    * `BaseActionRequest` and `BaseActionResponse` define only the common fields shared by all messages (`action`, `id`, `success`, `message`).

    ```python
    # Example from common.py
    from typing import Optional
    from pydantic import BaseModel, Field

    class BaseActionRequest(BaseModel):
        action: str = Field(..., description="The specific action to perform.")
        id: str = Field(..., description="Unique identifier for the request.")

    class BaseActionResponse(BaseModel):
        id: str = Field(..., description="Identifier matching the request.")
        action: str = Field(..., description="The action that was performed.")
        success: bool = Field(False, description="Indicates if the action was successful.")
        message: Optional[str] = Field(None, description="Optional message, often used for errors.")
    ```

2. **Specific Models (e.g., `console_log.py`):**
    * Each specific request/response model (e.g., `GetConsoleLogsRequest`, `GetConsoleLogsResponse`) inherits directly from the simple base model (`BaseActionRequest` or `BaseActionResponse`).
    * Unique fields required for that specific action are added directly as top-level fields within the specific model class.
    * The `action` field is defined with `const=True` to fix its value, which is essential for the discriminated union.

    ```python
    # Example from console_log.py
    from typing import List, Optional
    from pydantic import Field, ConfigDict
    from .common import BaseActionRequest, BaseActionResponse, ConsoleLogEntry # Assuming ConsoleLogEntry is defined

    class GetConsoleLogsRequest(BaseActionRequest):
        model_config = ConfigDict(extra='forbid')
        action: str = Field("get_console_logs", const=True)
        # Unique fields added directly:
        max_entries: Optional[int] = Field(None)
        log_type_filter: Optional[List[str]] = Field(None)

    class GetConsoleLogsResponse(BaseActionResponse):
        model_config = ConfigDict(extra='forbid')
        action: str = Field("get_console_logs", const=True)
        # Unique fields added directly:
        logs: Optional[List[ConsoleLogEntry]] = Field(None)
    ```

Note that for now we are not parsing specific response objects, just the base to check for success and message, then the result is passed to the calling client.

## Discriminated Unions (`__init__.py`)

To handle the variety of possible requests and responses, we use Pydantic's discriminated unions defined in `models/__init__.py`:

* All specific request model classes are imported into `__init__.py`.
* An `AnyRequest` type alias is created using `typing.Annotated` and `typing.Union`, listing all specific request types. `Field(discriminator='action')` tells Pydantic to use the `action` field to determine the correct model type during validation.
* Similarly, an `AnyResponse` type alias is created for all specific response models.

```python
# Example from models/__init__.py
from typing import Union, Annotated
from pydantic import Field

# Import ALL specific request/response models...
from .console_log import GetConsoleLogsRequest, GetConsoleLogsResponse # ...etc

AnyRequest = Annotated[
    Union[
        GetConsoleLogsRequest,
        # ... list ALL other specific request types ...
    ],
    Field(discriminator='action')
]

AnyResponse = Annotated[
    Union[
        GetConsoleLogsResponse,
        # ... list ALL other specific response types ...
    ],
    Field(discriminator='action')
]
```

## Validation

To validate incoming JSON data (e.g., a response dictionary `response_data` from Unity) against the correct specific model:

1. Import the union type (e.g., `AnyResponse`) and `pydantic.TypeAdapter`.
2. Create a `TypeAdapter` instance for the union: `response_validator = TypeAdapter(AnyResponse)`.
3. Call the adapter's validation method: `validated_response = response_validator.validate_python(response_data)` (or `validate_json`).

Pydantic uses the `action` field in the data and the `discriminator` configuration in the `AnyResponse` type to automatically select the correct specific model (e.g., `GetConsoleLogsResponse`) for validation and returns an instance of that specific model.
