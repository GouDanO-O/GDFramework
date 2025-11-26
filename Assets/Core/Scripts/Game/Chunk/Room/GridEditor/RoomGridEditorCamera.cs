using System;
using GDFramework.Input;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid.Editor
{
    /// <summary>
    /// 编辑器相机控制器
    /// 处理相机的移动、旋转、缩放
    /// </summary>
    public class RoomGridEditorCamera : MonoBehaviour, ICanRegisterEvent
    {
        #region 配置

        [Title("相机设置")]
        
        [LabelText("目标相机")]
        [SerializeField]
        private Camera _targetCamera;

        [LabelText("相机臂（Pivot）")]
        [SerializeField]
        private Transform _cameraPivot;

        [Title("移动设置")]
        
        [LabelText("移动速度")]
        [SerializeField]
        private float _moveSpeed = 20f;

        [LabelText("移动平滑度")]
        [SerializeField]
        [Range(0, 1)]
        private float _moveSmoothness = 0.1f;

        [LabelText("边界限制")]
        [SerializeField]
        private bool _enableBounds = true;

        [LabelText("边界范围")]
        [SerializeField]
        [ShowIf("_enableBounds")]
        private Bounds _moveBounds = new Bounds(Vector3.zero, new Vector3(100, 50, 100));

        [Title("缩放设置")]
        
        [LabelText("缩放速度")]
        [SerializeField]
        private float _zoomSpeed = 5f;

        [LabelText("最小距离")]
        [SerializeField]
        private float _minZoomDistance = 5f;

        [LabelText("最大距离")]
        [SerializeField]
        private float _maxZoomDistance = 50f;

        [LabelText("当前距离")]
        [SerializeField]
        [ReadOnly]
        private float _currentZoomDistance = 20f;

        [Title("旋转设置")]
        
        [LabelText("启用旋转")]
        [SerializeField]
        private bool _enableRotation = true;

        [LabelText("旋转速度")]
        [SerializeField]
        [ShowIf("_enableRotation")]
        private float _rotateSpeed = 100f;

        [LabelText("最小俯仰角")]
        [SerializeField]
        [ShowIf("_enableRotation")]
        private float _minPitch = 20f;

        [LabelText("最大俯仰角")]
        [SerializeField]
        [ShowIf("_enableRotation")]
        private float _maxPitch = 80f;

        [Title("当前状态")]
        
        [LabelText("当前位置")]
        [SerializeField]
        [ReadOnly]
        private Vector3 _currentPosition;

        [LabelText("当前旋转")]
        [SerializeField]
        [ReadOnly]
        private Vector2 _currentRotation = new Vector2(45f, 0f); // x=pitch, y=yaw

        #endregion

        #region 属性

        public Camera TargetCamera => _targetCamera;

        public bool IsMiddleMouseDragging { get; private set; }
        public bool IsRightMouseDragging { get; private set; }

        #endregion

        #region 私有字段

        private IArchitecture _architecture;
        private Vector3 _targetPosition;
        private float _targetZoomDistance;
        private Vector2 _targetRotation;
        private Vector2 _lastMousePosition;
        private bool _isInitialized;

        #endregion

        #region 生命周期

        private void Awake()
        {
            if (_targetCamera == null)
            {
                _targetCamera = GetComponentInChildren<Camera>();
            }

            if (_cameraPivot == null)
            {
                _cameraPivot = transform;
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (!_isInitialized) return;

            UpdateKeyboardMovement();
            UpdateCameraTransform();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #endregion

        #region 初始化

        public IArchitecture GetArchitecture()
        {
            return _architecture;
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            _architecture = architecture;
        }

        public void Initialize()
        {
            _currentPosition = _cameraPivot.position;
            _targetPosition = _currentPosition;
            _targetZoomDistance = _currentZoomDistance;
            _targetRotation = _currentRotation;

            RegisterEvents();
            ApplyCameraTransform();
            
            _isInitialized = true;
            Debug.Log("[EditorCamera] 初始化完成");
        }

        private void RegisterEvents()
        {
            if (_architecture == null) return;

            this.RegisterEvent<SInputEvent_MouseMiddleDown>(OnMiddleMouseDown);
            this.RegisterEvent<SInputEvent_MouseMiddleUp>(OnMiddleMouseUp);
            this.RegisterEvent<SInputEvent_MouseDrag>(OnMouseDrag);
            this.RegisterEvent<SInputEvent_MouseMiddleScroll>(OnMouseScroll);
            this.RegisterEvent<SInputEvent_MouseRightClick>(OnRightMouseClick);
        }

        private void UnregisterEvents()
        {
            if (_architecture == null) return;

            this.UnRegisterEvent<SInputEvent_MouseMiddleDown>(OnMiddleMouseDown);
            this.UnRegisterEvent<SInputEvent_MouseMiddleUp>(OnMiddleMouseUp);
            this.UnRegisterEvent<SInputEvent_MouseDrag>(OnMouseDrag);
            this.UnRegisterEvent<SInputEvent_MouseMiddleScroll>(OnMouseScroll);
            this.UnRegisterEvent<SInputEvent_MouseRightClick>(OnRightMouseClick);
        }

        #endregion

        #region 事件处理

        private void OnMiddleMouseDown(SInputEvent_MouseMiddleDown evt)
        {
            IsMiddleMouseDragging = true;
            _lastMousePosition = Input.mousePosition;
        }

        private void OnMiddleMouseUp(SInputEvent_MouseMiddleUp evt)
        {
            IsMiddleMouseDragging = false;
        }

        private void OnRightMouseClick(SInputEvent_MouseRightClick evt)
        {
            // 右键用于旋转
            if (Input.GetMouseButton(1))
            {
                IsRightMouseDragging = true;
                _lastMousePosition = Input.mousePosition;
            }
            else
            {
                IsRightMouseDragging = false;
            }
        }

        private void OnMouseDrag(SInputEvent_MouseDrag evt)
        {
            Vector2 mousePos = evt.mousePos;
            if (mousePos == Vector2.zero) return;

            Vector2 delta = mousePos - _lastMousePosition;
            _lastMousePosition = mousePos;

            // 中键拖拽 - 平移相机
            if (IsMiddleMouseDragging || Input.GetMouseButton(2))
            {
                HandleMiddleMouseDrag(delta);
            }

            // 右键拖拽 - 旋转相机
            if (_enableRotation && Input.GetMouseButton(1))
            {
                HandleRightMouseDrag(delta);
            }
        }

        private void OnMouseScroll(SInputEvent_MouseMiddleScroll evt)
        {
            float scrollValue = evt.scrollValue.y;
            HandleZoom(scrollValue);
        }

        #endregion

        #region 相机操作

        /// <summary>
        /// 处理中键拖拽（平移）
        /// </summary>
        private void HandleMiddleMouseDrag(Vector2 delta)
        {
            // 计算相机的右方向和前方向（忽略Y轴）
            Vector3 right = _cameraPivot.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up).normalized;

            // 根据鼠标移动量计算位移
            float sensitivity = _moveSpeed * _currentZoomDistance * 0.001f;
            Vector3 move = (-right * delta.x - forward * delta.y) * sensitivity;

            _targetPosition += move;
            ClampToBounds();
        }

        /// <summary>
        /// 处理右键拖拽（旋转）
        /// </summary>
        private void HandleRightMouseDrag(Vector2 delta)
        {
            float sensitivity = _rotateSpeed * Time.deltaTime;
            
            _targetRotation.y += delta.x * sensitivity; // Yaw
            _targetRotation.x -= delta.y * sensitivity; // Pitch
            
            // 限制俯仰角
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, _minPitch, _maxPitch);
        }

        /// <summary>
        /// 处理滚轮缩放
        /// </summary>
        private void HandleZoom(float scrollValue)
        {
            _targetZoomDistance -= scrollValue * _zoomSpeed;
            _targetZoomDistance = Mathf.Clamp(_targetZoomDistance, _minZoomDistance, _maxZoomDistance);
        }

        /// <summary>
        /// 键盘移动
        /// </summary>
        private void UpdateKeyboardMovement()
        {
            Vector3 input = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                input.z += 1;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                input.z -= 1;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                input.x -= 1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                input.x += 1;

            if (input.sqrMagnitude > 0.01f)
            {
                input.Normalize();

                // 根据相机方向计算移动
                Vector3 right = _cameraPivot.right;
                Vector3 forward = Vector3.Cross(right, Vector3.up).normalized;

                Vector3 move = (right * input.x + forward * input.z) * _moveSpeed * Time.deltaTime;
                _targetPosition += move;
                ClampToBounds();
            }
        }

        /// <summary>
        /// 更新相机变换
        /// </summary>
        private void UpdateCameraTransform()
        {
            // 平滑移动
            _currentPosition = Vector3.Lerp(_currentPosition, _targetPosition, 1f - _moveSmoothness);
            
            // 平滑缩放
            _currentZoomDistance = Mathf.Lerp(_currentZoomDistance, _targetZoomDistance, 1f - _moveSmoothness);
            
            // 平滑旋转
            _currentRotation = Vector2.Lerp(_currentRotation, _targetRotation, 1f - _moveSmoothness);

            ApplyCameraTransform();
        }

        /// <summary>
        /// 应用相机变换
        /// </summary>
        private void ApplyCameraTransform()
        {
            // 设置Pivot位置
            _cameraPivot.position = _currentPosition;
            
            // 设置Pivot旋转
            _cameraPivot.rotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0);
            
            // 设置相机距离
            if (_targetCamera != null)
            {
                _targetCamera.transform.localPosition = new Vector3(0, 0, -_currentZoomDistance);
                _targetCamera.transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// 限制在边界内
        /// </summary>
        private void ClampToBounds()
        {
            if (!_enableBounds) return;

            _targetPosition.x = Mathf.Clamp(_targetPosition.x, _moveBounds.min.x, _moveBounds.max.x);
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, _moveBounds.min.y, _moveBounds.max.y);
            _targetPosition.z = Mathf.Clamp(_targetPosition.z, _moveBounds.min.z, _moveBounds.max.z);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 聚焦到指定位置
        /// </summary>
        public void FocusOn(Vector3 position, bool instant = false)
        {
            _targetPosition = position;
            
            if (instant)
            {
                _currentPosition = position;
                ApplyCameraTransform();
            }
        }

        /// <summary>
        /// 聚焦到指定地块
        /// </summary>
        public void FocusOnTile(TilePosition tilePos, RoomGridConfig config, bool instant = false)
        {
            Vector3 worldPos = config.TileToWorld(tilePos);
            FocusOn(worldPos, instant);
        }

        /// <summary>
        /// 设置缩放距离
        /// </summary>
        public void SetZoom(float distance, bool instant = false)
        {
            _targetZoomDistance = Mathf.Clamp(distance, _minZoomDistance, _maxZoomDistance);
            
            if (instant)
            {
                _currentZoomDistance = _targetZoomDistance;
                ApplyCameraTransform();
            }
        }

        /// <summary>
        /// 设置旋转
        /// </summary>
        public void SetRotation(float pitch, float yaw, bool instant = false)
        {
            _targetRotation = new Vector2(
                Mathf.Clamp(pitch, _minPitch, _maxPitch),
                yaw
            );
            
            if (instant)
            {
                _currentRotation = _targetRotation;
                ApplyCameraTransform();
            }
        }

        /// <summary>
        /// 重置相机
        /// </summary>
        public void ResetCamera()
        {
            _targetPosition = Vector3.zero;
            _targetZoomDistance = 20f;
            _targetRotation = new Vector2(45f, 0f);
        }

        /// <summary>
        /// 设置边界
        /// </summary>
        public void SetBounds(Bounds bounds)
        {
            _moveBounds = bounds;
        }

        /// <summary>
        /// 根据网格配置设置边界
        /// </summary>
        public void SetBoundsFromGrid(RoomGridConfig config)
        {
            if (config == null) return;

            var gridBounds = config.GetWorldBounds();
            
            // 扩展一些边距
            float margin = 10f;
            gridBounds.Expand(margin * 2);
            
            _moveBounds = gridBounds;
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (_enableBounds)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(_moveBounds.center, _moveBounds.size);
            }
        }

        #endregion
    }
}