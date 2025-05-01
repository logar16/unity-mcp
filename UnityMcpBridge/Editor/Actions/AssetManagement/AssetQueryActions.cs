// Copyright (c) Vibraint. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;

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
        public static GetAssetInfoResponse HandleGetAssetInfo(GetAssetInfoRequest req)
        {
            var resp = new GetAssetInfoResponse();
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
                resp.asset_path = req.path;
                resp.guid = AssetDatabase.AssetPathToGUID(req.path);
                resp.asset_type = asset.GetType().Name;
                resp.name = asset.name;
                resp.success = true;
                resp.message = "Asset info retrieved.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"GetAssetInfo failed: {ex.Message}";
                return resp;
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
