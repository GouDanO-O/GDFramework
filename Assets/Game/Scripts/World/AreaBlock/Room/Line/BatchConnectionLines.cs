using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game.World
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class BatchConnectionLines : MaskableGraphic
    {
        [Header("连接线数据")] public List<ConnectionData> connections = new List<ConnectionData>();

        [Header("性能优化")] public bool enableCulling = true;
        public float cullingDistance = 2000f;
        public int maxLinesPerFrame = 50;

        [Header("全局设置")] public int globalCurveResolution = 20; // 全局曲线分辨率
        public bool enableGlobalAnimation = true;

        private List<Vector2> cachedPositions = new List<Vector2>();
        private int updateIndex = 0;
        private bool needsRebuild = true;
        private float globalAnimationTime = 0f;

        // 对象池
        private Queue<List<UIVertex>> vertexListPool = new Queue<List<UIVertex>>();
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

        void Update()
        {
            UpdateLineRender();
        }

        private void UpdateLineRender()
        {
            // 更新全局动画时间
            if (enableGlobalAnimation)
            {
                globalAnimationTime += Time.deltaTime;
                UpdateAnimations();
            }

            // 分帧检查连接点是否移动
            CheckForMovement();

            if (needsRebuild)
            {
                SetVerticesDirty();
                needsRebuild = false;
            }
        }

        void UpdateAnimations()
        {
            bool hasAnimatedConnections = false;

            foreach (var connection in connections)
            {
                if (connection.enableAnimation && connection.animationType != ConnectionAnimation.None)
                {
                    connection.animationTime += Time.deltaTime * connection.animationSpeed;
                    hasAnimatedConnections = true;
                }
            }

            if (hasAnimatedConnections)
            {
                needsRebuild = true;
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

                if (HasPositionChanged(connection, idx * 2) || HasPositionChanged(connection, idx * 2 + 1))
                {
                    connection.isDirty = true;
                    needsRebuild = true;
                    // 清除缓存的曲线点
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

        int AddDashedCurve(VertexHelper vh, ConnectionData connection, int vertexOffset, int resolution)
        {
            // 虚线实现
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

        int AddDottedCurve(VertexHelper vh, ConnectionData connection, int vertexOffset, int resolution)
        {
            // 点线实现
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

        int AddWaveCurve(VertexHelper vh, ConnectionData connection, int vertexOffset, int resolution)
        {
            // 波浪线实现
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

        Color GetAnimatedColor(ConnectionData connection, float t)
        {
            Color baseColor = connection.useGradient
                ? Color.Lerp(connection.startColor, connection.endColor, t)
                : connection.startColor;

            if (!connection.enableAnimation)
                return baseColor;

            switch (connection.animationType)
            {
                case ConnectionAnimation.Flow:
                {
                    float flowPos = (connection.animationTime + t) % 1f;
                    float intensity = Mathf.Sin(flowPos * Mathf.PI * 2) * 0.5f + 0.5f;
                    return Color.Lerp(baseColor, Color.white, intensity * 0.3f);
                }
                case ConnectionAnimation.Pulse:
                {
                    float pulse = Mathf.Sin(connection.animationTime * 2) * 0.5f + 0.5f;
                    return Color.Lerp(baseColor, Color.white, pulse * 0.5f);
                }
                case ConnectionAnimation.Glow:
                {
                    float glow = Mathf.Sin(connection.animationTime + t * Mathf.PI) * 0.5f + 0.5f;
                    baseColor.a *= glow;
                    return baseColor;
                }
                case ConnectionAnimation.Dash:
                {
                    float dashPos = (connection.animationTime * 2 + t * 4) % 2f;
                    float alpha = dashPos < 1f ? 1f : 0.3f;
                    baseColor.a *= alpha;
                    return baseColor;
                }
                default:
                    return baseColor;
            }
        }

        float GetCurveLength(ConnectionData connection, Vector2 start, Vector2 end)
        {
            // 简化的曲线长度计算
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

        // 公共API - 基础连接管理
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

        // 高级连接API
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

        // 带动画的连接API
        public int AddAnimatedConnection(RectTransform start, RectTransform end,
            ConnectionAnimation animationType = ConnectionAnimation.Flow,
            float animationSpeed = 1f, float width = 5f,
            Color? color = null)
        {
            var connection = new ConnectionData
            {
                startPoint = start,
                endPoint = end,
                width = width,
                startColor = color ?? Color.white,
                endColor = color ?? Color.white,
                enableAnimation = true,
                animationType = animationType,
                animationSpeed = animationSpeed
            };

            connections.Add(connection);
            needsRebuild = true;

            return connections.Count - 1;
        }

        // 带箭头的连接API
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

        // 完全自定义连接API
        public int AddCustomConnection(ConnectionData connectionData)
        {
            connections.Add(connectionData);
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

        public void UpdateConnection(int index, ConnectionData newData)
        {
            if (index >= 0 && index < connections.Count)
            {
                connections[index] = newData;
                connections[index].isDirty = true;
                needsRebuild = true;
            }
        }

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

        public void UpdateConnectionAnimation(int index, ConnectionAnimation animationType, float speed = 1f)
        {
            if (index >= 0 && index < connections.Count)
            {
                var connection = connections[index];
                connection.enableAnimation = animationType != ConnectionAnimation.None;
                connection.animationType = animationType;
                connection.animationSpeed = speed;
                connection.animationTime = 0f;
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

        public void SetAllConnectionsAnimation(ConnectionAnimation animationType, float speed = 1f)
        {
            foreach (var connection in connections)
            {
                connection.enableAnimation = animationType != ConnectionAnimation.None;
                connection.animationType = animationType;
                connection.animationSpeed = speed;
                connection.animationTime = UnityEngine.Random.Range(0f, 1f); // 随机起始时间
                connection.isDirty = true;
            }

            needsRebuild = true;
        }
    }
}