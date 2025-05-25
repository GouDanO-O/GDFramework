using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.World
{
    [RequireComponent(typeof(RectTransform))]
    public class UIRootController : MonoBehaviour, IScrollHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("相机设置")]
        public float minZoom = 0.2f;
        public float maxZoom = 3f;
        public float zoomSpeed = 0.1f;
        public bool smoothZoom = true;
        public float smoothSpeed = 5f;

        [Header("拖拽设置")]
        public bool enableMiddleMouseDrag = true;
        public float dragSensitivity = 1f;

        [Header("视图设置")]
        public RectTransform viewportRoot; // RoomRoot - 视窗
        public RectTransform contentRoot;  // Content - 内容容器

        // 相机状态
        private Vector2 cameraPosition = Vector2.zero;  // 相机在世界空间的位置
        private float cameraZoom = 1f;                  // 相机缩放（类似FOV）
        private float targetZoom = 1f;
        private Vector2 targetPosition = Vector2.zero;

        // 交互状态
        private bool isMouseOver = false;
        private bool isDragging = false;
        private Vector2 lastMousePosition;
        private Canvas parentCanvas;

        void Start()
        {
            // 自动查找组件
            if (viewportRoot == null)
                viewportRoot = transform.parent?.GetComponent<RectTransform>();

            if (contentRoot == null)
                contentRoot = GetComponent<RectTransform>();

            parentCanvas = GetComponentInParent<Canvas>();

            // 初始化相机状态
            cameraZoom = targetZoom = 1f;
            cameraPosition = targetPosition = Vector2.zero;

            // 确保有透明背景用于接收事件
            EnsureEventReceiver();

            // 应用初始变换
            ApplyCameraTransform();
        }

        void EnsureEventReceiver()
        {
            // 在viewportRoot上添加透明图片用于接收鼠标事件
            Image eventReceiver = viewportRoot.GetComponent<Image>();
            if (eventReceiver == null)
            {
                eventReceiver = viewportRoot.gameObject.AddComponent<Image>();
            }
            eventReceiver.color = new Color(0, 0, 0, 0);
            eventReceiver.raycastTarget = true;

            // 确保有GraphicRaycaster
            if (viewportRoot.GetComponent<GraphicRaycaster>() == null)
            {
                viewportRoot.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        void Update()
        {
            // 平滑相机移动和缩放
            bool needsUpdate = false;

            if (smoothZoom && Mathf.Abs(cameraZoom - targetZoom) > 0.001f)
            {
                cameraZoom = Mathf.Lerp(cameraZoom, targetZoom, Time.deltaTime * smoothSpeed);
                needsUpdate = true;
            }

            if (Vector2.Distance(cameraPosition, targetPosition) > 0.1f)
            {
                cameraPosition = Vector2.Lerp(cameraPosition, targetPosition, Time.deltaTime * smoothSpeed);
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                ApplyCameraTransform();
            }

            // 处理中键拖拽
            HandleMiddleMouseDrag();
        }

        void HandleMiddleMouseDrag()
        {
            if (!enableMiddleMouseDrag || !isMouseOver) return;

            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                isDragging = false;
            }

            if (isDragging && Input.GetMouseButton(2))
            {
                Vector2 currentMousePosition = Input.mousePosition;
                Vector2 screenDelta = currentMousePosition - lastMousePosition;

                // 将屏幕移动转换为世界空间移动（考虑缩放）
                Vector2 worldDelta = screenDelta / (parentCanvas.scaleFactor * cameraZoom) * dragSensitivity;

                // 移动相机（注意方向相反，因为是移动相机而不是内容）
                targetPosition -= worldDelta;
                ClampTargetPosition();

                if (!smoothZoom) // 如果不使用平滑，立即更新
                {
                    cameraPosition = targetPosition;
                    ApplyCameraTransform();
                }

                lastMousePosition = currentMousePosition;
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!isMouseOver) return;

            float scrollDelta = eventData.scrollDelta.y;
            float zoomChange = scrollDelta * zoomSpeed;

            float newTargetZoom = Mathf.Clamp(targetZoom + zoomChange, minZoom, maxZoom);

            if (Mathf.Abs(newTargetZoom - targetZoom) < 0.001f) return;

            // 获取鼠标在视窗中的位置
            Vector2 mouseViewportPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewportRoot, eventData.position, parentCanvas.worldCamera, out mouseViewportPos);

            // 计算鼠标在世界空间中的位置（缩放前）
            Vector2 mouseWorldPos = ViewportToWorldPosition(mouseViewportPos);

            // 更新缩放
            float oldZoom = targetZoom;
            targetZoom = newTargetZoom;

            // 调整相机位置，使鼠标指向的世界点保持不变
            Vector2 mouseWorldPosAfterZoom = ViewportToWorldPosition(mouseViewportPos);
            Vector2 worldDelta = mouseWorldPos - mouseWorldPosAfterZoom;
            targetPosition += worldDelta;
            ClampTargetPosition();

            if (!smoothZoom)
            {
                cameraZoom = targetZoom;
                cameraPosition = targetPosition;
                ApplyCameraTransform();
            }
        }

        // 将视窗坐标转换为世界坐标
        Vector2 ViewportToWorldPosition(Vector2 viewportPos)
        {
            return cameraPosition + viewportPos / cameraZoom;
        }

        // 将世界坐标转换为视窗坐标
        Vector2 WorldToViewportPosition(Vector2 worldPos)
        {
            return (worldPos - cameraPosition) * cameraZoom;
        }

        // 应用相机变换到Content
        void ApplyCameraTransform()
        {
            // 计算Content应该显示的位置和缩放
            Vector2 contentPosition = -cameraPosition * cameraZoom;
            float contentScale = cameraZoom;

            // 应用变换
            contentRoot.anchoredPosition = contentPosition;
            contentRoot.localScale = Vector3.one * contentScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOver = false;
            isDragging = false;
        }

        // === 新增方法：获取所有节点 Bounds ===
        Bounds GetNodeBounds()
        {
            Bounds bounds = new Bounds();
            bool init = false;
            foreach (RectTransform child in contentRoot.GetComponentsInChildren<RectTransform>())
            {
                if (child == contentRoot) continue;
                if (child.GetComponent<Graphic>() == null) continue;

                Vector3 localPos = contentRoot.InverseTransformPoint(child.position);
                if (!init)
                {
                    bounds = new Bounds(localPos, Vector3.zero);
                    init = true;
                }
                else
                {
                    bounds.Encapsulate(localPos);
                }
            }
            return bounds;
        }

        // === 新增方法：约束视口位置，保证至少一个节点可见 ===
        void ClampTargetPosition()
        {
            Bounds nb = GetNodeBounds();
            if (nb.size == Vector3.zero) return; // 没有节点就不约束

            Rect vp = viewportRoot.rect;
            Vector2 halfWorldSize = new Vector2(vp.width, vp.height) * 0.5f / targetZoom;

            float minX = nb.min.x + halfWorldSize.x;
            float maxX = nb.max.x - halfWorldSize.x;
            float minY = nb.min.y + halfWorldSize.y;
            float maxY = nb.max.y - halfWorldSize.y;

            // 如果视口比节点区域大，则允许居中显示
            if (minX > maxX)
            {
                float centerX = (nb.min.x + nb.max.x) * 0.5f;
                minX = maxX = centerX;
            }
            if (minY > maxY)
            {
                float centerY = (nb.min.y + nb.max.y) * 0.5f;
                minY = maxY = centerY;
            }

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // === 公共API ===
        public void SetZoom(float zoom)
        {
            targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            ClampTargetPosition();
            if (!smoothZoom)
            {
                cameraZoom = targetZoom;
                ApplyCameraTransform();
            }
        }

        public float GetZoom()
        {
            return cameraZoom;
        }

        public void SetCameraPosition(Vector2 position)
        {
            targetPosition = position;
            ClampTargetPosition();
            if (!smoothZoom)
            {
                cameraPosition = targetPosition;
                ApplyCameraTransform();
            }
        }

        public Vector2 GetCameraPosition()
        {
            return cameraPosition;
        }

        public void ResetCamera()
        {
            targetZoom = 1f;
            targetPosition = Vector2.zero;
            ClampTargetPosition();
            if (!smoothZoom)
            {
                cameraZoom = targetZoom;
                cameraPosition = targetPosition;
                ApplyCameraTransform();
            }
        }

        public void FocusOnWorldPosition(Vector2 worldPos, float zoom = -1)
        {
            targetPosition = worldPos;
            if (zoom > 0)
                targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            ClampTargetPosition();
            if (!smoothZoom)
            {
                cameraPosition = targetPosition;
                if (zoom > 0) cameraZoom = targetZoom;
                ApplyCameraTransform();
            }
        }

        public void FocusOnUIElement(RectTransform target, float zoom = -1)
        {
            Vector2 localPos = contentRoot.InverseTransformPoint(target.position);
            FocusOnWorldPosition(localPos, zoom);
        }

        public void FitAllContent(float padding = 0.1f)
        {
            Bounds contentBounds = GetContentBounds();
            if (contentBounds.size == Vector3.zero) return;

            Rect viewportRect = viewportRoot.rect;
            float scaleX = viewportRect.width / (contentBounds.size.x * (1 + padding * 2));
            float scaleY = viewportRect.height / (contentBounds.size.y * (1 + padding * 2));
            float newZoom = Mathf.Min(scaleX, scaleY);
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

            Vector2 contentCenter = new Vector2(contentBounds.center.x, contentBounds.center.y);
            targetZoom = newZoom;
            targetPosition = contentCenter;
            ClampTargetPosition();

            if (!smoothZoom)
            {
                cameraZoom = targetZoom;
                cameraPosition = targetPosition;
                ApplyCameraTransform();
            }
        }

        Bounds GetContentBounds()
        {
            Bounds bounds = new Bounds();
            bool hasBounds = false;
           
            RectTransform[] childRects = contentRoot.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform child in childRects)
            {
                if (child == contentRoot) continue;
                if (child.GetComponent<Graphic>() == null) continue;

                Vector3[] corners = new Vector3[4];
                child.GetLocalCorners(corners);
                foreach (Vector3 corner in corners)
                {
                    Vector3 worldCorner = child.TransformPoint(corner);
                    Vector3 localCorner = contentRoot.InverseTransformPoint(worldCorner);
                    if (!hasBounds)
                    {
                        bounds = new Bounds(localCorner, Vector3.zero);
                        hasBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(localCorner);
                    }
                }
            }
            return bounds;
        }
    }
}
