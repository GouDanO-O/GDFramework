using System;

namespace Core.Game.Storage.Data
{
    /// <summary>
    /// 存档槽位数据
    /// </summary>
    [Serializable]
    public class SaveSlotData
    {
        /// <summary>
        /// 存档槽位
        /// 按照顺序递增
        /// </summary>
        public int SlotIndex;

        /// <summary>
        /// 存档昵称
        /// </summary>
        public string SlotName;

        /// <summary>
        /// 宇宙ID--也可以理解为SlotID
        /// 每个存档只会有唯一的宇宙ID
        /// 这个宇宙不同存档也不会重复
        /// </summary>
        public string UniverseId;

        /// <summary>
        /// 上次存档时间
        /// </summary>
        public DateTime LastSaveTime;

        /// <summary>
        /// 游戏时长
        /// </summary>
        public string PlayTime;

        public SaveSlotData(int slotIndex)
        {
            SlotIndex = slotIndex;
            SlotName = "默认存档_" + slotIndex;
            UniverseId = string.Empty;
            PlayTime = "0";
        }
    }
}