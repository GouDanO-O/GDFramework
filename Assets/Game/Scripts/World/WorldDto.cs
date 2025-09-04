using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [CreateAssetMenu(fileName = "WorldDto", menuName = "Game/WorldDto")]
    public class WorldDto : Dto
    {
        /// <summary>
        /// 初始年数
        /// </summary>
        [LabelText("初始年数")]
        public int initialWorldYearTime;

        /// <summary>
        /// 初始月数
        /// </summary>
        [LabelText("初始月数")]
        public int initialWorldMonthTime;

        /// <summary>
        /// 初始天数
        /// </summary>
        [LabelText("初始天数")]
        public int initialWorldDayTime;

        /// <summary>
        /// 初始小时数
        /// </summary>
        [LabelText("初始小时数")]
        public int initialWorldHourTime;

        /// <summary>
        /// 初始分钟数
        /// </summary>
        [LabelText("初始分钟数")]
        public int initialWorldMinutesTime;

        /// <summary>
        /// 当前玩家所处的区块ID
        /// </summary>
        [LabelText("初始玩家所处的区块ID(即无特殊事件的情况下,玩家会处于的第一个区块的ID)")]
        public string initialPlayerLocateAreaBlockId;

#if UNITY_EDITOR
        [LabelText("(编辑期引用)初始区块"), Tooltip("仅编辑期辅助,构建/运行期请使用 initialAreaBlockId")]
        public AreaBlockDto initialAreaBlockRef;
#endif

        [LabelText("区块数据列表")]
        public List<AreaBlockDto> areaBlockDatas = new List<AreaBlockDto>();

        [LabelText("当前世界拥有的区块ID"), ReadOnly]
        public List<string> areaBlockIds = new List<string>();

#if UNITY_EDITOR
        /// <summary>
        /// 从当前 World 向下递归同步 dtoId 与 id 索引列表
        /// </summary>
        public void SyncIdsAndIndexes()
        {
            // 顶层 world 的 dtoId = configId
            dtoId = configId;

            areaBlockIds ??= new List<string>();
            areaBlockIds.Clear();

            if (areaBlockDatas != null)
            {
                foreach (var ab in areaBlockDatas)
                {
                    if (ab == null) continue;
                    ab.dtoId = DtoId.Join(dtoId, ab.configId);
                    areaBlockIds.Add(ab.dtoId);

#if UNITY_EDITOR
                    ab.SyncIdsAndIndexes(this);
#endif
                }
            }

            // 如果设置了编辑期引用，则归一化初始区块 id
            if (initialAreaBlockRef != null)
                initialPlayerLocateAreaBlockId = initialAreaBlockRef.dtoId;
        }
#endif
    }
}