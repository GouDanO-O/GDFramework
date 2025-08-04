using System;
using GDFrameworkExtend.Data;

namespace Game.World
{
    [Serializable]
    public class WorldDataTemporary : TemporalityData
    {
        /// <summary>
        /// 当前的年数
        /// </summary>
        public int curWorldYearTime = 1;
        
        /// <summary>
        /// 当前的天数
        /// </summary>
        public int curWorldDayTime = 1;

        /// <summary>
        /// 当前的小时数
        /// </summary>
        public int curWorldHourTime = 9;

        /// <summary>
        /// 当前的分钟数
        /// </summary>
        public int curWorldMinutesTime = 0;
    }
}