using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions.ScriptManagement
{
    [InitializeOnLoad]
    public static class CreateScriptActionHandler
    {
        static CreateScriptActionHandler()
        {
            ActionRegistry.RegisterAction<CreateScriptRequest>("create_script", Handle);
        }

        public static CreateScriptResponse Handle(CreateScriptRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.name))
            {
                return new CreateScriptResponse
                {
                    success = false,
                    message = "Script name is required."
                };
            }

            if (!IsValidScriptName(request.name))
            {
                return new CreateScriptResponse
                {
                    success = false,
                    message = $"Invalid script name: '{request.name}'. Use only letters, numbers, underscores, and don't start with a number."
                };
            }

            string relativeDir = string.IsNullOrEmpty(request.path) ? "Scripts" : request.path.Replace('\\', '/').Trim('/');
            if (relativeDir.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                relativeDir = relativeDir.Substring("Assets/".Length).TrimStart('/');
            if (string.IsNullOrEmpty(relativeDir))
                relativeDir = "Scripts";

            string scriptFileName = $"{request.name}.cs";
            string fullPathDir = Path.Combine(Application.dataPath, relativeDir);
            string fullPath = Path.Combine(fullPathDir, scriptFileName);
            string relativePath = Path.Combine("Assets", relativeDir, scriptFileName).Replace('\\', '/');

            if (File.Exists(fullPath))
            {
                return new CreateScriptResponse
                {
                    success = false,
                    message = $"Script already exists at '{relativePath}'."
                };
            }

            try
            {
                Directory.CreateDirectory(fullPathDir);

                string contents = string.IsNullOrEmpty(request.contents)
                    ? GenerateDefaultScriptContent(request.name, request.script_type, request.@namespace)
                    : request.contents;

                if (!ValidateScriptSyntax(contents))
                {
                    Debug.LogWarning($"Potential syntax error in script being created: {request.name}");
                }

                File.WriteAllText(fullPath, contents);
                AssetDatabase.ImportAsset(relativePath);
                AssetDatabase.Refresh();

                return new CreateScriptResponse
                {
                    success = true,
                    message = $"Script '{scriptFileName}' created successfully at '{relativePath}'.",
                    path = relativePath
                };
            }
            catch (Exception e)
            {
                return new CreateScriptResponse
                {
                    success = false,
                    message = $"Failed to create script '{relativePath}': {e.Message}"
                };
            }
        }

        private static bool IsValidScriptName(string name)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

        private static string GenerateDefaultScriptContent(string name, string scriptType, string namespaceName)
        {
            string usingStatements = "using UnityEngine;\nusing System.Collections;\n";
            string classDeclaration;
            string body =
                "\n    // Use this for initialization\n    void Start() {\n\n    }\n\n    // Update is called once per frame\n    void Update() {\n\n    }\n";
            string baseClass = "";
            if (!string.IsNullOrEmpty(scriptType))
            {
                if (scriptType.Equals("MonoBehaviour", StringComparison.OrdinalIgnoreCase))
                    baseClass = " : MonoBehaviour";
                else if (scriptType.Equals("ScriptableObject", StringComparison.OrdinalIgnoreCase))
                {
                    baseClass = " : ScriptableObject";
                    body = "";
                }
                else if (
                    scriptType.Equals("Editor", StringComparison.OrdinalIgnoreCase)
                    || scriptType.Equals("EditorWindow", StringComparison.OrdinalIgnoreCase)
                )
                {
                    usingStatements += "using UnityEditor;\n";
                    if (scriptType.Equals("Editor", StringComparison.OrdinalIgnoreCase))
                        baseClass = " : Editor";
                    else
                        baseClass = " : EditorWindow";
                    body = "";
                }
            }

            classDeclaration = $"public class {name}{baseClass}";

            string fullContent = $"{usingStatements}\n";
            bool useNamespace = !string.IsNullOrEmpty(namespaceName);

            if (useNamespace)
            {
                fullContent += $"namespace {namespaceName}\n{{\n";
                classDeclaration = "    " + classDeclaration;
                body = string.Join("\n", body.Split('\n').Select(line => "    " + line));
            }

            fullContent += $"{classDeclaration}\n{{\n{body}\n}}";

            if (useNamespace)
            {
                fullContent += "\n}";
            }

            return fullContent.Trim() + "\n";
        }

        private static bool ValidateScriptSyntax(string contents)
        {
            if (string.IsNullOrEmpty(contents))
                return true;
            int braceBalance = 0;
            foreach (char c in contents)
            {
                if (c == '{') braceBalance++;
                else if (c == '}') braceBalance--;
            }
            return braceBalance == 0;
        }
    }
}
