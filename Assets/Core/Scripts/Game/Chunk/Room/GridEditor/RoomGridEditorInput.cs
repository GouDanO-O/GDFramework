using System;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Game.Chunk.Room.Grid.Editor
{
    /// <summary>
    /// 编辑器输入处理
    /// 使用 Unity 原生 Input 作为主要输入源
    /// </summary>
    public class RoomGridEditorInput : ICanSendEvent
    {
        #region 事件

        /// <summary>
        /// 鼠标左键点击（单击，非拖拽）
        /// </summary>
        public event UnityAction<Vector3, TilePosition> OnLeftClick;

        /// <summary>
        /// 鼠标左键按下开始拖拽
        /// </summary>
        public event UnityAction<Vector3, TilePosition> OnLeftDragStart;

        /// <summary>
        /// 鼠标左键拖拽中
        /// </summary>
        public event UnityAction<Vector3, TilePosition> OnLeftDragging;

        /// <summary>
        /// 鼠标左键拖拽结束
        /// </summary>
        public event UnityAction<Vector3, TilePosition> OnLeftDragEnd;

        /// <summary>
        /// 鼠标右键点击
        /// </summary>
        public event UnityAction<Vector3, TilePosition> OnRightClick;

        /// <summary>
        /// 鼠标移动
        /// </summary>
        public event UnityAction<Vector3, TilePosition> OnMouseMove;

        /// <summary>
        /// 滚轮滚动
        /// </summary>
        public event UnityAction<float> OnScroll;

        /// <summary>
        /// 旋转快捷键
        /// </summary>
        public event UnityAction<bool> OnRotateKey;

        /// <summary>
        /// 删除快捷键
        /// </summary>
        public event UnityAction OnDeleteKey;

        /// <summary>
        /// 取消快捷键
        /// </summary>
        public event UnityAction OnCancelKey;

        #endregion

        #region 属性

        /// <summary>
        /// 是否启用输入
        /// </summary>
        public bool InputEnabled { get; set; } = true;

        /// <summary>
        /// 编辑器相机
        /// </summary>
        public Camera EditorCamera { get; set; }

        /// <summary>
        /// 网格配置（用于坐标转换）
        /// </summary>
        public RoomGridConfig GridConfig { get; set; }

        /// <summary>
        /// 地面高度
        /// </summary>
        public float GroundHeight { get; set; } = 0f;

        /// <summary>
        /// 当前鼠标世界坐标
        /// </summary>
        public Vector3 CurrentWorldPosition { get; private set; }

        /// <summary>
        /// 当前鼠标地块坐标
        /// </summary>
        public TilePosition CurrentTilePosition { get; private set; }

        /// <summary>
        /// 鼠标是否在有效区域
        /// </summary>
        public bool IsMouseInValidArea { get; private set; }

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        public bool IsDragging => _isDragging;

        #endregion

        #region 私有字段

        private IArchitecture _architecture;
        
        // 左键状态
        private bool _isLeftButtonDown;
        private bool _isDragging;
        private bool _hasFiredClick; // 防止重复触发点击
        private Vector2 _leftButtonDownMousePosition;
        private TilePosition _leftButtonDownTilePosition;
        private TilePosition _lastDragTilePosition;
        
        // 拖拽阈值（像素）
        private float _dragThreshold = 5f;

        #endregion

        #region 初始化

        public RoomGridEditorInput(IArchitecture architecture)
        {
            _architecture = architecture;
        }

        public IArchitecture GetArchitecture()
        {
            return _architecture;
        }

        /// <summary>
        /// 初始化输入系统
        /// </summary>
        public void Initialize(Camera camera, RoomGridConfig config)
        {
            EditorCamera = camera;
            GridConfig = config;
            
            Debug.Log("[EditorInput] 初始化完成");
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Dispose()
        {
            _isLeftButtonDown = false;
            _isDragging = false;
            _hasFiredClick = false;
        }

        #endregion

        #region Update

        /// <summary>
        /// 更新输入（需要每帧调用）
        /// </summary>
        public void Update()
        {
            if (!InputEnabled) 
                return;

            // 更新鼠标世界坐标
            UpdateMousePosition(Input.mousePosition);
            
            // 处理鼠标输入
            HandleMouseInput();
            
            // 处理键盘输入
            HandleKeyboardInput();
            
            // 触发鼠标移动事件
            OnMouseMove?.Invoke(CurrentWorldPosition, CurrentTilePosition);
        }

        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        private void HandleMouseInput()
        {
            // === 左键按下 ===
            if (Input.GetMouseButtonDown(0))
            {
                _isLeftButtonDown = true;
                _isDragging = false;
                _hasFiredClick = false;
                _leftButtonDownMousePosition = Input.mousePosition;
                _leftButtonDownTilePosition = CurrentTilePosition;
                _lastDragTilePosition = CurrentTilePosition;
            }
            
            // === 左键保持按住 ===
            if (Input.GetMouseButton(0) && _isLeftButtonDown)
            {
                if (!_isDragging)
                {
                    // 检查是否超过拖拽阈值
                    float distance = Vector2.Distance(Input.mousePosition, _leftButtonDownMousePosition);
                    if (distance > _dragThreshold)
                    {
                        // 开始拖拽
                        _isDragging = true;
                        
                        Debug.Log($"[EditorInput] 开始拖拽 - 起始: {_leftButtonDownTilePosition}");
                        OnLeftDragStart?.Invoke(
                            GridConfig.TileToWorld(_leftButtonDownTilePosition), 
                            _leftButtonDownTilePosition
                        );
                        
                        // 立即触发当前位置的拖拽事件
                        _lastDragTilePosition = _leftButtonDownTilePosition;
                        OnLeftDragging?.Invoke(CurrentWorldPosition, CurrentTilePosition);
                    }
                }
                else
                {
                    // 拖拽中 - 每次位置变化都触发
                    if (CurrentTilePosition != _lastDragTilePosition)
                    {
                        _lastDragTilePosition = CurrentTilePosition;
                        OnLeftDragging?.Invoke(CurrentWorldPosition, CurrentTilePosition);
                    }
                }
            }
            
            // === 左键释放 ===
            if (Input.GetMouseButtonUp(0))
            {
                if (_isLeftButtonDown)
                {
                    if (_isDragging)
                    {
                        // 拖拽结束
                        Debug.Log($"[EditorInput] 拖拽结束 - 终点: {CurrentTilePosition}");
                        OnLeftDragEnd?.Invoke(CurrentWorldPosition, CurrentTilePosition);
                    }
                    else if (!_hasFiredClick)
                    {
                        // 单击（未达到拖拽阈值且未触发过点击）
                        if (IsMouseInValidArea)
                        {
                            Debug.Log($"[EditorInput] 左键单击 - 位置: {CurrentTilePosition}");
                            OnLeftClick?.Invoke(CurrentWorldPosition, CurrentTilePosition);
                            _hasFiredClick = true;
                        }
                    }
                }
                
                _isLeftButtonDown = false;
                _isDragging = false;
            }

            // === 右键点击 ===
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log($"[EditorInput] 右键点击 - 位置: {CurrentTilePosition}");
                OnRightClick?.Invoke(CurrentWorldPosition, CurrentTilePosition);
            }

            // === 滚轮 ===
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                OnScroll?.Invoke(scroll * 10f);
            }
        }

        /// <summary>
        /// 处理键盘输入
        /// </summary>
        private void HandleKeyboardInput()
        {
            // R键 - 旋转
            if (Input.GetKeyDown(KeyCode.R))
            {
                bool clockwise = !Input.GetKey(KeyCode.LeftShift);
                OnRotateKey?.Invoke(clockwise);
            }

            // Delete/Backspace键 - 删除
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                OnDeleteKey?.Invoke();
            }

            // Escape键 - 取消
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnCancelKey?.Invoke();
            }

            // [ 和 ] 调整画笔大小
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                OnScroll?.Invoke(-1f);
            }
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                OnScroll?.Invoke(1f);
            }
        }

        #endregion

        #region 坐标转换

        /// <summary>
        /// 更新鼠标位置
        /// </summary>
        private void UpdateMousePosition(Vector2 screenPos)
        {
            CurrentWorldPosition = ScreenToWorldPosition(screenPos);
            CurrentTilePosition = WorldToTilePosition(CurrentWorldPosition);
            IsMouseInValidArea = GridConfig != null && GridConfig.IsInBounds(CurrentTilePosition);
        }

        /// <summary>
        /// 屏幕坐标转世界坐标
        /// </summary>
        public Vector3 ScreenToWorldPosition(Vector2 screenPos)
        {
            if (EditorCamera == null)
            {
                return Vector3.zero;
            }

            Ray ray = EditorCamera.ScreenPointToRay(screenPos);
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, GroundHeight, 0));
            
            if (groundPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// 世界坐标转地块坐标
        /// </summary>
        public TilePosition WorldToTilePosition(Vector3 worldPos)
        {
            if (GridConfig == null)
            {
                return TilePosition.Zero;
            }

            return GridConfig.WorldToTile(worldPos);
        }

        /// <summary>
        /// 地块坐标转世界坐标
        /// </summary>
        public Vector3 TileToWorldPosition(TilePosition tilePos)
        {
            if (GridConfig == null)
            {
                return Vector3.zero;
            }

            return GridConfig.TileToWorld(tilePos);
        }

        #endregion

        #region 射线检测

        /// <summary>
        /// 从屏幕位置发射射线检测
        /// </summary>
        public bool RaycastFromScreen(Vector2 screenPos, out RaycastHit hit, LayerMask layerMask)
        {
            hit = default;
            
            if (EditorCamera == null) return false;

            Ray ray = EditorCamera.ScreenPointToRay(screenPos);
            return Physics.Raycast(ray, out hit, 1000f, layerMask);
        }

        /// <summary>
        /// 从屏幕位置发射射线检测（使用当前鼠标位置）
        /// </summary>
        public bool RaycastFromMouse(out RaycastHit hit, LayerMask layerMask)
        {
            return RaycastFromScreen(Input.mousePosition, out hit, layerMask);
        }

        #endregion
    }
}