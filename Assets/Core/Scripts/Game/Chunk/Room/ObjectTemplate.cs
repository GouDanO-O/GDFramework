using System.Collections.Generic;
using Core.Game.Chunk.Room;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.RoomEditor
{
    /// <summary>
    /// 物体模板
    /// </summary>
    [System.Serializable]
    public class ObjectTemplate
    {
        [LabelText("模板名称")]
        public string TemplateName;
        
        [LabelText("模板描述")]
        [TextArea(2, 4)]
        public string Description;
        
        [LabelText("物体类型")]
        public EPlaceableObjectType ObjectType;
        
        [LabelText("预制体")]
        [AssetsOnly]
        public GameObject Prefab;
        
        [LabelText("预制体路径")]
        [ReadOnly]
        public string PrefabPath;
        
        [LabelText("占据尺寸(瓦片)")]
        [MinValue(1)]
        public Vector2Int Size = Vector2Int.one;
        
        [LabelText("阻挡移动")]
        public bool BlocksMovement = true;
        
        [LabelText("可旋转")]
        public bool Rotatable = true;
        
        [LabelText("预览图标")]
        [PreviewField(80)]
        public Sprite Icon;
        
        [Title("容器属性", TitleAlignment = TitleAlignments.Centered)]
        [ShowIf("IsContainer")]
        [LabelText("容器容量")]
        [MinValue(1)]
        public int ContainerCapacity = 10;
        
        [ShowIf("IsContainer")]
        [LabelText("容器类型")]
        public string ContainerType = "General";
        
        [Title("交互属性", TitleAlignment = TitleAlignments.Centered)]
        [ShowIf("IsInteractive")]
        [LabelText("交互类型")]
        public string InteractionType;
        
        [ShowIf("IsInteractive")]
        [LabelText("交互范围(瓦片)")]
        public int InteractionRange = 1;

        private bool IsContainer => ObjectType == EPlaceableObjectType.Container;
        private bool IsInteractive => ObjectType == EPlaceableObjectType.Interactive;

        /// <summary>
        /// 转换为运行时数据
        /// </summary>
        public PlaceableObjectData ToPlaceableData()
        {
            var data = new PlaceableObjectData
            {
                ObjectType = this.ObjectType,
                Size = this.Size,
                PrefabPath = this.PrefabPath,
                BlocksMovement = this.BlocksMovement,
                Properties = new Dictionary<string, string>()
            };

            // 添加特定属性
            if (IsContainer)
            {
                data.Properties["ContainerCapacity"] = ContainerCapacity.ToString();
                data.Properties["ContainerType"] = ContainerType;
            }

            if (IsInteractive)
            {
                data.Properties["InteractionType"] = InteractionType;
                data.Properties["InteractionRange"] = InteractionRange.ToString();
            }

            return data;
        }

        [Button("自动设置预制体路径")]
        private void AutoSetPrefabPath()
        {
            if (Prefab != null)
            {
#if UNITY_EDITOR
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(Prefab);
                if (assetPath.StartsWith("Assets/Resources/"))
                {
                    PrefabPath = assetPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
                }
                else
                {
                    Debug.LogWarning("预制体必须在 Resources 文件夹中!");
                }
#endif
            }
        }
    }

    /// <summary>
    /// 常用物体模板工厂
    /// </summary>
    public static class CommonObjectTemplates
    {
        /// <summary>
        /// 创建床模板
        /// </summary>
        public static ObjectTemplate CreateBed()
        {
            return new ObjectTemplate
            {
                TemplateName = "床",
                Description = "可供角色休息的床铺",
                ObjectType = EPlaceableObjectType.Furniture,
                Size = new Vector2Int(2, 3),
                BlocksMovement = true,
                Rotatable = true
            };
        }

        /// <summary>
        /// 创建桌子模板
        /// </summary>
        public static ObjectTemplate CreateTable()
        {
            return new ObjectTemplate
            {
                TemplateName = "桌子",
                Description = "普通的桌子",
                ObjectType = EPlaceableObjectType.Furniture,
                Size = new Vector2Int(2, 1),
                BlocksMovement = true,
                Rotatable = true
            };
        }

        /// <summary>
        /// 创建箱子模板
        /// </summary>
        public static ObjectTemplate CreateChest()
        {
            return new ObjectTemplate
            {
                TemplateName = "箱子",
                Description = "用于存储物品的箱子",
                ObjectType = EPlaceableObjectType.Container,
                Size = Vector2Int.one,
                BlocksMovement = true,
                Rotatable = false,
                ContainerCapacity = 20,
                ContainerType = "Storage"
            };
        }

        /// <summary>
        /// 创建衣柜模板
        /// </summary>
        public static ObjectTemplate CreateWardrobe()
        {
            return new ObjectTemplate
            {
                TemplateName = "衣柜",
                Description = "用于存储衣物的大型家具",
                ObjectType = EPlaceableObjectType.Container,
                Size = new Vector2Int(2, 1),
                BlocksMovement = true,
                Rotatable = true,
                ContainerCapacity = 50,
                ContainerType = "Clothing"
            };
        }

        /// <summary>
        /// 创建门模板
        /// </summary>
        public static ObjectTemplate CreateDoor()
        {
            return new ObjectTemplate
            {
                TemplateName = "门",
                Description = "可开关的门",
                ObjectType = EPlaceableObjectType.Interactive,
                Size = Vector2Int.one,
                BlocksMovement = false,
                Rotatable = true,
                InteractionType = "Door",
                InteractionRange = 1
            };
        }

        /// <summary>
        /// 创建椅子模板
        /// </summary>
        public static ObjectTemplate CreateChair()
        {
            return new ObjectTemplate
            {
                TemplateName = "椅子",
                Description = "可坐的椅子",
                ObjectType = EPlaceableObjectType.Interactive,
                Size = Vector2Int.one,
                BlocksMovement = true,
                Rotatable = true,
                InteractionType = "Sit",
                InteractionRange = 1
            };
        }

        /// <summary>
        /// 创建植物模板
        /// </summary>
        public static ObjectTemplate CreatePlant()
        {
            return new ObjectTemplate
            {
                TemplateName = "盆栽",
                Description = "装饰用的盆栽植物",
                ObjectType = EPlaceableObjectType.Decoration,
                Size = Vector2Int.one,
                BlocksMovement = false,
                Rotatable = false
            };
        }

        /// <summary>
        /// 创建灯模板
        /// </summary>
        public static ObjectTemplate CreateLamp()
        {
            return new ObjectTemplate
            {
                TemplateName = "台灯",
                Description = "提供照明的台灯",
                ObjectType = EPlaceableObjectType.LightSource,
                Size = Vector2Int.one,
                BlocksMovement = false,
                Rotatable = false
            };
        }

        /// <summary>
        /// 获取所有预设模板
        /// </summary>
        public static List<ObjectTemplate> GetAllPresetTemplates()
        {
            return new List<ObjectTemplate>
            {
                CreateBed(),
                CreateTable(),
                CreateChair(),
                CreateChest(),
                CreateWardrobe(),
                CreateDoor(),
                CreatePlant(),
                CreateLamp()
            };
        }
    }
}