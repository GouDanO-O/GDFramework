using Core.Game.Chunk.Room.Grid;
using Core.Game.Chunk.Room.Grid.Editor;
using Core.Game.Chunk.Room.Grid.Renderer;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Test
{
    /// <summary>
    /// 房间编辑器测试脚本
    /// 用于快速测试编辑器功能
    /// </summary>
    public class RoomGridEditorTest : MonoBehaviour, IController
    {
        [Title("编辑器设置")]
        
        [LabelText("网格宽度")]
        [SerializeField]
        private int _gridWidth = 50;

        [LabelText("网格深度")]
        [SerializeField]
        private int _gridDepth = 50;

        [LabelText("地块大小(米)")]
        [SerializeField]
        private float _tileSize = 1f;

        [LabelText("默认地块类型")]
        [SerializeField]
        private TileType _defaultTileType = TileType.Grass;

        [LabelText("自动填充")]
        [SerializeField]
        private bool _autoFill = true;

        [Title("组件引用")]
        
        [LabelText("编辑器")]
        [SerializeField]
        private RoomGridEditor _editor;

        [LabelText("编辑器相机")]
        [SerializeField]
        private RoomGridEditorCamera _editorCamera;

        [LabelText("地块渲染器")]
        [SerializeField]
        private TileRenderer _tileRenderer;

        [LabelText("预览渲染器")]
        [SerializeField]
        private PreviewRenderer _previewRenderer;

        [LabelText("物品渲染器")]
        [SerializeField]
        private ObjectRenderer _objectRenderer;

        [Title("调试信息")]
        
        [LabelText("当前模式")]
        [ReadOnly]
        [ShowInInspector]
        private string _currentMode;

        [LabelText("鼠标位置")]
        [ReadOnly]
        [ShowInInspector]
        private string _mousePosition;

        [LabelText("统计信息")]
        [ReadOnly]
        [ShowInInspector]
        private string _statistics;

        [LabelText("选中物品")]
        [ReadOnly]
        [ShowInInspector]
        private string _selectedObjectId;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            InitializeEditor();
        }

        private void Update()
        {
            UpdateDebugInfo();
            HandleTestInput();
        }

        /// <summary>
        /// 初始化编辑器
        /// </summary>
        [Button("初始化编辑器", ButtonSizes.Large)]
        private void InitializeEditor()
        {
            // 获取或创建编辑器组件
            if (_editor == null)
            {
                _editor = GetComponent<RoomGridEditor>();
                if (_editor == null)
                {
                    _editor = gameObject.AddComponent<RoomGridEditor>();
                }
            }

            // 获取或创建编辑器相机
            if (_editorCamera == null)
            {
                _editorCamera = GetComponentInChildren<RoomGridEditorCamera>();
                if (_editorCamera == null)
                {
                    _editorCamera = FindFirstObjectByType<RoomGridEditorCamera>();
                }
                // 如果还是没有，RoomGridEditor.AutoSetupComponents会自动创建
            }

            // 获取或创建地块渲染器
            if (_tileRenderer == null)
            {
                _tileRenderer = GetComponent<TileRenderer>();
                if (_tileRenderer == null)
                {
                    _tileRenderer = gameObject.AddComponent<TileRenderer>();
                }
            }

            // 获取或创建预览渲染器
            if (_previewRenderer == null)
            {
                _previewRenderer = GetComponent<PreviewRenderer>();
                if (_previewRenderer == null)
                {
                    _previewRenderer = gameObject.AddComponent<PreviewRenderer>();
                }
            }

            // 获取或创建物品渲染器
            if (_objectRenderer == null)
            {
                _objectRenderer = GetComponent<ObjectRenderer>();
                if (_objectRenderer == null)
                {
                    _objectRenderer = gameObject.AddComponent<ObjectRenderer>();
                }
            }

            // 加载物品定义
            ObjectDefinitionManager.Instance.LoadDefaultTestData();

            // 创建配置
            var config = new RoomGridConfig
            {
                Width = _gridWidth,
                Depth = _gridDepth,
                TileSize = _tileSize,
                DefaultTileType = _defaultTileType,
                AutoFill = _autoFill
            };

            // 初始化编辑器
            _editor.Initialize(config);
            
            // 订阅事件用于调试
            _editor.OnTileModified += (pos, tile) => 
            {
                Debug.Log($"[Test] 地块修改: {pos} -> {tile?.Type}");
            };

            _editor.OnObjectPlaced += (obj) =>
            {
                Debug.Log($"[Test] 物品放置: {obj.InstanceId} ({obj.ObjectDefId})");
            };

            _editor.OnObjectRemoved += (obj) =>
            {
                Debug.Log($"[Test] 物品移除: {obj.InstanceId}");
            };

            Debug.Log($"[Test] 编辑器初始化完成: {config}");
            Debug.Log($"[Test] 默认模式: {_editor.State?.CurrentMode}");
            Debug.Log($"[Test] 提示: 按 2 进入地块编辑模式，按 3 进入物品放置模式");
            Debug.Log($"[Test] 物品定义数量: {ObjectDefinitionManager.Instance.Count}");
        }

        /// <summary>
        /// 更新调试信息
        /// </summary>
        private void UpdateDebugInfo()
        {
            if (_editor == null || !_editor.IsInitialized) return;

            _currentMode = _editor.State?.GetStatusSummary() ?? "未初始化";
            
            var pos = _editor.State?.CurrentMouseTilePosition ?? TilePosition.Zero;
            var valid = _editor.State?.IsMouseInValidArea ?? false;
            _mousePosition = valid ? $"({pos.X}, {pos.Z})" : "无效";

            var stats = _editor.GetStatistics();
            _statistics = stats.ToString();

            _selectedObjectId = _editor.State?.SelectedObjectDefId ?? "无";
        }

        /// <summary>
        /// 处理测试输入
        /// </summary>
        private void HandleTestInput()
        {
            if (_editor == null || !_editor.IsInitialized) return;

            // 数字键快速切换模式
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _editor.SetMode(EditorMode.View);
                Debug.Log("[Test] 切换到查看模式");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _editor.SetMode(EditorMode.TileEdit);
                Debug.Log("[Test] 切换到地块编辑模式");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _editor.SetMode(EditorMode.ObjectPlace);
                // 自动选择一个物品
                if (string.IsNullOrEmpty(_editor.State?.SelectedObjectDefId))
                {
                    SelectNextObject(1);
                }
                Debug.Log("[Test] 切换到物品放置模式");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _editor.SetMode(EditorMode.ObjectSelect);
                Debug.Log("[Test] 切换到选择模式");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _editor.SetMode(EditorMode.Delete);
                Debug.Log("[Test] 切换到删除模式");
            }

            // Q/E 切换地块类型或物品
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_editor.State?.CurrentMode == EditorMode.TileEdit)
                {
                    CycleTileType(-1);
                }
                else if (_editor.State?.CurrentMode == EditorMode.ObjectPlace)
                {
                    SelectNextObject(-1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (_editor.State?.CurrentMode == EditorMode.TileEdit)
                {
                    CycleTileType(1);
                }
                else if (_editor.State?.CurrentMode == EditorMode.ObjectPlace)
                {
                    SelectNextObject(1);
                }
            }

            // Tab 切换工具或物品类别
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_editor.State?.CurrentMode == EditorMode.TileEdit)
                {
                    CycleTileTool();
                }
                else if (_editor.State?.CurrentMode == EditorMode.ObjectPlace)
                {
                    CycleObjectCategory();
                }
            }

            // F5 快速保存
            if (Input.GetKeyDown(KeyCode.F5))
            {
                QuickSave();
            }

            // F9 快速加载
            if (Input.GetKeyDown(KeyCode.F9))
            {
                QuickLoad();
            }
        }

        private ObjectCategory _currentObjectCategory = ObjectCategory.Furniture;
        private int _currentObjectIndex = 0;

        /// <summary>
        /// 选择下一个物品
        /// </summary>
        private void SelectNextObject(int direction)
        {
            var defManager = ObjectDefinitionManager.Instance;
            var objectsInCategory = defManager.GetDefinitionsByCategory(_currentObjectCategory);
            
            if (objectsInCategory.Count == 0)
            {
                // 如果当前类别没有物品，尝试切换类别
                CycleObjectCategory();
                return;
            }

            _currentObjectIndex = (_currentObjectIndex + direction + objectsInCategory.Count) % objectsInCategory.Count;
            var selectedObj = objectsInCategory[_currentObjectIndex];
            
            _editor.StartPlaceObject(selectedObj.Id);
            Debug.Log($"[Test] 选中物品: {selectedObj.Name} ({selectedObj.Id})");
        }

        /// <summary>
        /// 循环切换物品类别
        /// </summary>
        private void CycleObjectCategory()
        {
            var categories = new[] 
            { 
                ObjectCategory.Furniture, 
                ObjectCategory.Decoration, 
                ObjectCategory.Plant, 
                ObjectCategory.Lighting,
                ObjectCategory.Storage
            };

            int currentIndex = System.Array.IndexOf(categories, _currentObjectCategory);
            currentIndex = (currentIndex + 1) % categories.Length;
            _currentObjectCategory = categories[currentIndex];
            _currentObjectIndex = 0;

            // 选择该类别的第一个物品
            var defManager = ObjectDefinitionManager.Instance;
            var objectsInCategory = defManager.GetDefinitionsByCategory(_currentObjectCategory);
            
            if (objectsInCategory.Count > 0)
            {
                _editor.StartPlaceObject(objectsInCategory[0].Id);
                Debug.Log($"[Test] 类别: {_currentObjectCategory}, 物品: {objectsInCategory[0].Name}");
            }
            else
            {
                Debug.Log($"[Test] 类别: {_currentObjectCategory} (无物品)");
            }
        }

        /// <summary>
        /// 快速保存
        /// </summary>
        private void QuickSave()
        {
            if (_editor?.Grid == null) return;
            
            bool success = RoomGridSaveSystem.Instance.QuickSave(_editor.Grid);
            Debug.Log(success ? "[Test] 快速保存成功" : "[Test] 快速保存失败");
        }

        /// <summary>
        /// 快速加载
        /// </summary>
        private void QuickLoad()
        {
            var saves = RoomGridSaveSystem.Instance.GetAllSaves();
            if (saves.Length == 0)
            {
                Debug.Log("[Test] 没有存档可加载");
                return;
            }

            // 加载最新的存档
            var latestSave = saves[0];
            var fullSaveData = RoomGridSaveSystem.Instance.Load(latestSave.SaveName);
            
            if (fullSaveData != null)
            {
                var grid = RoomGridSaveSystem.Instance.RestoreGrid(fullSaveData);
                if (grid != null)
                {
                    _editor.InitializeWithGrid(grid);
                    Debug.Log($"[Test] 加载存档成功: {latestSave.SaveName}");
                }
            }
        }

        /// <summary>
        /// 循环切换地块类型
        /// </summary>
        private void CycleTileType(int direction)
        {
            if (_editor?.State == null) return;

            var types = new[] 
            { 
                TileType.Grass, 
                TileType.Dirt, 
                TileType.Stone, 
                TileType.Wood, 
                TileType.Sand, 
                TileType.Water 
            };

            int currentIndex = System.Array.IndexOf(types, _editor.State.SelectedTileType);
            currentIndex = (currentIndex + direction + types.Length) % types.Length;
            
            _editor.SetTileType(types[currentIndex]);
            Debug.Log($"[Test] 地块类型: {types[currentIndex]}");
        }

        /// <summary>
        /// 循环切换工具
        /// </summary>
        private void CycleTileTool()
        {
            if (_editor?.State == null) return;

            var tools = new[] 
            { 
                TileEditTool.Brush, 
                TileEditTool.Fill, 
                TileEditTool.Rectangle, 
                TileEditTool.Eraser 
            };

            int currentIndex = System.Array.IndexOf(tools, _editor.State.CurrentTileTool);
            currentIndex = (currentIndex + 1) % tools.Length;
            
            _editor.SetTileTool(tools[currentIndex]);
            Debug.Log($"[Test] 工具: {tools[currentIndex]}");
        }

        #region Inspector按钮

        [Title("测试操作")]

        [Button("切换到地块编辑")]
        private void SwitchToTileEdit()
        {
            _editor?.SetMode(EditorMode.TileEdit);
        }

        [Button("切换到物品放置")]
        private void SwitchToObjectPlace()
        {
            _editor?.SetMode(EditorMode.ObjectPlace);
            if (string.IsNullOrEmpty(_editor?.State?.SelectedObjectDefId))
            {
                SelectNextObject(1);
            }
        }

        [ButtonGroup("Tools")]
        [Button("画笔")]
        private void SetBrushTool()
        {
            _editor?.SetTileTool(TileEditTool.Brush);
        }

        [ButtonGroup("Tools")]
        [Button("填充")]
        private void SetFillTool()
        {
            _editor?.SetTileTool(TileEditTool.Fill);
        }

        [ButtonGroup("Tools")]
        [Button("矩形")]
        private void SetRectTool()
        {
            _editor?.SetTileTool(TileEditTool.Rectangle);
        }

        [ButtonGroup("Tools")]
        [Button("橡皮擦")]
        private void SetEraserTool()
        {
            _editor?.SetTileTool(TileEditTool.Eraser);
        }

        [ButtonGroup("TileTypes")]
        [Button("草地")]
        private void SetGrass()
        {
            _editor?.SetTileType(TileType.Grass);
        }

        [ButtonGroup("TileTypes")]
        [Button("石板")]
        private void SetStone()
        {
            _editor?.SetTileType(TileType.Stone);
        }

        [ButtonGroup("TileTypes")]
        [Button("木板")]
        private void SetWood()
        {
            _editor?.SetTileType(TileType.Wood);
        }

        [ButtonGroup("TileTypes")]
        [Button("水")]
        private void SetWater()
        {
            _editor?.SetTileType(TileType.Water);
        }

        [Title("物品操作")]

        [ButtonGroup("Objects")]
        [Button("桌子")]
        private void PlaceTable()
        {
            _editor?.StartPlaceObject("furniture_table_small");
            _editor?.SetMode(EditorMode.ObjectPlace);
        }

        [ButtonGroup("Objects")]
        [Button("椅子")]
        private void PlaceChair()
        {
            _editor?.StartPlaceObject("furniture_chair");
            _editor?.SetMode(EditorMode.ObjectPlace);
        }

        [ButtonGroup("Objects")]
        [Button("沙发")]
        private void PlaceSofa()
        {
            _editor?.StartPlaceObject("furniture_sofa");
            _editor?.SetMode(EditorMode.ObjectPlace);
        }

        [ButtonGroup("Objects")]
        [Button("床")]
        private void PlaceBed()
        {
            _editor?.StartPlaceObject("furniture_bed_single");
            _editor?.SetMode(EditorMode.ObjectPlace);
        }

        [Title("楼层操作")]

        [Button("清空当前楼层", ButtonSizes.Medium)]
        private void ClearCurrentFloor()
        {
            if (_editor?.Grid == null) return;
            
            _editor.Grid.ClearFloor(_editor.State.CurrentFloor);
            Debug.Log("[Test] 清空当前楼层");
        }

        [Button("重新填充当前楼层", ButtonSizes.Medium)]
        private void RefillCurrentFloor()
        {
            if (_editor?.Grid == null) return;
            
            _editor.Grid.FillFloor(_editor.State.CurrentFloor, _defaultTileType);
            Debug.Log("[Test] 重新填充当前楼层");
        }

        [Title("存档操作")]

        [Button("保存到JSON")]
        private void SaveToJson()
        {
            if (_editor == null) return;
            
            string json = _editor.SaveToJson();
            Debug.Log($"[Test] 保存的JSON长度: {json?.Length ?? 0}");
            
            // 保存到PlayerPrefs用于测试
            PlayerPrefs.SetString("RoomGridTest", json);
            PlayerPrefs.Save();
        }

        [Button("从JSON加载")]
        private void LoadFromJson()
        {
            string json = PlayerPrefs.GetString("RoomGridTest", "");
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[Test] 没有保存的数据");
                return;
            }

            _editor?.LoadFromJson(json);
            Debug.Log("[Test] 加载完成");
        }

        [Button("保存到文件")]
        private void SaveToFile()
        {
            if (_editor?.Grid == null) return;
            
            string saveName = $"Room_{System.DateTime.Now:yyyyMMdd_HHmmss}";
            bool success = RoomGridSaveSystem.Instance.Save(_editor.Grid, saveName);
            
            if (success)
            {
                Debug.Log($"[Test] 保存成功: {saveName}");
            }
        }

        [Button("显示所有存档")]
        private void ShowAllSaves()
        {
            var saves = RoomGridSaveSystem.Instance.GetAllSaves();
            Debug.Log($"[Test] 共有 {saves.Length} 个存档:");
            foreach (var save in saves)
            {
                Debug.Log($"  - {save.SaveName} ({save.LastModifiedTime})");
            }
        }

        [Button("加载最新存档")]
        private void LoadLatestSave()
        {
            QuickLoad();
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            // 绘制网格边界
            if (_editor == null || _editor.Grid?.Config == null) return;

            var config = _editor.Grid.Config;
            var bounds = config.GetWorldBounds();

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        #endregion
    }
}