using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [CreateAssetMenu(fileName = "RoomDto", menuName = "Game/RoomDto")]
    public class RoomDto : Dto
    {
        [LabelText("房间里面拥有的互动节点")]
        public List<NodeDto> nodeDatas = new List<NodeDto>();

        [LabelText("房间里面拥有的节点ID"), ReadOnly]
        public List<string> nodeIds = new List<string>();
        

#if UNITY_EDITOR
        public void SyncIdsAndIndexes(AreaBlockDto ownerAb)
        {
            dtoId = DtoId.Join(ownerAb.dtoId, configId);

            nodeIds ??= new List<string>();
            nodeIds.Clear();

            if (nodeDatas != null)
            {
                foreach (var n in nodeDatas)
                {
                    if (n == null) continue;
                    n.dtoId = DtoId.Join(dtoId, n.configId);
                    nodeIds.Add(n.dtoId);
                }
            }

            // 校验：节点列表无空、无重复
            for (int i = 0; i < nodeIds.Count; i++)
            {
                if (string.IsNullOrEmpty(nodeIds[i]))
                    Debug.LogError($"[RoomDto] 存在空节点ID: 房间 {name}");
            }
            var set = new HashSet<string>(nodeIds);
            if (set.Count != nodeIds.Count)
                Debug.LogError($"[RoomDto] 节点ID存在重复: 房间 {name}");
        }
#endif
    }
}