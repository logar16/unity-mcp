// Copyright (c) Vibraint. All rights reserved.

using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityMcp.Editor.Core;
using UnityMcp.Editor.Models;
using UnityMcp.Editor.Utility;

namespace UnityMcp.Editor.Actions.AssetManagement
{
    [InitializeOnLoad]
    public static class AssetFileOpsActions
    {
        static AssetFileOpsActions()
        {
            ActionRegistry.RegisterAction<MoveAssetRequest>("move_asset", HandleMoveAsset);
            ActionRegistry.RegisterAction<DeleteAssetRequest>("delete_asset", HandleDeleteAsset);
            ActionRegistry.RegisterAction<DuplicateAssetRequest>("duplicate_asset", HandleDuplicateAsset);
        }

        // MOVE ASSET
        public static MoveAssetResponse HandleMoveAsset(MoveAssetRequest req)
        {
            var resp = new MoveAssetResponse();
            try
            {
                if (string.IsNullOrEmpty(req.path) || string.IsNullOrEmpty(req.destination))
                {
                    resp.message = "Missing required parameters: path or destination.";
                    return resp;
                }

                string error = AssetDatabase.MoveAsset(req.path, req.destination);

                if (!string.IsNullOrEmpty(error))
                {
                    resp.message = $"MoveAsset error: {error}";
                    return resp;
                }

                string guid = AssetDatabase.AssetPathToGUID(req.destination);

                resp.new_path = req.destination;
                resp.guid = guid;
                resp.success = true;
                resp.message = "Asset moved.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"MoveAsset failed: {ex.Message}";
                return resp;
            }
        }

        // DELETE ASSET
        public static DeleteAssetResponse HandleDeleteAsset(DeleteAssetRequest req)
        {
            var resp = new DeleteAssetResponse();
            try
            {
                if (string.IsNullOrEmpty(req.path))
                {
                    resp.message = "Missing required parameter: path.";
                    return resp;
                }

                bool result = AssetDatabase.DeleteAsset(req.path);

                if (!result)
                {
                    resp.message = "DeleteAsset failed.";
                    return resp;
                }
                resp.success = true;
                resp.message = "Asset deleted.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"DeleteAsset failed: {ex.Message}";
                return resp;
            }
        }

        // DUPLICATE ASSET
        public static DuplicateAssetResponse HandleDuplicateAsset(DuplicateAssetRequest req)
        {
            var resp = new DuplicateAssetResponse();
            try
            {
                if (string.IsNullOrEmpty(req.path) || string.IsNullOrEmpty(req.destination))
                {
                    resp.message = "Missing required parameters: path or destination.";
                    return resp;
                }
                bool result = AssetDatabase.CopyAsset(req.path, req.destination);
                if (!result)
                {
                    resp.message = "DuplicateAsset failed.";
                    return resp;
                }
                resp.new_path = req.destination;
                resp.guid = AssetDatabase.AssetPathToGUID(req.destination);
                resp.success = true;
                resp.message = "Asset duplicated.";
                return resp;
            }
            catch (Exception ex)
            {
                resp.message = $"DuplicateAsset failed: {ex.Message}";
                return resp;
            }
        }
    }
}
