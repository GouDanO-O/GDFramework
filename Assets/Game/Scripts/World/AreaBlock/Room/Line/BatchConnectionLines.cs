using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Game.World
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class BatchConnectionLines : MaskableGraphic
    {
        [TabGroup("连接管理")]
        [ListDrawerSettings(
            Expanded = true,
            ShowIndexLabels = true,
            ShowPaging = true,
            NumberOfItemsPerPage = 5,
            DraggableItems = true,
            HideAddButton = false,
            HideRemoveButton = false,
            CustomAddFunction = nameof(AddNewConnection),
            CustomRemoveIndexFunction = nameof(RemoveConnectionAt)
        )]
        [PropertySpace(SpaceAfter = 10)]
        public List<ConnectionData> connections = new List<ConnectionData>();

        [TabGroup("连接管理")]
        [Button("全部展开", ButtonSizes.Medium)]
        [GUIColor(0.7f, 1f, 0.7f)]
        private void ExpandAll()
        {
            Debug.Log("全部展开连接");
        }

        [TabGroup("连接管理")]
        [Button("全部收起", ButtonSizes.Medium)]
        [GUIColor(1f, 0.7f, 0.7f)]
        private void CollapseAll()
        {
            Debug.Log("全部收起连接");
        }

        [TabGroup("连接管理")]
        [Button("清空全部", ButtonSizes.Medium)]
        [GUIColor(1f, 0.5f, 0.5f)]
        private void ClearAllConnectionsInspector()
        {
            ClearAllConnections();
        }

        [TabGroup("连接管理")]
        [LabelWidth(80)]
        [LabelText("连线样式"),SerializeField] 
        private ConnectionStyle _batchStyle = ConnectionStyle.Solid;

        [TabGroup("连接管理")]
        [Button("应用样式", ButtonSizes.Medium)]
        [GUIColor(0.7f, 0.7f, 1f)]
        private void ApplyBatchStyle()
        {
            SetAllConnectionsStyle(_batchStyle);
        }

        [TabGroup("连接管理")]
        [LabelWidth(80)]
        [LabelText("连线颜色"),SerializeField] 
        private Color _batchColor = Color.white;

        [TabGroup("连接管理")]
        [Button("应用颜色", ButtonSizes.Medium)]
        [GUIColor(0.7f, 1f, 1f)]
        private void ApplyBatchColor()
        {
            foreach (var connection in connections)
            {
                connection.startColor = _batchColor;
                connection.endColor = _batchColor;
                connection.isDirty = true;
            }
            needsRebuild = true;
        }

        [TabGroup("性能设置")]
        [LabelText("是否允许剔除"),PropertySpace(SpaceAfter = 5)]
        public bool enableCulling = true;

        [TabGroup("性能设置")]
        [ShowIf("enableCulling")]
        [Range(100, 5000)]
        [LabelText("剔除距离"),SuffixLabel("pixels")]
        public float cullingDistance = 2000f;

        [TabGroup("性能设置")]
        [Range(1, 100)]
        [LabelText("最大同时渲染线数量"),SuffixLabel("lines/frame")]
        public int maxLinesPerFrame = 50;

        [TabGroup("性能设置")]
        [Range(3, 50)]
        [LabelText("渲染质量设置"),SuffixLabel("segments")]
        public int globalCurveResolution = 20;

        [TabGroup("性能设置")]
        [ShowInInspector]
        [ReadOnly]
        [LabelText("当前连接数")]
        private int ConnectionCount => connections.Count;

        [TabGroup("性能设置")]
        [ShowInInspector]
        [ReadOnly]
        [LabelText("需要重建")]
        private bool NeedsRebuild => needsRebuild;

        [TabGroup("调试")]
        [ShowInInspector]
        [ReadOnly]
        [LabelText("脏连接数")]
        private int DirtyConnectionCount
        {
            get
            {
                int count = 0;
                foreach (var connection in connections)
                {
                    if (connection.isDirty) count++;
                }
                return count;
            }
        }

        [TabGroup("调试")]
        [ShowInInspector]
        [ReadOnly]
        [LabelText("缓存位置数")]
        private int CachedPositionCount => cachedPositions.Count;

        [TabGroup("调试")]
        [Button("强制重建所有", ButtonSizes.Large)]
        [GUIColor(1f, 0.8f, 0.6f)]
        private void ForceRebuildAll()
        {
            SetDirtyAll();
        }

        [TabGroup("调试")]
        [Button("清理缓存", ButtonSizes.Large)]
        [GUIColor(0.8f, 0.8f, 1f)]
        private void ClearCache()
        {
            cachedPositions.Clear();
            foreach (var connection in connections)
            {
                connection.cachedCurvePoints = null;
                connection.isDirty = true;
            }
            needsRebuild = true;
        }

        // 私有字段（不显示在 Inspector 中）
        [HideInInspector]
        private List<Vector2> cachedPositions = new List<Vector2>();
        
        [HideInInspector]
        private int updateIndex = 0;
        
        [HideInInspector]
        private bool needsRebuild = true;
        
        [HideInInspector]
        private float globalAnimationTime = 0f;

        // 对象池
        [HideInInspector]
        private Queue<List<UIVertex>> vertexListPool = new Queue<List<UIVertex>>();
        
        [HideInInspector]
        private Queue<List<int>> triangleListPool = new Queue<List<int>>();

        protected override void Start()
        {
            base.Start();
            // 预热对象池
            for (int i = 0; i < 10; i++)
            {
                vertexListPool.Enqueue(new List<UIVertex>());
                triangleListPool.Enqueue(new List<int>());
            }
        }

        /// <summary>
        /// 更新所有点位
        /// </summary>
        public void UpdateLineRender()
        {
            CheckForMovement();

            if (needsRebuild)
            {
                SetVerticesDirty();
                needsRebuild = false;
            }
        }
        
        void CheckForMovement()
        {
            if (connections.Count == 0) 
                return;

            int checkCount = Mathf.Min(maxLinesPerFrame, connections.Count);
            int startIdx = updateIndex;

            for (int i = 0; i < checkCount; i++)
            {
                int idx = (startIdx + i) % connections.Count;
                var connection = connections[idx];

                if (connection.startPoint == null || connection.endPoint == null)
                    continue;

                if (HasPositionChanged(connection, idx * 2) || HasPositionChanged(connection, idx * 2 + 1))
                {
                    connection.isDirty = true;
                    needsRebuild = true;
                    connection.cachedCurvePoints = null;
                }
            }

            updateIndex = (updateIndex + checkCount) % connections.Count;
        }

        bool HasPositionChanged(ConnectionData connection, int cacheIndex)
        {
            if (cacheIndex >= cachedPositions.Count)
            {
                while (cachedPositions.Count <= cacheIndex)
                    cachedPositions.Add(Vector2.zero);
                return true;
            }

            Vector2 currentPos = cacheIndex % 2 == 0
                ? (Vector2)connection.startPoint.position
                : (Vector2)connection.endPoint.position;

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

                if (enableCulling && !IsLineVisible(connection))
                    continue;

                int addedVertices = AddCurveToMesh(vh, connection, vertexCount);
                vertexCount += addedVertices;

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

            if (Vector2.Distance(localPos1, localPos2) > cullingDistance)
                return false;

            Rect canvasRect = rectTransform.rect;
            return canvasRect.Contains(localPos1) || canvasRect.Contains(localPos2) ||
                   LineIntersectsRect(localPos1, localPos2, canvasRect);
        }

        bool LineIntersectsRect(Vector2 p1, Vector2 p2, Rect rect)
        {
            return !(Mathf.Max(p1.x, p2.x) < rect.xMin ||
                     Mathf.Min(p1.x, p2.x) > rect.xMax ||
                     Mathf.Max(p1.y, p2.y) < rect.yMin ||
                     Mathf.Min(p1.y, p2.y) > rect.yMax);
        }

        int AddCurveToMesh(VertexHelper vh, ConnectionData connection, int vertexOffset)
        {
            Vector2 localPos1, localPos2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.startPoint.position, null, out localPos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.endPoint.position, null, out localPos2);

            // 生成或使用缓存的曲线点
            if (connection.cachedCurvePoints == null || connection.isDirty)
            {
                GenerateCurvePoints(connection, localPos1, localPos2);
            }

            int resolution = connection.curveResolution > 0 ? connection.curveResolution : globalCurveResolution;
            int vertexCount = 0;

            // 根据样式生成不同的网格
            switch (connection.style)
            {
                case ConnectionStyle.Solid:
                    vertexCount = AddSolidCurve(vh, connection, vertexOffset, resolution);
                    break;
                case ConnectionStyle.Dashed:
                    vertexCount = AddDashedCurve(vh, connection, vertexOffset, resolution);
                    break;
                case ConnectionStyle.Dotted:
                    vertexCount = AddDottedCurve(vh, connection, vertexOffset, resolution);
                    break;
                case ConnectionStyle.Wave:
                    vertexCount = AddWaveCurve(vh, connection, vertexOffset, resolution);
                    break;
            }

            // 添加箭头
            if (connection.showArrow)
            {
                vertexCount += AddArrows(vh, connection, vertexOffset + vertexCount);
            }

            return vertexCount;
        }

        void GenerateCurvePoints(ConnectionData connection, Vector2 start, Vector2 end)
        {
            int resolution = connection.curveResolution > 0 ? connection.curveResolution : globalCurveResolution;
            connection.cachedCurvePoints = new Vector2[resolution + 1];

            for (int i = 0; i <= resolution; i++)
            {
                float t = (float)i / resolution;
                connection.cachedCurvePoints[i] = connection.GetPointOnCurve(t, start, end);
            }
        }

        /// <summary>
        /// 线条
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        int AddSolidCurve(VertexHelper vh, ConnectionData connection, int vertexOffset, int resolution)
        {
            Vector2 localPos1, localPos2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.startPoint.position, null, out localPos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.endPoint.position, null, out localPos2);

            int vertexCount = 0;

            for (int i = 0; i < resolution; i++)
            {
                float t1 = (float)i / resolution;
                float t2 = (float)(i + 1) / resolution;

                Vector2 p1 = connection.GetPointOnCurve(t1, localPos1, localPos2);
                Vector2 p2 = connection.GetPointOnCurve(t2, localPos1, localPos2);

                Vector2 direction = (p2 - p1).normalized;
                Vector2 perpendicular = new Vector2(-direction.y, direction.x);

                float width1 = connection.GetWidthAtPoint(t1) * 0.5f;
                float width2 = connection.GetWidthAtPoint(t2) * 0.5f;

                Color color1 = GetAnimatedColor(connection, t1);
                Color color2 = GetAnimatedColor(connection, t2);

                // 创建四边形
                UIVertex v1 = CreateVertex(p1 + perpendicular * width1, color1, new Vector2(t1, 1));
                UIVertex v2 = CreateVertex(p1 - perpendicular * width1, color1, new Vector2(t1, 0));
                UIVertex v3 = CreateVertex(p2 - perpendicular * width2, color2, new Vector2(t2, 0));
                UIVertex v4 = CreateVertex(p2 + perpendicular * width2, color2, new Vector2(t2, 1));

                vh.AddVert(v1);
                vh.AddVert(v2);
                vh.AddVert(v3);
                vh.AddVert(v4);

                int baseIndex = vertexOffset + vertexCount;
                vh.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
                vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex);

                vertexCount += 4;
            }

            return vertexCount;
        }

        /// <summary>
        /// 虚线
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        int AddDashedCurve(VertexHelper vh, ConnectionData connection, int vertexOffset, int resolution)
        {
            Vector2 localPos1, localPos2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.startPoint.position, null, out localPos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.endPoint.position, null, out localPos2);

            float totalLength = GetCurveLength(connection, localPos1, localPos2);
            float dashCycle = connection.dashLength + connection.gapLength;
            int vertexCount = 0;

            for (int i = 0; i < resolution; i++)
            {
                float t = (float)i / resolution;
                float distance = t * totalLength;
                float cyclePos = distance % dashCycle;

                if (cyclePos <= connection.dashLength)
                {
                    // 在虚线段内，添加几何体
                    float t1 = (float)i / resolution;
                    float t2 = (float)(i + 1) / resolution;

                    Vector2 p1 = connection.GetPointOnCurve(t1, localPos1, localPos2);
                    Vector2 p2 = connection.GetPointOnCurve(t2, localPos1, localPos2);

                    Vector2 direction = (p2 - p1).normalized;
                    Vector2 perpendicular = new Vector2(-direction.y, direction.x);

                    float width1 = connection.GetWidthAtPoint(t1) * 0.5f;
                    float width2 = connection.GetWidthAtPoint(t2) * 0.5f;

                    Color color1 = GetAnimatedColor(connection, t1);
                    Color color2 = GetAnimatedColor(connection, t2);

                    UIVertex v1 = CreateVertex(p1 + perpendicular * width1, color1, new Vector2(t1, 1));
                    UIVertex v2 = CreateVertex(p1 - perpendicular * width1, color1, new Vector2(t1, 0));
                    UIVertex v3 = CreateVertex(p2 - perpendicular * width2, color2, new Vector2(t2, 0));
                    UIVertex v4 = CreateVertex(p2 + perpendicular * width2, color2, new Vector2(t2, 1));

                    vh.AddVert(v1);
                    vh.AddVert(v2);
                    vh.AddVert(v3);
                    vh.AddVert(v4);

                    int baseIndex = vertexOffset + vertexCount;
                    vh.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
                    vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex);

                    vertexCount += 4;
                }
            }

            return vertexCount;
        }

        /// <summary>
        /// 点线
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        int AddDottedCurve(VertexHelper vh, ConnectionData connection, int vertexOffset, int resolution)
        {

            Vector2 localPos1, localPos2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.startPoint.position, null, out localPos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.endPoint.position, null, out localPos2);

            float totalLength = GetCurveLength(connection, localPos1, localPos2);
            float dotSpacing = connection.dashLength;
            int dotCount = Mathf.FloorToInt(totalLength / dotSpacing);
            int vertexCount = 0;

            for (int i = 0; i <= dotCount; i++)
            {
                float t = (float)i / dotCount;
                Vector2 point = connection.GetPointOnCurve(t, localPos1, localPos2);
                float dotSize = connection.GetWidthAtPoint(t);
                Color color = GetAnimatedColor(connection, t);

                // 创建圆形点
                vertexCount += AddCircle(vh, point, dotSize * 0.5f, color, vertexOffset + vertexCount, 8);
            }

            return vertexCount;
        }

        /// <summary>
        /// 波浪线
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        int AddWaveCurve(VertexHelper vh, ConnectionData connection, int vertexOffset, int resolution)
        {
            Vector2 localPos1, localPos2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.startPoint.position, null, out localPos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.endPoint.position, null, out localPos2);

            int vertexCount = 0;

            for (int i = 0; i < resolution; i++)
            {
                float t1 = (float)i / resolution;
                float t2 = (float)(i + 1) / resolution;

                Vector2 p1 = connection.GetPointOnCurve(t1, localPos1, localPos2);
                Vector2 p2 = connection.GetPointOnCurve(t2, localPos1, localPos2);

                // 添加波浪偏移
                float waveOffset1 = Mathf.Sin(t1 * Mathf.PI * 8 + globalAnimationTime * 2) * connection.width * 0.3f;
                float waveOffset2 = Mathf.Sin(t2 * Mathf.PI * 8 + globalAnimationTime * 2) * connection.width * 0.3f;

                Vector2 direction = (p2 - p1).normalized;
                Vector2 perpendicular = new Vector2(-direction.y, direction.x);

                p1 += perpendicular * waveOffset1;
                p2 += perpendicular * waveOffset2;

                Vector2 newDirection = (p2 - p1).normalized;
                Vector2 newPerpendicular = new Vector2(-newDirection.y, newDirection.x);

                float width1 = connection.GetWidthAtPoint(t1) * 0.5f;
                float width2 = connection.GetWidthAtPoint(t2) * 0.5f;

                Color color1 = GetAnimatedColor(connection, t1);
                Color color2 = GetAnimatedColor(connection, t2);

                UIVertex v1 = CreateVertex(p1 + newPerpendicular * width1, color1, new Vector2(t1, 1));
                UIVertex v2 = CreateVertex(p1 - newPerpendicular * width1, color1, new Vector2(t1, 0));
                UIVertex v3 = CreateVertex(p2 - newPerpendicular * width2, color2, new Vector2(t2, 0));
                UIVertex v4 = CreateVertex(p2 + newPerpendicular * width2, color2, new Vector2(t2, 1));

                vh.AddVert(v1);
                vh.AddVert(v2);
                vh.AddVert(v3);
                vh.AddVert(v4);

                int baseIndex = vertexOffset + vertexCount;
                vh.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
                vh.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex);

                vertexCount += 4;
            }

            return vertexCount;
        }

        /// <summary>
        /// 添加箭头
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <returns></returns>
        int AddArrows(VertexHelper vh, ConnectionData connection, int vertexOffset)
        {
            Vector2 localPos1, localPos2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.startPoint.position, null, out localPos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, connection.endPoint.position, null, out localPos2);

            int vertexCount = 0;

            if (connection.arrowPosition == ArrowPosition.Start || connection.arrowPosition == ArrowPosition.Both)
            {
                Vector2 direction = connection.GetTangentAtPoint(0, localPos1, localPos2);
                vertexCount += AddArrow(vh, localPos1, direction, connection, vertexOffset + vertexCount);
            }

            if (connection.arrowPosition == ArrowPosition.End || connection.arrowPosition == ArrowPosition.Both)
            {
                Vector2 direction = connection.GetTangentAtPoint(1, localPos1, localPos2);
                vertexCount += AddArrow(vh, localPos2, direction, connection, vertexOffset + vertexCount);
            }

            if (connection.arrowPosition == ArrowPosition.Middle)
            {
                Vector2 midPoint = connection.GetPointOnCurve(0.5f, localPos1, localPos2);
                Vector2 direction = connection.GetTangentAtPoint(0.5f, localPos1, localPos2);
                vertexCount += AddArrow(vh, midPoint, direction, connection, vertexOffset + vertexCount);
            }

            return vertexCount;
        }

        /// <summary>
        /// 添加箭头
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <returns></returns>
        int AddArrow(VertexHelper vh, Vector2 position, Vector2 direction, ConnectionData connection, int vertexOffset)
        {
            switch (connection.arrowType)
            {
                case ArrowType.Triangle:
                    return AddTriangleArrow(vh, position, direction, connection, vertexOffset);
                case ArrowType.Circle:
                    return AddCircle(vh, position, connection.arrowSize * 0.5f, connection.endColor, vertexOffset, 12);
                case ArrowType.Diamond:
                    return AddDiamondArrow(vh, position, direction, connection, vertexOffset);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 添加三角形箭头
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <returns></returns>
        int AddTriangleArrow(VertexHelper vh, Vector2 position, Vector2 direction, ConnectionData connection,
            int vertexOffset)
        {
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            float size = connection.arrowSize;

            Vector2 tip = position + direction * size;
            Vector2 base1 = position - direction * size * 0.5f + perpendicular * size * 0.5f;
            Vector2 base2 = position - direction * size * 0.5f - perpendicular * size * 0.5f;

            UIVertex v1 = CreateVertex(tip, connection.endColor, Vector2.zero);
            UIVertex v2 = CreateVertex(base1, connection.endColor, Vector2.zero);
            UIVertex v3 = CreateVertex(base2, connection.endColor, Vector2.zero);

            vh.AddVert(v1);
            vh.AddVert(v2);
            vh.AddVert(v3);

            vh.AddTriangle(vertexOffset, vertexOffset + 1, vertexOffset + 2);

            return 3;
        }

        /// <summary>
        /// 添加钻石箭头
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="connection"></param>
        /// <param name="vertexOffset"></param>
        /// <returns></returns>
        int AddDiamondArrow(VertexHelper vh, Vector2 position, Vector2 direction, ConnectionData connection,
            int vertexOffset)
        {
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            float size = connection.arrowSize * 0.5f;

            Vector2 top = position + direction * size;
            Vector2 right = position + perpendicular * size;
            Vector2 bottom = position - direction * size;
            Vector2 left = position - perpendicular * size;

            UIVertex v1 = CreateVertex(top, connection.endColor, Vector2.zero);
            UIVertex v2 = CreateVertex(right, connection.endColor, Vector2.zero);
            UIVertex v3 = CreateVertex(bottom, connection.endColor, Vector2.zero);
            UIVertex v4 = CreateVertex(left, connection.endColor, Vector2.zero);

            vh.AddVert(v1);
            vh.AddVert(v2);
            vh.AddVert(v3);
            vh.AddVert(v4);

            vh.AddTriangle(vertexOffset, vertexOffset + 1, vertexOffset + 2);
            vh.AddTriangle(vertexOffset + 2, vertexOffset + 3, vertexOffset);

            return 4;
        }

        /// <summary>
        /// 添加圆点
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        /// <param name="vertexOffset"></param>
        /// <param name="segments"></param>
        /// <returns></returns>
        int AddCircle(VertexHelper vh, Vector2 center, float radius, Color color, int vertexOffset, int segments)
        {
            // 添加中心点
            vh.AddVert(CreateVertex(center, color, Vector2.zero));
            int centerIndex = vertexOffset;
            int vertexCount = 1;

            // 添加圆周点
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2;
                Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                vh.AddVert(CreateVertex(point, color, Vector2.zero));

                if (i > 0)
                {
                    vh.AddTriangle(centerIndex, vertexOffset + vertexCount - 1, vertexOffset + vertexCount);
                }

                vertexCount++;
            }

            return vertexCount;
        }

        /// <summary>
        /// 渲染动画颜色
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        Color GetAnimatedColor(ConnectionData connection, float t)
        {
            Color baseColor = connection.useGradient
                ? Color.Lerp(connection.startColor, connection.endColor, t)
                : connection.startColor;
            return baseColor;
        }

        /// <summary>
        /// 简化的曲线长度计算
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        float GetCurveLength(ConnectionData connection, Vector2 start, Vector2 end)
        {
            if (connection.curveType == ConnectionCurveType.Straight)
                return Vector2.Distance(start, end);

            float length = 0f;
            Vector2 lastPoint = start;
            int segments = 20;

            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector2 currentPoint = connection.GetPointOnCurve(t, start, end);
                length += Vector2.Distance(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }

            return length;
        }

        UIVertex CreateVertex(Vector3 position, Color color, Vector2 uv)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.position = position;
            vertex.color = color;
            vertex.uv0 = uv;
            return vertex;
        }

        #region 公共API--基础连接管理

        /// <summary>
        /// 添加连线
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        ///</summary>
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

        /// <summary>
        /// 添加曲线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="curveType"></param>
        /// <param name="curvature"></param>
        /// <param name="width"></param>
        /// <param name="startColor"></param>
        /// <param name="endColor"></param>
        /// <param name="useGradient"></param>
        /// <returns></returns>
        public int AddCurvedConnection(RectTransform start, RectTransform end,
            ConnectionCurveType curveType = ConnectionCurveType.Bezier,
            float curvature = 0.5f, float width = 5f,
            Color? startColor = null, Color? endColor = null, bool useGradient = false)
        {
            var connection = new ConnectionData
            {
                startPoint = start,
                endPoint = end,
                curveType = curveType,
                curvature = curvature,
                width = width,
                startColor = startColor ?? Color.white,
                endColor = endColor ?? (startColor ?? Color.white),
                useGradient = useGradient
            };

            connections.Add(connection);
            needsRebuild = true;

            return connections.Count - 1;
        }

        /// <summary>
        /// 带箭头的连接API
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="arrowType"></param>
        /// <param name="arrowPosition"></param>
        /// <param name="arrowSize"></param>
        /// <param name="width"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public int AddArrowConnection(RectTransform start, RectTransform end,
            ArrowType arrowType = ArrowType.Triangle,
            ArrowPosition arrowPosition = ArrowPosition.End,
            float arrowSize = 10f, float width = 5f,
            Color? color = null)
        {
            var connection = new ConnectionData
            {
                startPoint = start,
                endPoint = end,
                width = width,
                startColor = color ?? Color.white,
                endColor = color ?? Color.white,
                showArrow = true,
                arrowType = arrowType,
                arrowPosition = arrowPosition,
                arrowSize = arrowSize
            };

            connections.Add(connection);
            needsRebuild = true;

            return connections.Count - 1;
        }
        
        public void RemoveConnection()
        {
            
        }
        
        /// <summary>
        /// 移除连线--通过索引
        /// </summary>
        /// <param name="index"></param>
        public void RemoveConnection(int index)
        {
            if (index >= 0 && index < connections.Count)
            {
                connections.RemoveAt(index);
                needsRebuild = true;
            }
        }

        public void RemoveConnection(ConnectionData connection)
        {
            
        }
        
        /// <summary>
        /// 更新连线
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newData"></param>
        public void UpdateConnection(ConnectionData newData)
        {
            if (connections.Contains(newData))
            {
                
            }
        }
        
        /// <summary>
        /// 更新连线
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newData"></param>
        public void UpdateConnection(int index, ConnectionData newData)
        {
            if (index >= 0 && index < connections.Count)
            {
                connections[index] = newData;
                connections[index].isDirty = true;
                needsRebuild = true;
            }
        }

        #endregion


        // 批量更新连接属性
        public void UpdateConnectionStyle(int index, ConnectionStyle style, float dashLength = 10f,
            float gapLength = 5f)
        {
            if (index >= 0 && index < connections.Count)
            {
                var connection = connections[index];
                connection.style = style;
                connection.dashLength = dashLength;
                connection.gapLength = gapLength;
                connection.isDirty = true;
                needsRebuild = true;
            }
        }

        public void UpdateConnectionCurve(int index, ConnectionCurveType curveType, float curvature = 0.5f)
        {
            if (index >= 0 && index < connections.Count)
            {
                var connection = connections[index];
                connection.curveType = curveType;
                connection.curvature = curvature;
                connection.cachedCurvePoints = null; // 清除缓存
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

        public void SetDirtyAll()
        {
            foreach (var connection in connections)
            {
                connection.isDirty = true;
                connection.cachedCurvePoints = null;
            }

            needsRebuild = true;
        }

        public void SetCullingSettings(bool enable, float distance)
        {
            enableCulling = enable;
            cullingDistance = distance;
            needsRebuild = true;
        }

        public void SetGlobalCurveResolution(int resolution)
        {
            globalCurveResolution = Mathf.Max(3, resolution);
            SetDirtyAll();
        }

        // 获取连接信息
        public ConnectionData GetConnection(int index)
        {
            if (index >= 0 && index < connections.Count)
                return connections[index];
            return null;
        }

        public int GetConnectionCount()
        {
            return connections.Count;
        }

        // 查找连接
        public int FindConnection(RectTransform start, RectTransform end)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];
                if (connection.startPoint == start && connection.endPoint == end)
                    return i;
            }

            return -1;
        }

        // 批量操作
        public void SetAllConnectionsStyle(ConnectionStyle style)
        {
            foreach (var connection in connections)
            {
                connection.style = style;
                connection.isDirty = true;
            }

            needsRebuild = true;
        }
        
        private void AddNewConnection()
        {
            var newConnection = new ConnectionData();
            connections.Add(newConnection);
            needsRebuild = true;
        }

        // 自定义移除连接方法
        private void RemoveConnectionAt(int index)
        {
            if (index >= 0 && index < connections.Count)
            {
                connections.RemoveAt(index);
                needsRebuild = true;
            }
        }
        
        [TabGroup("调试")]
        [Button("验证所有连接", ButtonSizes.Medium)]
        [GUIColor(0.6f, 1f, 0.6f)]
        private void ValidateAllConnections()
        {
            int invalidCount = 0;
            for (int i = 0; i < connections.Count; i++)
            {
                var connection = connections[i];
                if (connection.startPoint == null || connection.endPoint == null)
                {
                    Debug.LogWarning($"连接 {i} 的起点或终点为空");
                    invalidCount++;
                }
            }
            
            if (invalidCount == 0)
            {
                Debug.Log("所有连接都有效");
            }
            else
            {
                Debug.LogWarning($"发现 {invalidCount} 个无效连接");
            }
        }
    }
}