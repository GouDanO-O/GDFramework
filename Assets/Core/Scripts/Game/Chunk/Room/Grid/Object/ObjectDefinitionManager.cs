using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 物品定义管理器
    /// 负责加载、缓存和查询物品定义
    /// </summary>
    public class ObjectDefinitionManager
    {
        #region 单例

        private static ObjectDefinitionManager _instance;
        public static ObjectDefinitionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObjectDefinitionManager();
                }
                return _instance;
            }
        }

        #endregion

        #region 数据

        /// <summary>
        /// 物品定义字典
        /// </summary>
        private Dictionary<string, ObjectDefinition> _definitions = new Dictionary<string, ObjectDefinition>();

        /// <summary>
        /// 按类别分类的定义
        /// </summary>
        private Dictionary<ObjectCategory, List<ObjectDefinition>> _definitionsByCategory = 
            new Dictionary<ObjectCategory, List<ObjectDefinition>>();

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 定义数量
        /// </summary>
        public int Count => _definitions.Count;

        #endregion

        #region 事件

        /// <summary>
        /// 定义加载完成事件
        /// </summary>
        public event UnityAction OnDefinitionsLoaded;

        #endregion

        #region 初始化

        private ObjectDefinitionManager()
        {
            // 初始化类别字典
            foreach (ObjectCategory category in Enum.GetValues(typeof(ObjectCategory)))
            {
                _definitionsByCategory[category] = new List<ObjectDefinition>();
            }
        }

        /// <summary>
        /// 从JSON字符串加载定义
        /// </summary>
        public void LoadFromJson(string json)
        {
            try
            {
                var collection = JsonConvert.DeserializeObject<ObjectDefinitionCollection>(json);
                if (collection?.Objects != null)
                {
                    foreach (var def in collection.Objects)
                    {
                        RegisterDefinition(def);
                    }
                }
                
                IsInitialized = true;
                OnDefinitionsLoaded?.Invoke();
                
                Debug.Log($"[ObjectDefManager] 加载了 {_definitions.Count} 个物品定义");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ObjectDefManager] JSON解析失败: {e.Message}");
            }
        }

        /// <summary>
        /// 从TextAsset加载定义
        /// </summary>
        public void LoadFromTextAsset(TextAsset textAsset)
        {
            if (textAsset == null)
            {
                Debug.LogError("[ObjectDefManager] TextAsset为空");
                return;
            }
            
            LoadFromJson(textAsset.text);
        }

        /// <summary>
        /// 注册单个定义
        /// </summary>
        public void RegisterDefinition(ObjectDefinition definition)
        {
            if (definition == null || string.IsNullOrEmpty(definition.Id))
            {
                Debug.LogWarning("[ObjectDefManager] 无效的物品定义");
                return;
            }

            if (_definitions.ContainsKey(definition.Id))
            {
                Debug.LogWarning($"[ObjectDefManager] 物品定义已存在，将覆盖: {definition.Id}");
            }

            _definitions[definition.Id] = definition;
            
            // 添加到类别列表
            if (!_definitionsByCategory[definition.Category].Contains(definition))
            {
                _definitionsByCategory[definition.Category].Add(definition);
            }
        }

        /// <summary>
        /// 清除所有定义
        /// </summary>
        public void Clear()
        {
            _definitions.Clear();
            foreach (var list in _definitionsByCategory.Values)
            {
                list.Clear();
            }
            IsInitialized = false;
        }

        /// <summary>
        /// 加载默认测试数据
        /// </summary>
        public void LoadDefaultTestData()
        {
            var testObjects = new List<ObjectDefinition>
            {
                // 家具
                new ObjectDefinition
                {
                    Id = "furniture_table_small",
                    Name = "小桌子",
                    Description = "一张小型木桌",
                    Category = ObjectCategory.Furniture,
                    Size = new ObjectSize(2, 2, 0.8f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    AllowStacking = true,
                    Price = 500,
                    Rarity = 1,
                    Tags = new List<string> { "table", "wood" }
                },
                new ObjectDefinition
                {
                    Id = "furniture_table_large",
                    Name = "大桌子",
                    Description = "一张大型木桌",
                    Category = ObjectCategory.Furniture,
                    Size = new ObjectSize(3, 2, 0.8f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    AllowStacking = true,
                    Price = 800,
                    Rarity = 2,
                    Tags = new List<string> { "table", "wood" }
                },
                new ObjectDefinition
                {
                    Id = "furniture_chair",
                    Name = "椅子",
                    Description = "一把普通椅子",
                    Category = ObjectCategory.Furniture,
                    Size = new ObjectSize(1, 1, 1f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    Interactable = true,
                    InteractionType = "sit",
                    Price = 200,
                    Rarity = 1,
                    Tags = new List<string> { "chair", "seating" }
                },
                new ObjectDefinition
                {
                    Id = "furniture_sofa",
                    Name = "沙发",
                    Description = "舒适的沙发",
                    Category = ObjectCategory.Furniture,
                    Size = new ObjectSize(3, 1, 1f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    Interactable = true,
                    InteractionType = "sit",
                    Price = 1200,
                    Rarity = 2,
                    Tags = new List<string> { "sofa", "seating" }
                },
                new ObjectDefinition
                {
                    Id = "furniture_bed_single",
                    Name = "单人床",
                    Description = "单人床铺",
                    Category = ObjectCategory.Furniture,
                    Size = new ObjectSize(1, 2, 0.6f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    Interactable = true,
                    InteractionType = "sleep",
                    Price = 800,
                    Rarity = 1,
                    Tags = new List<string> { "bed", "sleeping" }
                },
                new ObjectDefinition
                {
                    Id = "furniture_bed_double",
                    Name = "双人床",
                    Description = "宽敞的双人床",
                    Category = ObjectCategory.Furniture,
                    Size = new ObjectSize(2, 2, 0.6f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    Interactable = true,
                    InteractionType = "sleep",
                    Price = 1500,
                    Rarity = 2,
                    Tags = new List<string> { "bed", "sleeping" }
                },
                new ObjectDefinition
                {
                    Id = "furniture_wardrobe",
                    Name = "衣柜",
                    Description = "存放衣物的柜子",
                    Category = ObjectCategory.Storage,
                    Size = new ObjectSize(2, 1, 2f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    Interactable = true,
                    InteractionType = "storage",
                    Price = 1000,
                    Rarity = 2,
                    Tags = new List<string> { "wardrobe", "storage" }
                },
                new ObjectDefinition
                {
                    Id = "furniture_bookshelf",
                    Name = "书架",
                    Description = "放置书籍的架子",
                    Category = ObjectCategory.Storage,
                    Size = new ObjectSize(2, 1, 2f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    AllowStacking = false,
                    Price = 600,
                    Rarity = 1,
                    Tags = new List<string> { "bookshelf", "storage" }
                },
                
                // 装饰
                new ObjectDefinition
                {
                    Id = "decoration_rug_small",
                    Name = "小地毯",
                    Description = "小型装饰地毯",
                    Category = ObjectCategory.Decoration,
                    Size = new ObjectSize(2, 2, 0.02f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    YOffset = 0.01f,
                    Price = 300,
                    Rarity = 1,
                    Tags = new List<string> { "rug", "floor" }
                },
                new ObjectDefinition
                {
                    Id = "decoration_rug_large",
                    Name = "大地毯",
                    Description = "大型装饰地毯",
                    Category = ObjectCategory.Decoration,
                    Size = new ObjectSize(4, 3, 0.02f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    YOffset = 0.01f,
                    Price = 600,
                    Rarity = 2,
                    Tags = new List<string> { "rug", "floor" }
                },
                new ObjectDefinition
                {
                    Id = "decoration_vase",
                    Name = "花瓶",
                    Description = "精美的花瓶",
                    Category = ObjectCategory.Decoration,
                    Size = new ObjectSize(1, 1, 0.5f),
                    PlacementType = PlacementType.Table,
                    CanRotate = false,
                    Price = 200,
                    Rarity = 1,
                    Tags = new List<string> { "vase", "tabletop" }
                },
                
                // 植物
                new ObjectDefinition
                {
                    Id = "plant_potted_small",
                    Name = "小盆栽",
                    Description = "小型盆栽植物",
                    Category = ObjectCategory.Plant,
                    Size = new ObjectSize(1, 1, 0.5f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = false,
                    Price = 150,
                    Rarity = 1,
                    Tags = new List<string> { "plant", "potted" }
                },
                new ObjectDefinition
                {
                    Id = "plant_tree_indoor",
                    Name = "室内树",
                    Description = "大型室内植物",
                    Category = ObjectCategory.Plant,
                    Size = new ObjectSize(1, 1, 2f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = false,
                    Price = 500,
                    Rarity = 2,
                    Tags = new List<string> { "plant", "tree" }
                },
                
                // 照明
                new ObjectDefinition
                {
                    Id = "lighting_lamp_floor",
                    Name = "落地灯",
                    Description = "落地式台灯",
                    Category = ObjectCategory.Lighting,
                    Size = new ObjectSize(1, 1, 1.5f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = false,
                    Interactable = true,
                    InteractionType = "toggle",
                    Price = 400,
                    Rarity = 1,
                    Tags = new List<string> { "lamp", "lighting" }
                },
                new ObjectDefinition
                {
                    Id = "lighting_lamp_table",
                    Name = "台灯",
                    Description = "桌面台灯",
                    Category = ObjectCategory.Lighting,
                    Size = new ObjectSize(1, 1, 0.4f),
                    PlacementType = PlacementType.Table,
                    CanRotate = false,
                    Interactable = true,
                    InteractionType = "toggle",
                    Price = 250,
                    Rarity = 1,
                    Tags = new List<string> { "lamp", "lighting", "tabletop" }
                },
                
                // 交互点
                new ObjectDefinition
                {
                    Id = "interactive_door",
                    Name = "门",
                    Description = "可开关的门",
                    Category = ObjectCategory.Interactive,
                    Size = new ObjectSize(1, 1, 2.2f),
                    PlacementType = PlacementType.Floor,
                    CanRotate = true,
                    Interactable = true,
                    InteractionType = "door",
                    Price = 300,
                    Rarity = 1,
                    Tags = new List<string> { "door", "passage" }
                }
            };

            foreach (var def in testObjects)
            {
                RegisterDefinition(def);
            }

            IsInitialized = true;
            OnDefinitionsLoaded?.Invoke();
            
            Debug.Log($"[ObjectDefManager] 加载了 {_definitions.Count} 个测试物品定义");
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取物品定义
        /// </summary>
        public ObjectDefinition GetDefinition(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
                
            _definitions.TryGetValue(id, out var def);
            return def;
        }

        /// <summary>
        /// 获取所有定义
        /// </summary>
        public IEnumerable<ObjectDefinition> GetAllDefinitions()
        {
            return _definitions.Values;
        }

        /// <summary>
        /// 获取指定类别的所有定义
        /// </summary>
        public List<ObjectDefinition> GetDefinitionsByCategory(ObjectCategory category)
        {
            if (_definitionsByCategory.TryGetValue(category, out var list))
                return list;
            return new List<ObjectDefinition>();
        }

        /// <summary>
        /// 搜索物品定义
        /// </summary>
        public List<ObjectDefinition> Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return _definitions.Values.ToList();

            keyword = keyword.ToLower();
            return _definitions.Values
                .Where(d => d.Name.ToLower().Contains(keyword) || 
                           d.Id.ToLower().Contains(keyword) ||
                           (d.Description != null && d.Description.ToLower().Contains(keyword)))
                .ToList();
        }

        /// <summary>
        /// 通过标签查找
        /// </summary>
        public List<ObjectDefinition> GetByTag(string tag)
        {
            return _definitions.Values
                .Where(d => d.HasTag(tag))
                .ToList();
        }

        /// <summary>
        /// 检查定义是否存在
        /// </summary>
        public bool HasDefinition(string id)
        {
            return !string.IsNullOrEmpty(id) && _definitions.ContainsKey(id);
        }

        #endregion

        #region 导出

        /// <summary>
        /// 导出为JSON
        /// </summary>
        public string ExportToJson(bool formatted = true)
        {
            var collection = new ObjectDefinitionCollection
            {
                Version = "1.0",
                Objects = _definitions.Values.ToList()
            };

            return JsonConvert.SerializeObject(collection, formatted ? Formatting.Indented : Formatting.None);
        }

        #endregion
    }
}