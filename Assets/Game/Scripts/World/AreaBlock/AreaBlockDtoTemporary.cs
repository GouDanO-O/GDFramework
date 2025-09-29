using System;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable,JsonObject]
    public class AreaBlockDtoTemporary  : TemporaryData
    {
        [LabelText("是否已经解锁")]
        public bool isUnlocked;

        [ShowIf("isUnlocked"),LabelText("当前探索的进度")] 
        public float curExploreProgress;

        [LabelText("当前玩家是否所处这边区块")]
        public bool playerIsLocateThisAreaBlock;
        
        [ShowIf("playerIsLocateThisAreaBlock"),LabelText("当前玩家所处的区块的房间ID")]
        public string curPlayerLocateRoomId;

        /// <summary>
        /// 只有当玩家所处这块区块时,当玩家进入和离开该区域的房间时,才会进行更新房间
        /// 如果玩家离开该区块时,如果区块设置里面没有开启缓存当前房间ID,则下次进入则会进入初始房间
        /// </summary>
        /// <param name="curRoomId"></param>
        public void UpdateCurPlayerLocateRoomId(string curRoomId)
        {
            if (playerIsLocateThisAreaBlock)
            {
                this.curPlayerLocateRoomId = curRoomId;
            }
        }
    }
}