// Copyright (c) Vibraint. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

namespace UnityMcp.Editor.Actions.AssetManagement
{
    [InitializeOnLoad]
    public static class AssetCreateModifyActions
    {
        static AssetCreateModifyActions()
        {
            ActionRegistry.RegisterAction<CreateAssetRequest>("create_asset", HandleCreateAsset);
            ActionRegistry.RegisterAction<ModifyAssetRequest>("modify_asset", HandleModifyAsset);
        }

        // CREATE ASSET
        public static CreateAssetResponse HandleCreateAsset(CreateAssetRequest req)
        {
            var resp = new CreateAssetResponse();
            try
            {
                if (string.IsNullOrEmpty(req.path) || string.IsNullOrEmpty(req.asset_type))
                {
                    resp.message = "Missing required parameters: path or asset_type.";
                    return resp;
                }

                string fullPath = req.path.Replace("\\", "/");
                string dir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    AssetDatabase.Refresh();
                }
                if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPath) != null)
                {
                    resp.message = $"Asset already exists at path: {fullPath}";
                    return resp;
                }

                string type = req.asset_type.ToLowerInvariant();
                UnityEngine.Object created = null;

                if (type == "folder")
                {
                    string parent = Path.GetDirectoryName(fullPath);
                    string folderName = Path.GetFileName(fullPath);
                    AssetDatabase.CreateFolder(parent, folderName);
                    AssetDatabase.Refresh();
                    resp.asset_path = fullPath;
                    resp.guid = AssetDatabase.AssetPathToGUID(fullPath);
                    resp.success = true;
                    resp.message = "Folder created.";
                    return resp;
                }
                else if (type == "material")
                {
                    var mat = new Material(Shader.Find("Standard"));
                    ApplyMaterialProperties(mat, req.properties);
                    AssetDatabase.CreateAsset(mat, fullPath);
                    created = mat;
                }
                else if (type == "scriptableobject")
                {
                    if (req.properties == null || !req.properties.ContainsKey("scriptClass"))
                    {
                        resp.message = "Missing scriptClass for ScriptableObject creation.";
                        return resp;
                    }
                    string className = req.properties["scriptClass"].ToString();
                    var soType = Type.GetType(className) ?? AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.Name == className);
                    if (soType == null || !typeof(ScriptableObject).IsAssignableFrom(soType))
                    {
                        resp.message = $"Type '{className}' not found or not a ScriptableObject.";
                        return resp;
                    }
                    var so = ScriptableObject.CreateInstance(soType);
                    ApplyScriptableObjectProperties(so, req.properties);
                    AssetDatabase.CreateAsset(so, fullPath);
                    created = so;
                }
                else
                {
                    resp.message = $"Unsupported asset_type: {type}";
                    return resp;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                resp.asset_path = fullPath;
                resp.guid = AssetDatabase.AssetPathToGUID(fullPath);
                resp.success = true;
                resp.message = "Asset created.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"CreateAsset failed: {ex.Message}";
                return resp;
            }
        }

        // MODIFY ASSET
        public static ModifyAssetResponse HandleModifyAsset(ModifyAssetRequest req)
        {
            var resp = new ModifyAssetResponse();
            try
            {
                if (string.IsNullOrEmpty(req.path))
                {
                    resp.message = "Missing required parameter: path.";
                    return resp;
                }
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(req.path);
                if (asset == null)
                {
                    resp.message = $"Asset not found at path: {req.path}";
                    return resp;
                }

                if (asset is Material mat)
                {
                    ApplyMaterialProperties(mat, req.properties);
                    EditorUtility.SetDirty(mat);
                }
                else if (asset is ScriptableObject so)
                {
                    ApplyScriptableObjectProperties(so, req.properties);
                    EditorUtility.SetDirty(so);
                }
                else
                {
                    // Try AssetImporter for textures, etc.
                    var importer = AssetImporter.GetAtPath(req.path);
                    if (importer != null)
                        ApplyImporterProperties(importer, req.properties);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                resp.success = true;
                resp.message = "Asset modified.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"ModifyAsset failed: {ex.Message}";
                return resp;
            }
        }

        // --- Helpers ---
        private static void ApplyMaterialProperties(Material mat, Dictionary<string, object> properties)
        {
            if (mat == null || properties == null) return;
            foreach (var kvp in properties)
            {
                switch (kvp.Key)
                {
                    case "shader":
                        mat.shader = Shader.Find(kvp.Value.ToString());
                        break;
                    case "color":
                        if (ColorUtility.TryParseHtmlString(kvp.Value.ToString(), out var color))
                            mat.color = color;
                        break;
                    // Add more property mappings as needed
                }
            }
        }

        private static void ApplyScriptableObjectProperties(ScriptableObject so, Dictionary<string, object> properties)
        {
            if (so == null || properties == null) return;
            foreach (var kvp in properties)
            {
                if (kvp.Key == "scriptClass") continue;
                var field = so.GetType().GetField(kvp.Key);
                if (field != null)
                {
                    try { field.SetValue(so, Convert.ChangeType(kvp.Value, field.FieldType)); }
                    catch { }
                }
            }
        }

        private static void ApplyImporterProperties(AssetImporter importer, Dictionary<string, object> properties)
        {
            if (importer == null || properties == null) return;
            foreach (var kvp in properties)
            {
                var prop = importer.GetType().GetProperty(kvp.Key);
                if (prop != null && prop.CanWrite)
                {
                    try { prop.SetValue(importer, Convert.ChangeType(kvp.Value, prop.PropertyType)); }
                    catch { }
                }
            }
            importer.SaveAndReimport();
        }
    }
}
