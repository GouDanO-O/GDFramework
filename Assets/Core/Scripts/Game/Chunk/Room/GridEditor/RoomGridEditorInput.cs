using System;
using GDFramework.Input;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid.Editor
{
    /// <summary>
    /// 编辑器输入处理
    /// 对接 NewInputManager，处理编辑器相关的输入
    /// </summary>
    public class RoomGridEditorInput : ICanRegisterEvent, ICanSendEvent
    {
        #region 事件

        /// <summary>
        /// 鼠标左键点击
        /// </summary>
        public event Action<Vector3, TilePosition> OnLeftClick;

        /// <summary>
        /// 鼠标左键按下开始拖拽
        /// </summary>
        public event Action<Vector3, TilePosition> OnLeftDragStart;

        /// <summary>
        /// 鼠标左键拖拽中
        /// </summary>
        public event Action<Vector3, TilePosition> OnLeftDragging;

        /// <summary>
        /// 鼠标左键拖拽结束
        /// </summary>
        public event Action<Vector3, TilePosition> OnLeftDragEnd;

        /// <summary>
        /// 鼠标右键点击
        /// </summary>
        public event Action<Vector3, TilePosition> OnRightClick;

        /// <summary>
        /// 鼠标移动
        /// </summary>
        public event Action<Vector3, TilePosition> OnMouseMove;

        /// <summary>
        /// 滚轮滚动
        /// </summary>
        public event Action<float> OnScroll;

        /// <summary>
        /// 旋转快捷键
        /// </summary>
        public event Action<bool> OnRotateKey;

        /// <summary>
        /// 删除快捷键
        /// </summary>
        public event System.Action OnDeleteKey;

        /// <summary>
        /// 取消快捷键
        /// </summary>
        public event System.Action OnCancelKey;

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

        #endregion

        #region 私有字段

        private IArchitecture _architecture;
        private bool _isLeftButtonDown;
        private bool _isDragging;
        private Vector2 _lastMousePosition;
        private Vector2 _dragStartMousePosition;
        private TilePosition _lastTilePosition;
        private float _dragThreshold = 5f; // 拖拽阈值（像素）

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
            
            RegisterEvents();
            Debug.Log("[EditorInput] 初始化完成");
        }

        /// <summary>
        /// 注册输入事件
        /// </summary>
        private void RegisterEvents()
        {
            // 鼠标左键
            this.RegisterEvent<SInputEvent_MouseLeftClick>(OnMouseLeftClickEvent);
            
            // 鼠标右键
            this.RegisterEvent<SInputEvent_MouseRightClick>(OnMouseRightClickEvent);
            
            // 鼠标移动/拖拽
            this.RegisterEvent<SInputEvent_MouseDrag>(OnMouseDragEvent);
            
            // 鼠标中键
            this.RegisterEvent<SInputEvent_MouseMiddleDown>(OnMouseMiddleDownEvent);
            this.RegisterEvent<SInputEvent_MouseMiddleUp>(OnMouseMiddleUpEvent);
            
            // 滚轮
            this.RegisterEvent<SInputEvent_MouseMiddleScroll>(OnMouseScrollEvent);
        }

        /// <summary>
        /// 注销输入事件
        /// </summary>
        public void UnregisterEvents()
        {
            this.UnRegisterEvent<SInputEvent_MouseLeftClick>(OnMouseLeftClickEvent);
            this.UnRegisterEvent<SInputEvent_MouseRightClick>(OnMouseRightClickEvent);
            this.UnRegisterEvent<SInputEvent_MouseDrag>(OnMouseDragEvent);
            this.UnRegisterEvent<SInputEvent_MouseMiddleDown>(OnMouseMiddleDownEvent);
            this.UnRegisterEvent<SInputEvent_MouseMiddleUp>(OnMouseMiddleUpEvent);
            this.UnRegisterEvent<SInputEvent_MouseMiddleScroll>(OnMouseScrollEvent);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Dispose()
        {
            UnregisterEvents();
        }

        #endregion

        #region 事件处理

        private void OnMouseLeftClickEvent(SInputEvent_MouseLeftClick evt)
        {
            if (!InputEnabled) return;

            // 使用 Unity 的 Input.mousePosition 获取当前鼠标位置
            Vector2 mousePos = Input.mousePosition;
            UpdateMousePosition(mousePos);

            Debug.Log($"[EditorInput] 左键点击 - 屏幕位置: {mousePos}, 世界位置: {CurrentWorldPosition}, 地块位置: {CurrentTilePosition}, 有效: {IsMouseInValidArea}");

            if (!IsMouseInValidArea)
            {
                Debug.Log("[EditorInput] 点击位置无效，忽略");
                return;
            }

            // 触发点击事件
            OnLeftClick?.Invoke(CurrentWorldPosition, CurrentTilePosition);
        }

        private void OnMouseRightClickEvent(SInputEvent_MouseRightClick evt)
        {
            if (!InputEnabled) return;

            Vector2 mousePos = Input.mousePosition;
            UpdateMousePosition(mousePos);

            Debug.Log($"[EditorInput] 右键点击 - 地块位置: {CurrentTilePosition}");

            OnRightClick?.Invoke(CurrentWorldPosition, CurrentTilePosition);
        }

        private void OnMouseDragEvent(SInputEvent_MouseDrag evt)
        {
            if (!InputEnabled) return;

            Vector2 mousePos = evt.mousePos;
            
            // 如果是零向量，使用 Input.mousePosition
            if (mousePos == Vector2.zero)
            {
                mousePos = Input.mousePosition;
            }

            UpdateMousePosition(mousePos);
            
            // 触发鼠标移动事件
            OnMouseMove?.Invoke(CurrentWorldPosition, CurrentTilePosition);

            // 处理左键拖拽（使用 Unity Input 检测按键状态）
            if (Input.GetMouseButton(0)) // 左键按住
            {
                if (!_isLeftButtonDown)
                {
                    // 开始按住
                    _isLeftButtonDown = true;
                    _dragStartMousePosition = mousePos;
                    _lastTilePosition = CurrentTilePosition;
                    
                    Debug.Log($"[EditorInput] 开始按住左键 - 位置: {CurrentTilePosition}");
                }
                else if (!_isDragging)
                {
                    // 检查是否超过拖拽阈值
                    float distance = Vector2.Distance(mousePos, _dragStartMousePosition);
                    if (distance > _dragThreshold)
                    {
                        _isDragging = true;
                        
                        // 转换起始位置
                        var startWorldPos = ScreenToWorldPosition(_dragStartMousePosition);
                        var startTilePos = WorldToTilePosition(startWorldPos);
                        
                        Debug.Log($"[EditorInput] 开始拖拽 - 起始位置: {startTilePos}");
                        OnLeftDragStart?.Invoke(startWorldPos, startTilePos);
                    }
                }
                else
                {
                    // 拖拽中 - 只有位置变化时才触发
                    if (CurrentTilePosition != _lastTilePosition)
                    {
                        _lastTilePosition = CurrentTilePosition;
                        OnLeftDragging?.Invoke(CurrentWorldPosition, CurrentTilePosition);
                    }
                }
            }
            else
            {
                // 左键释放
                if (_isDragging)
                {
                    Debug.Log($"[EditorInput] 拖拽结束 - 位置: {CurrentTilePosition}");
                    _isDragging = false;
                    OnLeftDragEnd?.Invoke(CurrentWorldPosition, CurrentTilePosition);
                }
                _isLeftButtonDown = false;
            }

            _lastMousePosition = mousePos;
        }

        private void OnMouseMiddleDownEvent(SInputEvent_MouseMiddleDown evt)
        {
            // 中键按下，可用于相机拖拽（由相机控制器处理）
        }

        private void OnMouseMiddleUpEvent(SInputEvent_MouseMiddleUp evt)
        {
            // 中键释放
        }

        private void OnMouseScrollEvent(SInputEvent_MouseMiddleScroll evt)
        {
            if (!InputEnabled) return;

            float scrollValue = evt.scrollValue.y;
            OnScroll?.Invoke(scrollValue);
        }

        #endregion

        #region Update（需要在MonoBehaviour中调用）

        /// <summary>
        /// 更新输入（需要每帧调用）
        /// </summary>
        public void Update()
        {
            if (!InputEnabled) return;

            // 持续更新鼠标位置
            UpdateMousePosition(Input.mousePosition);
            
            // 更新键盘输入
            UpdateKeyboardInput();
        }

        /// <summary>
        /// 更新键盘输入
        /// </summary>
        public void UpdateKeyboardInput()
        {
            if (!InputEnabled) return;

            // R键 - 旋转
            if (Input.GetKeyDown(KeyCode.R))
            {
                bool clockwise = !Input.GetKey(KeyCode.LeftShift);
                OnRotateKey?.Invoke(clockwise);
            }

            // Delete键 - 删除
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                OnDeleteKey?.Invoke();
            }

            // Escape键 - 取消
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnCancelKey?.Invoke();
            }

            // 画笔大小快捷键
            // [ 减小, ] 增大
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
                Debug.LogWarning("[EditorInput] EditorCamera is null");
                return Vector3.zero;
            }

            // 创建一个从相机发出的射线
            Ray ray = EditorCamera.ScreenPointToRay(screenPos);

            // 与地面平面相交
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