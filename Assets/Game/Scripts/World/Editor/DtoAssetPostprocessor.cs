using UnityEditor;
using System.Linq;

namespace Game.World.Editor
{
    public class DtoAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // 查找所有被修改的 WorldDto
            var worldDtosToSync = importedAssets
                .Select(AssetDatabase.LoadAssetAtPath<WorldDto>)
                .Where(dto => dto != null)
                .ToList();

            // 如果修改的不是 WorldDto，而是子 Dto，需要找到其归属的 WorldDto
            // 这是一个简化的示例，实际可能需要更复杂的查找逻辑
            foreach (var path in importedAssets)
            {
                var asset = AssetDatabase.LoadAssetAtPath<Dto>(path);
                if (asset != null && !(asset is WorldDto))
                {
                    // 假设所有 Dto 都在一个总的文件夹下，每个子文件夹是一个 World
                    // 或者通过其他方式找到根 WorldDto
                    // 这里为了演示，我们直接查找所有 WorldDto 并更新
                    var allWorldDtoGuids = AssetDatabase.FindAssets("t:WorldDto");
                    foreach (var guid in allWorldDtoGuids)
                    {
                        var worldPath = AssetDatabase.GUIDToAssetPath(guid);
                        var worldDto = AssetDatabase.LoadAssetAtPath<WorldDto>(worldPath);
                        // 一个简单的检查，看被修改的资源是否被这个 WorldDto 引用
                        if (worldDto.areaBlockDatas.Contains(asset as AreaBlockDto)) // 仅为示例
                        {
                            if (!worldDtosToSync.Contains(worldDto))
                            {
                                worldDtosToSync.Add(worldDto);
                            }
                        }
                    }
                }
            }

            foreach (var worldDto in worldDtosToSync)
            {
                UnityEngine.Debug.Log($"自动同步ID: {worldDto.name}");
                worldDto.SyncIdsAndIndexes();
                EditorUtility.SetDirty(worldDto); // 标记为已修改，以便保存
            }
        }
    }
}