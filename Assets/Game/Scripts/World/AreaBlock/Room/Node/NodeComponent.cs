using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("拖拽设置")]
    public bool isDraggable = true;
    public bool constrainToParent = true;
    private RectTransform constraintArea; // 如果为空，则使用直接父级
    
    [Header("拖拽反馈")]
    public float dragScale = 1.1f;
    public Color dragColor = new Color(1, 1, 1, 0.8f);
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image buttonImage;
    
    // 拖拽状态
    private bool isDragging = false;
    private Vector3 originalScale;
    private Color originalColor;
    private int originalSortingOrder;
    
    // 约束区域
    private RectTransform parentRect;
    private Rect constraintBounds;
    
    // 拖拽偏移
    private Vector2 dragOffset;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttonImage = GetComponent<Image>();
        
        // 如果没有CanvasGroup，添加一个
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // 设置约束区域
        if (constraintArea == null)
        {
            parentRect = rectTransform.parent.GetComponent<RectTransform>();
        }
        else
        {
            parentRect = constraintArea;
        }
        
        // 保存原始状态
        originalScale = rectTransform.localScale;
        if (buttonImage != null)
            originalColor = buttonImage.color;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // 计算拖拽偏移，让鼠标不一定要在按钮中心
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition);
        
        dragOffset = localPointerPosition;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        isDragging = true;
        // 更新约束边界
        UpdateConstraintBounds();
        
        // 应用拖拽视觉效果
        ApplyDragVisuals(true);
        
        // 提升层级
        transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable || !isDragging) return;
        
        Vector2 globalMousePos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            // 减去拖拽偏移
            Vector2 targetPosition = globalMousePos - dragOffset * rectTransform.localScale.x;
            
            // 应用约束
            if (constrainToParent)
            {
                targetPosition = ConstrainToParent(targetPosition);
            }
            
            rectTransform.anchoredPosition = targetPosition;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable || !isDragging) return;
        
        isDragging = false;
        
        // 恢复视觉效果
        ApplyDragVisuals(false);
        
        // 最后一次约束检查
        if (constrainToParent)
        {
            Vector2 constrainedPos = ConstrainToParent(rectTransform.anchoredPosition);
            rectTransform.anchoredPosition = constrainedPos;
        }
        
        // 触发位置改变事件
        OnPositionChanged?.Invoke(rectTransform.anchoredPosition);
    }
    
    void UpdateConstraintBounds()
    {
        if (parentRect == null) 
            return;
        
        // 获取父级的实际可用区域
        constraintBounds = parentRect.rect;
        
        // 考虑当前缩放
        float currentScale = rectTransform.localScale.x;
        Vector2 buttonSize = rectTransform.rect.size * currentScale;
        
        // 调整约束边界以确保按钮完全在父级内部
        constraintBounds.xMin += buttonSize.x * 0.5f;
        constraintBounds.xMax -= buttonSize.x * 0.5f;
        constraintBounds.yMin += buttonSize.y * 0.5f;
        constraintBounds.yMax -= buttonSize.y * 0.5f;
    }
    
    Vector2 ConstrainToParent(Vector2 position)
    {
        // 实时更新约束边界（因为缩放可能改变）
        UpdateConstraintBounds();
        
        Vector2 constrainedPos = position;
        
        // X轴约束
        constrainedPos.x = Mathf.Clamp(constrainedPos.x, constraintBounds.xMin, constraintBounds.xMax);
        
        // Y轴约束
        constrainedPos.y = Mathf.Clamp(constrainedPos.y, constraintBounds.yMin, constraintBounds.yMax);
        
        return constrainedPos;
    }
    
    void ApplyDragVisuals(bool isDragging)
    {
        if (isDragging)
        {
            // 放大
            rectTransform.localScale = originalScale * dragScale;
            
            // 改变颜色
            if (buttonImage != null)
                buttonImage.color = dragColor;
            
            // 降低透明度
            if (canvasGroup != null)
                canvasGroup.alpha = 0.8f;
        }
        else
        {
            // 恢复原状
            rectTransform.localScale = originalScale;
            
            if (buttonImage != null)
                buttonImage.color = originalColor;
            
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }
    }
    
    // 公共API和事件
    public System.Action<Vector2> OnPositionChanged;
    public System.Action<NodeComponent> OnDragStart;
    public System.Action<NodeComponent> OnDragEnd;
    
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
    
    public void SetConstraintArea(RectTransform area)
    {
        constraintArea = area;
        parentRect = area;
    }
    
    public Vector2 GetPosition()
    {
        return rectTransform.anchoredPosition;
    }
    
    public void SetPosition(Vector2 position, bool forceConstrain = true)
    {
        if (forceConstrain && constrainToParent)
        {
            position = ConstrainToParent(position);
        }
        
        rectTransform.anchoredPosition = position;
    }
    
    public bool IsWithinBounds()
    {
        if (!constrainToParent || parentRect == null) return true;
        
        UpdateConstraintBounds();
        Vector2 currentPos = rectTransform.anchoredPosition;
        
        return currentPos.x >= constraintBounds.xMin && currentPos.x <= constraintBounds.xMax &&
               currentPos.y >= constraintBounds.yMin && currentPos.y <= constraintBounds.yMax;
    }
    
    public void SnapToConstraints()
    {
        if (constrainToParent)
        {
            Vector2 constrainedPos = ConstrainToParent(rectTransform.anchoredPosition);
            rectTransform.anchoredPosition = constrainedPos;
        }
    }
    
    // 调试用：绘制约束边界
    void OnDrawGizmosSelected()
    {
        if (parentRect == null) return;
        
        UpdateConstraintBounds();
        
        // 转换到世界坐标
        Vector3[] corners = new Vector3[4];
        Vector3 center = parentRect.TransformPoint(constraintBounds.center);
        Vector3 size = parentRect.TransformVector(constraintBounds.size);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}