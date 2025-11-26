using Core.Game.Chunk.Room.Grid;
using Core.Game.Chunk.Room.Grid.Editor;
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
            if (_editor == null)
            {
                _editor = GetComponent<RoomGridEditor>();
                if (_editor == null)
                {
                    _editor = gameObject.AddComponent<RoomGridEditor>();
                }
            }

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

            Debug.Log($"[Test] 编辑器初始化完成: {config}");
            Debug.Log($"[Test] 默认模式: {_editor.State?.CurrentMode}");
            Debug.Log($"[Test] 提示: 按 2 进入地块编辑模式，然后点击绘制");
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

            // Q/E 切换地块类型
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CycleTileType(-1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                CycleTileType(1);
            }

            // Tab 切换工具
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CycleTileTool();
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