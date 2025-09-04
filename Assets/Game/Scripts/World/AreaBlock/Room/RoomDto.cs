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
        }
#endif
    }
}