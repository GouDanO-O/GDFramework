using System;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class WorldDataTemporary : TemporalityData
    {
        /// <summary>
        /// 当前的年数
        /// </summary>
        [LabelText("当前的年数")]
        public int curWorldYearTime = 1;
        
        /// <summary>
        /// 当前的天数
        /// </summary>
        [LabelText("当前的天数")]
        public int curWorldDayTime = 1;

        /// <summary>
        /// 当前的小时数
        /// </summary>
        [LabelText("当前的小时数")]
        public int curWorldHourTime = 9;

        /// <summary>
        /// 当前的分钟数
        /// </summary>
        [LabelText("当前的分钟数")]
        public int curWorldMinutesTime = 0;
        
        [LabelText("当前玩家是否所处这个世界")]
        public bool playerIsLocateThisWorld;

        /// <summary>
        /// 当前玩家所处的区块ID
        /// </summary>
        [LabelText("当前玩家所处的区块ID")]
        public string curPlayerLocateAreaBlockId;
        
        /// <summary>
        /// 只有当玩家所处这块世界时,当玩家进入和离开该世界的区域时,才会进行更新区域
        /// 如果玩家离开该世界时,如果世界设置里面没有开启缓存当前区域ID,则下次进入则会进入初始区域
        /// </summary>
        /// <param name="curAreaBlockId"></param>
        public void UpdateCurPlayerLocateRoomId(string curAreaBlockId)
        {
            if (playerIsLocateThisWorld)
            {
                this.curPlayerLocateAreaBlockId = curAreaBlockId;
            }
        }
        
    }
}