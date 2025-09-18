using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [CreateAssetMenu(fileName = "AreaBlockDto", menuName = "Game/AreaBlockDto")]
    public class AreaBlockDto : Dto
    {
        [Title("玩家第一次进入区块所处的房间ID,如果为空,则默认取索引第一位"),LabelText("初始房间ID")]
        public string initialRoomId;
        
#if UNITY_EDITOR
        [LabelText("(编辑期引用)初始房间"), Tooltip("仅编辑期辅助,构建/运行期请使用 initialRoomId")]
        public RoomDto initialRoomDtoRef;
#endif

        [Title("当玩家进入又离开区块时,是否需要缓存当前所处房间ID,如果不缓存,则每次进入都进入初始房间,否则进入历史房间"),LabelText("是否缓存当前房间id")]
        public bool willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock;
        
        [LabelText("区块里面的房间")]
        public List<RoomDto> roomDatas = new List<RoomDto>();
        
        [LabelText("区块所拥有的房间ID"),ReadOnly]
        public List<string> roomIds  = new List<string>();

#if UNITY_EDITOR
        public void SyncIdsAndIndexes(WorldDto ownerWorld)
        {
            // 自己的 dtoId 在上层已设置；这里主要负责向下（房间/节点）同步
            roomIds ??= new List<string>();
            roomIds.Clear();

            if (roomDatas != null)
            {
                foreach (var room in roomDatas)
                {
                    if (room == null) continue;
                    room.dtoId = DtoId.Join(dtoId, room.configId);
                    roomIds.Add(room.dtoId);
                    room.SyncIdsAndIndexes(this);
                }
            }

            // 归一化初始房间 id（若设置了引用）
            if (initialRoomDtoRef != null)
                initialRoomId = initialRoomDtoRef.dtoId;

            // 校验：初始房间必须存在
            if (!string.IsNullOrEmpty(initialRoomId) && !roomIds.Contains(initialRoomId))
            {
                Debug.LogError($"[AreaBlockDto] 初始房间ID不在列表中: {initialRoomId}  区块: {name}");
            }
        }
#endif
        
    }
}