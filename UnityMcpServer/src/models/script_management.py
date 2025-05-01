from pydantic import BaseModel, Field
from typing import Annotated
from .common import BaseActionRequest

class ReadScriptRequest(BaseActionRequest):
    name: str
    path: Annotated[str | None, "Optional. Relative to the Assets folder. Used to identify the location of the script. If omitted, defaults to 'Scripts'."] = None
    id: str | None = None

class DeleteScriptRequest(BaseActionRequest):
    name: str
    path: Annotated[str | None, "Optional. Relative to the Assets folder. Used to identify the location of the script. If omitted, defaults to 'Scripts'."] = None
    id: str | None = None

class CreateScriptRequest(BaseActionRequest):
    name: str
    path: Annotated[str | None, "Optional. Relative to the Assets folder. Used to identify or specify the location for script creation. If omitted, defaults to 'Scripts'."] = None
    contents: Annotated[str | None, "Full script text. If omitted, a default template is generated."] = None
    script_type: Annotated[str | None, "Optional. Determines the script template and base class. Examples: MonoBehaviour, ScriptableObject, EditorWindow."] = None
    namespace: Annotated[str | None, "Optional. Wraps the script in a C# namespace. Ignored if not provided."] = None
    id: str | None = None

class UpdateScriptRequest(BaseActionRequest):
    name: str
    path: Annotated[str | None, "Optional. Relative to the Assets folder. Used to identify the location of the script. If omitted, defaults to 'Scripts'."] = None
    contents: Annotated[str, "Full script text to overwrite the file."]
    id: str | None = None
