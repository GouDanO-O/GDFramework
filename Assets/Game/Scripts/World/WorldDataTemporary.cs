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
    }
}