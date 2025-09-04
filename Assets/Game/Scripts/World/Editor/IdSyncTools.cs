using Game.World;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.World.Editor
{
    public class IdSyncTools
    {
        [MenuItem("Tools/IDs/同步选中 World 的 dtoId 与索引")]
        public static void SyncSelectedWorld()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is WorldDto wd)
                {
                    wd.SyncIdsAndIndexes();
                    EditorUtility.SetDirty(wd);

                    // 标记子资产已变更
                    if (wd.areaBlockDatas != null)
                    {
                        foreach (var ab in wd.areaBlockDatas)
                        {
                            if (!ab) 
                                continue;
                            EditorUtility.SetDirty(ab);

                            if (ab.roomDatas != null)
                            {
                                foreach (var r in ab.roomDatas)
                                {
                                    if (!r) continue;
                                    EditorUtility.SetDirty(r);

                                    if (r.nodeDatas != null)
                                    {
                                        foreach (var n in r.nodeDatas)
                                        {
                                            if (!n) 
                                                continue;
                                            EditorUtility.SetDirty(n);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[IDs] 同步完成。");
        }
    }
}