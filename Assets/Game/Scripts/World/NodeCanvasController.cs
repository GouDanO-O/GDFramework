using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NodeCanvasController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    [Header("画布控制设置")]
    public RectTransform nodeContainer; // 存放所有节点的容器
    public float dragSensitivity = 1f;
    public float zoomSensitivity = 0.1f;
    public float minZoom = 0.2f;
    public float maxZoom = 3f;
    public bool enableDrag = true;
    public bool enableZoom = true;
    
    [Header("边界限制")]
    public bool enableBounds = true;
    public float boundsPadding = 500f; // 边界内边距
    
    [Header("惯性滑动")]
    public bool enableInertia = true;
    public float inertiaDeceleration = 10f;
    
    private Vector2 lastPointerPosition;
    private bool isDragging = false;
    private float currentZoom = 1f;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    
    // 拖拽模式枚举
    public enum DragMode
    {
        Canvas,  // 拖拽画布
        Node     // 拖拽节点
    }
    
    private DragMode currentDragMode = DragMode.Canvas;
    private NodeComponent selectedNode;
    
    // 惯性相关
    private Vector2 velocity = Vector2.zero;
    private bool hasInertia = false;
    
    // 边界计算
    private Vector2 canvasSize;
    private Vector2 containerSize;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        
        if (nodeContainer == null)
        {
            // 如果没有指定容器，创建一个
            GameObject container = new GameObject("NodeContainer");
            container.transform.SetParent(transform);
            nodeContainer = container.AddComponent<RectTransform>();
            nodeContainer.anchorMin = Vector2.zero;
            nodeContainer.anchorMax = Vector2.one;
            nodeContainer.offsetMin = Vector2.zero;
            nodeContainer.offsetMax = Vector2.zero;
        }
        
        currentZoom = nodeContainer.localScale.x;
    }
    
    void Start()
    {
        UpdateCanvasSize();
    }
    
    void Update()
    {
        // 处理惯性滑动
        if (hasInertia && enableInertia)
        {
            if (velocity.magnitude > 0.1f)
            {
                nodeContainer.anchoredPosition += velocity * Time.deltaTime;
                velocity = Vector2.Lerp(velocity, Vector2.zero, inertiaDeceleration * Time.deltaTime);
                
                // 应用边界限制
                if (enableBounds)
                {
                    ApplyBounds();
                }
            }
            else
            {
                hasInertia = false;
                velocity = Vector2.zero;
            }
        }
        
        // 检测键盘输入进行画布控制
        HandleKeyboardInput();
    }
    
    private void HandleKeyboardInput()
    {
        // 空格键重置画布
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCanvas();
        }
        
        // WASD键移动画布
        Vector2 keyboardMove = Vector2.zero;
        float moveSpeed = 500f * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.W)) keyboardMove.y += moveSpeed;
        if (Input.GetKey(KeyCode.S)) keyboardMove.y -= moveSpeed;
        if (Input.GetKey(KeyCode.A)) keyboardMove.x -= moveSpeed;
        if (Input.GetKey(KeyCode.D)) keyboardMove.x += moveSpeed;
        
        if (keyboardMove != Vector2.zero)
        {
            nodeContainer.anchoredPosition += keyboardMove;
            if (enableBounds)
            {
                ApplyBounds();
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enableDrag || currentDragMode != DragMode.Canvas) return;
        
        isDragging = true;
        lastPointerPosition = eventData.position;
        hasInertia = false;
        velocity = Vector2.zero;
        
        Debug.Log("开始拖拽画布");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!enableDrag || !isDragging || currentDragMode != DragMode.Canvas) return;
        
        Vector2 currentPointerPosition = eventData.position;
        Vector2 deltaPosition = (currentPointerPosition - lastPointerPosition) * dragSensitivity;
        
        // 根据当前缩放调整拖拽灵敏度
        deltaPosition /= currentZoom;
        
        nodeContainer.anchoredPosition += deltaPosition;
        
        // 记录速度用于惯性
        velocity = deltaPosition / Time.deltaTime;
        
        lastPointerPosition = currentPointerPosition;
        
        // 应用边界限制
        if (enableBounds)
        {
            ApplyBounds();
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!enableDrag) return;
        
        isDragging = false;
        
        // 启动惯性滑动
        if (enableInertia && velocity.magnitude > 100f)
        {
            hasInertia = true;
        }
        
        Debug.Log("结束拖拽画布");
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        if (!enableZoom) return;
        
        float scrollDelta = eventData.scrollDelta.y;
        float zoomFactor = 1 + (scrollDelta * zoomSensitivity);
        
        ZoomCanvas(zoomFactor, eventData.position);
    }
    
    private void ZoomCanvas(float zoomFactor, Vector2 zoomCenter)
    {
        float newZoom = Mathf.Clamp(currentZoom * zoomFactor, minZoom, maxZoom);
        
        if (Mathf.Approximately(newZoom, currentZoom)) return;
        
        // 计算缩放中心点
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, zoomCenter, null, out localPoint);
        
        // 应用缩放
        Vector2 pivotOffset = localPoint - nodeContainer.anchoredPosition;
        float scaleRatio = newZoom / currentZoom;
        
        nodeContainer.localScale = Vector3.one * newZoom;
        nodeContainer.anchoredPosition = localPoint - pivotOffset * scaleRatio;
        
        currentZoom = newZoom;
        
        // 应用边界限制
        if (enableBounds)
        {
            ApplyBounds();
        }
        
        Debug.Log($"画布缩放: {currentZoom:F2}");
    }
    
    private void ApplyBounds()
    {
        UpdateCanvasSize();
        
        Vector2 currentPos = nodeContainer.anchoredPosition;
        Vector2 scaledContainerSize = containerSize * currentZoom;
        
        // 计算边界
        float minX = canvasSize.x - scaledContainerSize.x - boundsPadding;
        float maxX = boundsPadding;
        float minY = canvasSize.y - scaledContainerSize.y - boundsPadding;
        float maxY = boundsPadding;
        
        // 如果容器比画布小，则居中
        if (scaledContainerSize.x < canvasSize.x)
        {
            minX = maxX = (canvasSize.x - scaledContainerSize.x) * 0.5f;
        }
        if (scaledContainerSize.y < canvasSize.y)
        {
            minY = maxY = (canvasSize.y - scaledContainerSize.y) * 0.5f;
        }
        
        // 应用边界限制
        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);
        
        nodeContainer.anchoredPosition = currentPos;
    }
    
    private void UpdateCanvasSize()
    {
        canvasSize = rectTransform.rect.size;
        containerSize = nodeContainer.rect.size;
    }
    
    // 重置画布到初始状态
    public void ResetCanvas()
    {
        nodeContainer.anchoredPosition = Vector2.zero;
        nodeContainer.localScale = Vector3.one;
        currentZoom = 1f;
        velocity = Vector2.zero;
        hasInertia = false;
        
        Debug.Log("画布已重置");
    }
    
    // 设置拖拽模式
    public void SetDragMode(DragMode mode)
    {
        currentDragMode = mode;
    }
    
    // 节点选择回调
    public void OnNodeSelected(NodeComponent node)
    {
        selectedNode = node;
    }
    
    // 缩放到指定节点
    public void FocusOnNode(NodeComponent node, float targetZoom = 1f)
    {
        if (node == null) return;
        
        // 计算节点在画布中的位置
        Vector2 nodeLocalPos = nodeContainer.InverseTransformPoint(node.transform.position);
        
        // 设置缩放
        currentZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        nodeContainer.localScale = Vector3.one * currentZoom;
        
        // 将节点移动到画布中心
        Vector2 canvasCenter = rectTransform.rect.center;
        nodeContainer.anchoredPosition = canvasCenter - nodeLocalPos * currentZoom;
        
        // 应用边界限制
        if (enableBounds)
        {
            ApplyBounds();
        }
        
        Debug.Log($"聚焦到节点: {node.nodeData?.NodeDataPersistent.nodeName ?? "Unknown"}");
    }
    
    // 自动适应所有节点
    public void FitAllNodes()
    {
        NodeComponent[] allNodes = nodeContainer.GetComponentsInChildren<NodeComponent>();
        if (allNodes.Length == 0) return;
        
        // 计算所有节点的边界
        Bounds bounds = new Bounds();
        bool firstNode = true;
        
        foreach (var node in allNodes)
        {
            Vector3 nodePos = nodeContainer.InverseTransformPoint(node.transform.position);
            if (firstNode)
            {
                bounds = new Bounds(nodePos, Vector3.zero);
                firstNode = false;
            }
            else
            {
                bounds.Encapsulate(nodePos);
            }
        }
        
        // 计算适合的缩放比例
        float scaleX = (canvasSize.x - boundsPadding * 2) / bounds.size.x;
        float scaleY = (canvasSize.y - boundsPadding * 2) / bounds.size.y;
        float optimalScale = Mathf.Min(scaleX, scaleY);
        optimalScale = Mathf.Clamp(optimalScale, minZoom, maxZoom);
        
        // 应用缩放和位置
        currentZoom = optimalScale;
        nodeContainer.localScale = Vector3.one * currentZoom;
        
        Vector2 canvasCenter = rectTransform.rect.center;
        nodeContainer.anchoredPosition = canvasCenter - (Vector2)bounds.center * currentZoom;
        
        Debug.Log($"自动适应所有节点，缩放: {currentZoom:F2}");
    }
    
    // 公共API
    public float GetCurrentZoom() => currentZoom;
    public Vector2 GetCanvasPosition() => nodeContainer.anchoredPosition;
    public DragMode GetCurrentDragMode() => currentDragMode;
}