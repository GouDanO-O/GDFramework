using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ConnectionData
{
    public RectTransform startPoint;
    public RectTransform endPoint;
    public float width = 5f;
    public Color startColor = Color.white;
    public Color endColor = Color.white;
    public bool useGradient = false;
    
    [System.NonSerialized]
    public bool isDirty = true; // 标记是否需要更新
}

[RequireComponent(typeof(CanvasRenderer))]
public class BatchConnectionLines : MaskableGraphic
{
    [Header("连接线数据")]
    public List<ConnectionData> connections = new List<ConnectionData>();
    
    [Header("性能优化")]
    public bool enableCulling = true;
    public float cullingDistance = 2000f; // 超出此距离的线条将被剔除
    public int maxLinesPerFrame = 50; // 每帧最多更新的线条数量
    
    private List<Vector2> cachedPositions = new List<Vector2>();
    private int updateIndex = 0; // 用于分帧更新
    private bool needsRebuild = true;
    
    // 对象池
    private Queue<UIVertex[]> vertexPool = new Queue<UIVertex[]>();
    private Queue<int[]> trianglePool = new Queue<int[]>();
    
    protected override void Start()
    {
        base.Start();
        // 预热对象池
        for (int i = 0; i < 20; i++)
        {
            vertexPool.Enqueue(new UIVertex[4]);
            trianglePool.Enqueue(new int[6]);
        }
    }
    
    void Update()
    {
        // 分帧检查连接点是否移动
        CheckForMovement();
        
        if (needsRebuild)
        {
            SetVerticesDirty();
            needsRebuild = false;
        }
    }
    
    void CheckForMovement()
    {
        if (connections.Count == 0) return;
        
        int checkCount = Mathf.Min(maxLinesPerFrame, connections.Count);
        int startIdx = updateIndex;
        
        for (int i = 0; i < checkCount; i++)
        {
            int idx = (startIdx + i) % connections.Count;
            var connection = connections[idx];
            
            if (connection.startPoint == null || connection.endPoint == null)
                continue;
                
            // 检查位置是否改变
            if (HasPositionChanged(connection, idx * 2) || HasPositionChanged(connection, idx * 2 + 1))
            {
                connection.isDirty = true;
                needsRebuild = true;
            }
        }
        
        updateIndex = (updateIndex + checkCount) % connections.Count;
    }
    
    bool HasPositionChanged(ConnectionData connection, int cacheIndex)
    {
        if (cacheIndex >= cachedPositions.Count)
        {
            // 扩展缓存
            while (cachedPositions.Count <= cacheIndex)
                cachedPositions.Add(Vector2.zero);
            return true;
        }
        
        Vector2 currentPos = cacheIndex % 2 == 0 ? 
            (Vector2)connection.startPoint.position : 
            (Vector2)connection.endPoint.position;
            
        if (Vector2.Distance(cachedPositions[cacheIndex], currentPos) > 0.1f)
        {
            cachedPositions[cacheIndex] = currentPos;
            return true;
        }
        
        return false;
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        if (connections.Count == 0) return;
        
        int vertexCount = 0;
        
        for (int i = 0; i < connections.Count; i++)
        {
            var connection = connections[i];
            
            if (connection.startPoint == null || connection.endPoint == null)
                continue;
            
            // 视锥剔除
            if (enableCulling && !IsLineVisible(connection))
                continue;
            
            AddLineToMesh(vh, connection, vertexCount);
            vertexCount += 4;
            
            connection.isDirty = false;
        }
    }
    
    bool IsLineVisible(ConnectionData connection)
    {
        Vector2 localPos1, localPos2;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.startPoint.position, null, out localPos1) ||
            !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.endPoint.position, null, out localPos2))
        {
            return false;
        }
        
        // 简单的距离剔除
        if (Vector2.Distance(localPos1, localPos2) > cullingDistance)
            return false;
        
        // 可以添加更复杂的视锥剔除逻辑
        Rect canvasRect = rectTransform.rect;
        return canvasRect.Contains(localPos1) || canvasRect.Contains(localPos2) ||
               LineIntersectsRect(localPos1, localPos2, canvasRect);
    }
    
    bool LineIntersectsRect(Vector2 p1, Vector2 p2, Rect rect)
    {
        // 简化的线段与矩形相交检测
        return !(Mathf.Max(p1.x, p2.x) < rect.xMin || 
                 Mathf.Min(p1.x, p2.x) > rect.xMax ||
                 Mathf.Max(p1.y, p2.y) < rect.yMin || 
                 Mathf.Min(p1.y, p2.y) > rect.yMax);
    }
    
    void AddLineToMesh(VertexHelper vh, ConnectionData connection, int vertexOffset)
    {
        Vector2 localPos1, localPos2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, connection.startPoint.position, null, out localPos1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, connection.endPoint.position, null, out localPos2);
        
        Vector2 direction = (localPos2 - localPos1).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * connection.width * 0.5f;
        
        Color color1 = connection.useGradient ? connection.startColor : connection.startColor;
        Color color2 = connection.useGradient ? connection.endColor : connection.startColor;
        
        // 获取或创建顶点数组
        UIVertex[] vertices = vertexPool.Count > 0 ? vertexPool.Dequeue() : new UIVertex[4];
        
        vertices[0] = CreateVertex(localPos1 + perpendicular, color1, new Vector2(0, 1));
        vertices[1] = CreateVertex(localPos1 - perpendicular, color1, new Vector2(0, 0));
        vertices[2] = CreateVertex(localPos2 - perpendicular, color2, new Vector2(1, 0));
        vertices[3] = CreateVertex(localPos2 + perpendicular, color2, new Vector2(1, 1));
        
        // 添加顶点
        for (int i = 0; i < 4; i++)
        {
            vh.AddVert(vertices[i]);
        }
        
        // 添加三角形
        vh.AddTriangle(vertexOffset, vertexOffset + 1, vertexOffset + 2);
        vh.AddTriangle(vertexOffset + 2, vertexOffset + 3, vertexOffset);
        
        // 回收到对象池
        vertexPool.Enqueue(vertices);
    }
    
    UIVertex CreateVertex(Vector3 position, Color color, Vector2 uv)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = color;
        vertex.uv0 = uv;
        return vertex;
    }
    
    // 公共API
    public int AddConnection(RectTransform start, RectTransform end, float width = 5f, Color? color = null)
    {
        var connection = new ConnectionData
        {
            startPoint = start,
            endPoint = end,
            width = width,
            startColor = color ?? Color.white,
            endColor = color ?? Color.white
        };
        
        connections.Add(connection);
        needsRebuild = true;
        
        return connections.Count - 1;
    }
    
    public void RemoveConnection(int index)
    {
        if (index >= 0 && index < connections.Count)
        {
            connections.RemoveAt(index);
            needsRebuild = true;
        }
    }
    
    public void UpdateConnection(int index, float width, Color startColor, Color endColor, bool useGradient = false)
    {
        if (index >= 0 && index < connections.Count)
        {
            var connection = connections[index];
            connection.width = width;
            connection.startColor = startColor;
            connection.endColor = endColor;
            connection.useGradient = useGradient;
            connection.isDirty = true;
            needsRebuild = true;
        }
    }
    
    public void ClearAllConnections()
    {
        connections.Clear();
        cachedPositions.Clear();
        needsRebuild = true;
    }
    
    // 批量操作API
    public void SetDirtyAll()
    {
        foreach (var connection in connections)
        {
            connection.isDirty = true;
        }
        needsRebuild = true;
    }
    
    public void SetCullingSettings(bool enable, float distance)
    {
        enableCulling = enable;
        cullingDistance = distance;
        needsRebuild = true;
    }
}