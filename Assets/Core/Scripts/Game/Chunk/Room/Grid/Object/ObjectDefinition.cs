using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 物品放置类型
    /// </summary>
    public enum PlacementType
    {
        [LabelText("地面")]
        Floor = 0,
        
        [LabelText("墙壁")]
        Wall = 1,
        
        [LabelText("天花板")]
        Ceiling = 2,
        
        [LabelText("桌面")]
        Table = 3,
        
        [LabelText("悬挂")]
        Hanging = 4
    }

    /// <summary>
    /// 物品定义 - 描述一个物品的基本属性
    /// 对应JSON配置文件
    /// </summary>
    [Serializable]
    public class ObjectDefinition
    {
        #region 基础信息

        [Title("基础信息")]
        
        [LabelText("定义ID")]
        [JsonProperty("id")]
        public string Id;

        [LabelText("显示名称")]
        [JsonProperty("name")]
        public string Name;

        [LabelText("描述")]
        [TextArea(2, 4)]
        [JsonProperty("description")]
        public string Description;

        [LabelText("类别")]
        [JsonProperty("category")]
        public ObjectCategory Category = ObjectCategory.Furniture;

        [LabelText("标签")]
        [JsonProperty("tags")]
        public List<string> Tags = new List<string>();

        #endregion

        #region 尺寸与放置

        [Title("尺寸与放置")]
        
        [LabelText("尺寸")]
        [JsonProperty("size")]
        public ObjectSize Size = ObjectSize.One;

        [LabelText("放置类型")]
        [JsonProperty("placementType")]
        public PlacementType PlacementType = PlacementType.Floor;

        [LabelText("可旋转")]
        [JsonProperty("canRotate")]
        public bool CanRotate = true;

        [LabelText("允许叠放")]
        [PropertyTooltip("是否允许在此物品上放置其他物品")]
        [JsonProperty("allowStacking")]
        public bool AllowStacking = false;

        [LabelText("需要的地块类型")]
        [JsonProperty("requiredTileTypes")]
        public List<TileType> RequiredTileTypes = new List<TileType>();

        #endregion

        #region 视觉表现

        [Title("视觉表现")]
        
        [LabelText("预制体路径")]
        [JsonProperty("prefabPath")]
        public string PrefabPath;

        [LabelText("图标路径")]
        [JsonProperty("iconPath")]
        public string IconPath;

        [LabelText("预览缩放")]
        [JsonProperty("previewScale")]
        public float PreviewScale = 1f;

        [LabelText("Y轴偏移")]
        [PropertyTooltip("放置时的Y轴偏移")]
        [JsonProperty("yOffset")]
        public float YOffset = 0f;

        #endregion

        #region 交互设置

        [Title("交互设置")]
        
        [LabelText("可交互")]
        [JsonProperty("interactable")]
        public bool Interactable = false;

        [LabelText("交互类型")]
        [JsonProperty("interactionType")]
        [ShowIf("Interactable")]
        public string InteractionType;

        [LabelText("交互范围")]
        [JsonProperty("interactionRange")]
        [ShowIf("Interactable")]
        public float InteractionRange = 1.5f;

        #endregion

        #region 游戏数据

        [Title("游戏数据")]
        
        [LabelText("价格")]
        [JsonProperty("price")]
        public int Price = 100;

        [LabelText("出售价格")]
        [JsonProperty("sellPrice")]
        public int SellPrice = 50;

        [LabelText("稀有度")]
        [Range(1, 5)]
        [JsonProperty("rarity")]
        public int Rarity = 1;

        [LabelText("解锁条件")]
        [JsonProperty("unlockCondition")]
        public string UnlockCondition;

        #endregion

        #region 扩展数据

        [Title("扩展数据")]
        
        [LabelText("自定义属性")]
        [JsonProperty("customProperties")]
        public Dictionary<string, string> CustomProperties = new Dictionary<string, string>();

        #endregion

        #region 方法

        /// <summary>
        /// 检查是否可以放置在指定地块类型上
        /// </summary>
        public bool CanPlaceOnTileType(TileType tileType)
        {
            if (RequiredTileTypes == null || RequiredTileTypes.Count == 0)
                return true;
            
            return RequiredTileTypes.Contains(tileType);
        }

        /// <summary>
        /// 获取自定义属性
        /// </summary>
        public string GetProperty(string key, string defaultValue = null)
        {
            if (CustomProperties != null && CustomProperties.TryGetValue(key, out var value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// 获取自定义属性（整数）
        /// </summary>
        public int GetPropertyInt(string key, int defaultValue = 0)
        {
            var value = GetProperty(key);
            if (int.TryParse(value, out var result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// 获取自定义属性（浮点数）
        /// </summary>
        public float GetPropertyFloat(string key, float defaultValue = 0f)
        {
            var value = GetProperty(key);
            if (float.TryParse(value, out var result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// 是否有指定标签
        /// </summary>
        public bool HasTag(string tag)
        {
            return Tags != null && Tags.Contains(tag);
        }

        public override string ToString()
        {
            return $"ObjectDef[{Id}] {Name} Size:{Size} Category:{Category}";
        }

        #endregion
    }

    /// <summary>
    /// 物品定义集合 - 用于JSON序列化
    /// </summary>
    [Serializable]
    public class ObjectDefinitionCollection
    {
        [JsonProperty("version")]
        public string Version = "1.0";

        [JsonProperty("objects")]
        public List<ObjectDefinition> Objects = new List<ObjectDefinition>();
    }
}