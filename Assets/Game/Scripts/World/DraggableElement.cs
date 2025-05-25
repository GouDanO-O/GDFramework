using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.World
{
    [RequireComponent(typeof(RectTransform))]
    public class DraggableElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        private RectTransform rectTransform;
        private Canvas canvas;
        private RectTransform parentRectTransform;
        private Vector2 originalLocalPosition;
        private bool isDragging = false;

        [Header("拖拽设置")]
        public bool enableDrag = true;
        public float dragThreshold = 1f;

        [Header("点击事件")]
        public bool enableClick = true;
        public UnityEngine.Events.UnityEvent onClick;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!enableDrag) return;

            isDragging = true;
            originalLocalPosition = rectTransform.localPosition;

            // 可以在这里添加开始拖拽时的视觉反馈
            // 比如改变透明度或添加外发光效果
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!enableDrag || !isDragging) return;

            // 将屏幕空间的delta转换为世界空间的delta
            Vector2 delta = eventData.delta / canvas.scaleFactor;

            // 考虑父物体的缩放
            Vector3 scale = parentRectTransform.lossyScale;
            delta.x /= scale.x;
            delta.y /= scale.y;

            // 应用移动
            rectTransform.localPosition += (Vector3)delta;

            // 可以在这里添加拖拽限制
            // ClampPosition();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!enableDrag) return;

            isDragging = false;

            // 如果移动距离小于阈值，恢复原位
            float distance = Vector2.Distance(rectTransform.localPosition, originalLocalPosition);
            if (distance < dragThreshold)
            {
                rectTransform.localPosition = originalLocalPosition;
            }

            // 可以在这里添加结束拖拽时的视觉反馈
            // 比如恢复原来的透明度或移除外发光效果
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!enableClick) return;

            // 如果正在拖拽，不触发点击事件
            if (isDragging) return;

            // 触发点击事件
            onClick?.Invoke();
        }

        // 可选：限制拖拽范围的方法
        private void ClampPosition()
        {
            Vector3 localPos = rectTransform.localPosition;
            
            // 获取父容器的尺寸
            Vector2 parentSize = parentRectTransform.rect.size;
            
            // 获取当前元素的尺寸
            Vector2 elementSize = rectTransform.rect.size;
            
            // 计算最大允许范围
            float maxX = parentSize.x * 0.5f - elementSize.x * 0.5f;
            float maxY = parentSize.y * 0.5f - elementSize.y * 0.5f;
            
            // 限制位置
            localPos.x = Mathf.Clamp(localPos.x, -maxX, maxX);
            localPos.y = Mathf.Clamp(localPos.y, -maxY, maxY);
            
            rectTransform.localPosition = localPos;
        }

        // 公共方法：设置元素位置
        public void SetPosition(Vector2 newLocalPosition)
        {
            rectTransform.localPosition = newLocalPosition;
        }

        // 公共方法：获取元素位置
        public Vector2 GetPosition()
        {
            return rectTransform.localPosition;
        }
    }
} 