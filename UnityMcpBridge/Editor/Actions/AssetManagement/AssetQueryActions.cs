// Copyright (c) Vibraint. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Utility;

namespace UnityMcp.Editor.Actions.AssetManagement
{
    [InitializeOnLoad]
    public static class AssetQueryActions
    {
        static AssetQueryActions()
        {
            ActionRegistry.RegisterAction<ImportAssetRequest>("import_asset", HandleImportAsset);
            ActionRegistry.RegisterAction<GetAssetInfoRequest>("get_asset_info", HandleGetAssetInfo);
            ActionRegistry.RegisterAction<SearchAssetsRequest>("search_assets", HandleSearchAssets);
        }

        // IMPORT ASSET
        public static ImportAssetResponse HandleImportAsset(ImportAssetRequest req)
        {
            var resp = new ImportAssetResponse();
            try
            {
                if (string.IsNullOrEmpty(req.path))
                {
                    resp.message = "Missing required parameter: path.";
                    return resp;
                }
                AssetDatabase.ImportAsset(req.path, ImportAssetOptions.ForceUpdate);
                resp.success = true;
                resp.message = "Asset imported.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"ImportAsset failed: {ex.Message}";
                return resp;
            }
        }

        // GET ASSET INFO
        /// <summary>
        /// Retrieves information about a specific asset, with optional detail level.
        /// If detail_level is "full_serialized", includes all visible serialized properties.
        /// </summary>
        /// <param name="req">The asset info request.</param>
        /// <returns>Asset info response, optionally including serialized properties.</returns>
        public static GetAssetInfoResponse HandleGetAssetInfo(GetAssetInfoRequest req)
        {
            try
            {
                if (string.IsNullOrEmpty(req.path))
                {
                    return new GetAssetInfoResponse().Fail(req, "Missing required parameter: path.") as GetAssetInfoResponse;
                }

                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(req.path);

                if (asset == null)
                {
                    return new GetAssetInfoResponse().Fail(req, $"Asset not found at path: {req.path}") as GetAssetInfoResponse;
                }

                string guid = AssetDatabase.AssetPathToGUID(req.path);

                var resp = new GetAssetInfoResponse
                {
                    asset_path = req.path,
                    guid = guid,
                    asset_type = asset.GetType().Name,
                    name = asset.name
                };

                // Enhanced: Optionally include serialized properties
                string detailLevel = req.detail_level ?? "basic";
                if (detailLevel == "full_serialized")
                {
                    var serializedProps = new Dictionary<string, object>();
                    try
                    {
                        var so = new UnityEditor.SerializedObject(asset);
                        var prop = so.GetIterator();
                        bool enterChildren = true;
                        while (prop.NextVisible(enterChildren))
                        {
                            enterChildren = false;
                            // Skip "m_Script" for MonoBehaviours/ScriptableObjects
                            if (prop.propertyPath == "m_Script")
                                continue;

                            object value = null;
                            switch (prop.propertyType)
                            {
                                case UnityEditor.SerializedPropertyType.Integer:
                                    value = prop.intValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.Boolean:
                                    value = prop.boolValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.Float:
                                    value = prop.floatValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.String:
                                    value = prop.stringValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.Color:
                                    value = prop.colorValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.ObjectReference:
                                    value = prop.objectReferenceValue != null ? prop.objectReferenceValue.name : null;
                                    break;
                                case UnityEditor.SerializedPropertyType.LayerMask:
                                    value = prop.intValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.Enum:
                                    value = prop.enumNames != null && prop.enumValueIndex >= 0 && prop.enumValueIndex < prop.enumNames.Length
                                        ? prop.enumNames[prop.enumValueIndex]
                                        : prop.enumValueIndex;
                                    break;
                                case UnityEditor.SerializedPropertyType.Vector2:
                                    value = prop.vector2Value;
                                    break;
                                case UnityEditor.SerializedPropertyType.Vector3:
                                    value = prop.vector3Value;
                                    break;
                                case UnityEditor.SerializedPropertyType.Vector4:
                                    value = prop.vector4Value;
                                    break;
                                case UnityEditor.SerializedPropertyType.Rect:
                                    value = prop.rectValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.Bounds:
                                    value = prop.boundsValue;
                                    break;
                                case UnityEditor.SerializedPropertyType.Quaternion:
                                    value = prop.quaternionValue;
                                    break;
                                default:
                                    value = null;
                                    break;
                            }
                            serializedProps[prop.propertyPath] = value;
                        }
                        resp.serialized_properties = serializedProps;
                        return resp.Success(req, "Asset info with serialized properties retrieved.") as GetAssetInfoResponse;
                    }
                    catch (Exception serEx)
                    {
                        resp.serialized_properties = serializedProps;
                        return resp.Success(req, $"Asset info retrieved, but failed to serialize properties: {serEx.Message}") as GetAssetInfoResponse;
                    }
                }
                else
                {
                    return resp.Success(req, "Asset info retrieved.") as GetAssetInfoResponse;
                }
            }
            catch (Exception ex)
            {
                return new GetAssetInfoResponse().Fail(req, $"GetAssetInfo failed: {ex.Message}") as GetAssetInfoResponse;
            }
        }

        // SEARCH ASSETS
        public static SearchAssetsResponse HandleSearchAssets(SearchAssetsRequest req)
        {
            var resp = new SearchAssetsResponse();
            try
            {
                if (string.IsNullOrEmpty(req.search_pattern))
                {
                    resp.message = "Missing required parameter: search_pattern.";
                    return resp;
                }
                string[] folders = req.search_folders?.ToArray() ?? null;
                var guids = AssetDatabase.FindAssets(req.search_pattern, folders);
                var assets = new List<AssetInfo>();
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                    if (asset != null)
                    {
                        assets.Add(new AssetInfo
                        {
                            asset_path = path,
                            guid = guid,
                            asset_type = asset.GetType().Name,
                            name = asset.name
                        });
                    }
                }
                resp.assets = assets;
                resp.success = true;
                resp.message = "Assets searched.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"SearchAssets failed: {ex.Message}";
                return resp;
            }
        }
    }
}
