using Game.World;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("节点数据")]
    public NodeData nodeData;
    
    [Header("拖拽设置")]
    public bool isDraggable = true;
    public float dragSensitivity = 1f;
    
    [Header("视觉反馈")]
    public Image nodeImage;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color dragColor = Color.green;
    public float scaleOnHover = 1.1f;
    
    private Vector2 originalPosition;
    private bool isDragging = false;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private NodeCanvasController canvasController;
    private Vector3 originalScale;
    
    // 连线相关
    private BatchConnectionLines connectionLines;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        canvasController = GetComponentInParent<NodeCanvasController>();
        connectionLines = FindObjectOfType<BatchConnectionLines>();
        
        if (nodeImage == null)
            nodeImage = GetComponent<Image>();
            
        originalScale = transform.localScale;
    }
    
    void Start()
    {
        // 设置初始颜色
        if (nodeImage != null)
            nodeImage.color = normalColor;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        // 通知画布控制器，当前有节点被选中
        if (canvasController != null)
        {
            canvasController.OnNodeSelected(this);
        }
        
        // 记录原始位置
        originalPosition = rectTransform.anchoredPosition;
        
        // 视觉反馈
        SetNodeColor(dragColor);
        
        Debug.Log($"节点 {nodeData?.NodeDataPersistent.nodeName ?? "Unknown"} 被点击");
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        isDragging = true;
        
        // 通知画布控制器开始拖拽节点
        if (canvasController != null)
        {
            canvasController.SetDragMode(NodeCanvasController.DragMode.Node);
        }
        
        // 将节点移到最前面
        transform.SetAsLastSibling();
        
        Debug.Log($"开始拖拽节点: {nodeData?.NodeDataPersistent.nodeName ?? "Unknown"}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable || !isDragging) return;
        
        // 计算新位置
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint))
        {
            rectTransform.anchoredPosition = localPoint * dragSensitivity;
        }
        
        // 通知连线系统更新
        if (connectionLines != null)
        {
            connectionLines.SetDirtyAll();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        isDragging = false;
        
        // 恢复画布控制器的拖拽模式
        if (canvasController != null)
        {
            canvasController.SetDragMode(NodeCanvasController.DragMode.Canvas);
        }
        
        // 恢复视觉状态
        SetNodeColor(normalColor);
        
        Debug.Log($"结束拖拽节点: {nodeData?.NodeDataPersistent.nodeName ?? "Unknown"}");
        Debug.Log($"节点最终位置: {rectTransform.anchoredPosition}");
    }
    
    // 鼠标悬停效果
    public void OnPointerEnter()
    {
        if (!isDragging)
        {
            SetNodeColor(hoverColor);
            transform.localScale = originalScale * scaleOnHover;
        }
    }
    
    public void OnPointerExit()
    {
        if (!isDragging)
        {
            SetNodeColor(normalColor);
            transform.localScale = originalScale;
        }
    }
    
    private void SetNodeColor(Color color)
    {
        if (nodeImage != null)
        {
            nodeImage.color = color;
        }
    }
    
    // 重置节点位置
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        if (connectionLines != null)
        {
            connectionLines.SetDirtyAll();
        }
    }
    
    // 设置节点是否可拖拽
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
    
    // 获取节点的世界坐标
    public Vector3 GetWorldPosition()
    {
        return rectTransform.position;
    }
    
    // 获取连接点位置（用于连线）
    public RectTransform GetConnectionPoint()
    {
        return rectTransform;
    }
    
    // 节点交互方法（预留接口）
    public virtual void OnNodeInteract()
    {
        Debug.Log($"与节点 {nodeData?.NodeDataPersistent.nodeName ?? "Unknown"} 交互");
        // 子类可以重写此方法实现具体的交互逻辑
    }
    
    // 节点连接方法（预留接口）
    public virtual bool CanConnectTo(NodeComponent otherNode)
    {
        // 基础连接规则，子类可以重写
        return otherNode != null && otherNode != this;
    }
    
    // 创建到其他节点的连接
    public void CreateConnectionTo(NodeComponent targetNode, Color? connectionColor = null)
    {
        if (connectionLines != null && CanConnectTo(targetNode))
        {
            connectionLines.AddConnection(
                this.GetConnectionPoint(),
                targetNode.GetConnectionPoint(),
                5f,
                connectionColor ?? Color.white
            );
        }
    }
}