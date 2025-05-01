import builtins
print(">>> [models/__init__.py] LOADING __init__.py", file=builtins.__dict__.get('stderr', None) or __import__('sys').stderr)

from typing import Annotated, Union
from pydantic import Field

from .asset_management import (
    CreateAssetRequest,
    ModifyAssetRequest,
    MoveAssetRequest,
    DeleteAssetRequest,
    ImportAssetRequest,
    GetAssetInfoRequest,
    SearchAssetsRequest,
    DuplicateAssetRequest,
)
print(">>> [models/__init__.py] imported asset_management", file=builtins.__dict__.get('stderr', None) or __import__('sys').stderr)

from .console_log import (
    GetConsoleLogsRequest,
    ClearConsoleRequest,
)
print(">>> [models/__init__.py] imported console_log", file=builtins.__dict__.get('stderr', None) or __import__('sys').stderr)

from .editor_control import (
    ExecuteMenuItemRequest,
    PlayRequest,
    PauseRequest,
    StopRequest,
    GetStateRequest,
)
print(">>> [models/__init__.py] imported editor_control", file=builtins.__dict__.get('stderr', None) or __import__('sys').stderr)

from .game_object_management import (
    CreateGameObjectRequest,
    FindGameObjectRequest,
    ModifyGameObjectRequest,
    DeleteGameObjectRequest,
    GetGameObjectInfoRequest,
)
print(">>> [models/__init__.py] imported game_object_management", file=builtins.__dict__.get('stderr', None) or __import__('sys').stderr)

from .prefab_management import (
    AddPrefabChildRequest,
    RemovePrefabChildRequest,
    RenamePrefabChildRequest,
    ModifyPrefabChildRequest,
    AddPrefabComponentRequest,
    RemovePrefabComponentRequest,
    ModifyPrefabComponentRequest,
)
print(">>> [models/__init__.py] imported prefab_management", file=builtins.__dict__.get('stderr', None) or __import__('sys').stderr)

AnyRequest = Annotated[
    Union[
        # Asset Management
        CreateAssetRequest,
        ModifyAssetRequest,
        MoveAssetRequest,
        DeleteAssetRequest,
        ImportAssetRequest,
        GetAssetInfoRequest,
        SearchAssetsRequest,
        DuplicateAssetRequest,
        # Console Log
        GetConsoleLogsRequest,
        ClearConsoleRequest,
        # Editor Control
        ExecuteMenuItemRequest,
        PlayRequest,
        PauseRequest,
        StopRequest,
        GetStateRequest,
        # GameObject Management
        CreateGameObjectRequest,
        FindGameObjectRequest,
        ModifyGameObjectRequest,
        DeleteGameObjectRequest,
        GetGameObjectInfoRequest,
        # Prefab Management
        AddPrefabChildRequest,
        RemovePrefabChildRequest,
        RenamePrefabChildRequest,
        ModifyPrefabChildRequest,
        AddPrefabComponentRequest,
        RemovePrefabComponentRequest,
        ModifyPrefabComponentRequest,
    ],
    Field(discriminator="action"),
]
