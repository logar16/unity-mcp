from pydantic import Field
from typing import Annotated
from models.common import BaseActionRequest

class GetSceneHierarchyInput(BaseActionRequest):
    pass

class GetActiveSceneInput(BaseActionRequest):
    pass

class LoadSceneInput(BaseActionRequest):
    name: Annotated[str | None, "Name of the scene to load. One way to identify the scene. If not provided, use 'path' or 'build_index'."] = None
    path: Annotated[str | None, "Relative path (from Assets) to the scene file. One way to identify the scene. If not provided, use 'name' or 'build_index'."] = None
    build_index: Annotated[dict | None, "Build index of the scene to load. Alternative way to identify the scene. If not provided, use 'name' or 'path'."] = None

class CreateSceneInput(BaseActionRequest):
    name: str | None = None
    path: Annotated[str | None, "Optional directory (relative to Assets) where the scene will be created. Defaults to 'Assets/Scenes' if not specified."] = None

class SaveSceneInput(BaseActionRequest):
    name: Annotated[str | None, "Name to use when saving the scene. Required if saving an untitled scene or using 'Save As'."] = None
    path: Annotated[str | None, "Optional directory (relative to Assets) where the scene will be saved. If not specified, saves to the current scene's path."] = None
