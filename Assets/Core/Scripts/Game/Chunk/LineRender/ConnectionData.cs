using System;
using UnityEngine;

namespace Core.World
{
    [Serializable]
    public class ConnectionData
    {
        [Header("连接点")]
        public RectTransform startPoint;
        public RectTransform endPoint;
        
        [Header("基础属性")]
        public float width = 5f;
        public Color startColor = Color.white;
        public Color endColor = Color.white;
        public bool useGradient = false;
        
        [Header("曲线设置")]
        public ConnectionCurveType curveType = ConnectionCurveType.Bezier;
        public float curvature = 0.5f; // 曲线弯曲程度 0-1
        public Vector2 controlPointOffset = Vector2.zero; // 控制点偏移
        public bool autoCalculateControlPoints = true; // 自动计算控制点
        
        [Header("样式设置")]
        public ConnectionStyle style = ConnectionStyle.Solid;
        public float dashLength = 10f; // 虚线长度
        public float gapLength = 5f;   // 虚线间隔
        public AnimationCurve widthCurve = AnimationCurve.Linear(0, 1, 1, 1); // 宽度变化曲线
        
        [Header("箭头")]
        public bool showArrow = false;
        public ArrowType arrowType = ArrowType.Triangle;
        public float arrowSize = 10f;
        public ArrowPosition arrowPosition = ArrowPosition.End;
        
        [System.NonSerialized]
        public bool isDirty = true; // 标记是否需要更新
        
        [System.NonSerialized]
        public float animationTime = 0f; // 动画时间
        
        [System.NonSerialized]
        public Vector2[] cachedCurvePoints; // 缓存的曲线点
        
        [System.NonSerialized]
        public int curveResolution = 20; // 曲线分辨率
        
        // 获取曲线上的点
        public Vector2 GetPointOnCurve(float t, Vector2 start, Vector2 end)
        {
            switch (curveType)
            {
                case ConnectionCurveType.Straight:
                    return Vector2.Lerp(start, end, t);
                    
                case ConnectionCurveType.Bezier:
                    return GetBezierPoint(t, start, end);
                    
                case ConnectionCurveType.Arc:
                    return GetArcPoint(t, start, end);
                    
                case ConnectionCurveType.Spline:
                    return GetSplinePoint(t, start, end);
                    
                default:
                    return Vector2.Lerp(start, end, t);
            }
        }
        
        private Vector2 GetBezierPoint(float t, Vector2 start, Vector2 end)
        {
            Vector2 control1, control2;
            
            if (autoCalculateControlPoints)
            {
                Vector2 direction = (end - start);
                Vector2 perpendicular = new Vector2(-direction.y, direction.x).normalized;
                float distance = direction.magnitude;
                
                control1 = start + direction * 0.25f + perpendicular * distance * curvature;
                control2 = end - direction * 0.25f + perpendicular * distance * curvature;
            }
            else
            {
                control1 = start + controlPointOffset;
                control2 = end - controlPointOffset;
            }
            
            // 三次贝塞尔曲线
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            
            Vector2 point = uuu * start;
            point += 3 * uu * t * control1;
            point += 3 * u * tt * control2;
            point += ttt * end;
            
            return point;
        }
        
        private Vector2 GetArcPoint(float t, Vector2 start, Vector2 end)
        {
            Vector2 center = (start + end) * 0.5f;
            Vector2 direction = (end - start);
            Vector2 perpendicular = new Vector2(-direction.y, direction.x).normalized;
            
            float radius = direction.magnitude * 0.5f;
            float arcHeight = radius * curvature;
            
            Vector2 arcCenter = center + perpendicular * arcHeight;
            
            float angle = Mathf.Lerp(0, Mathf.PI, t);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * arcHeight;
            
            return arcCenter + new Vector2(x, y);
        }
        
        private Vector2 GetSplinePoint(float t, Vector2 start, Vector2 end)
        {
            // 简化的样条曲线，可以后续扩展为更复杂的实现
            Vector2 mid = (start + end) * 0.5f;
            Vector2 direction = (end - start);
            Vector2 perpendicular = new Vector2(-direction.y, direction.x).normalized;
            
            mid += perpendicular * direction.magnitude * curvature * Mathf.Sin(t * Mathf.PI);
            
            return Vector2.Lerp(Vector2.Lerp(start, mid, t * 2), Vector2.Lerp(mid, end, (t - 0.5f) * 2), t);
        }
        
        // 获取曲线上指定点的切线方向
        public Vector2 GetTangentAtPoint(float t, Vector2 start, Vector2 end)
        {
            float delta = 0.01f;
            Vector2 p1 = GetPointOnCurve(Mathf.Max(0, t - delta), start, end);
            Vector2 p2 = GetPointOnCurve(Mathf.Min(1, t + delta), start, end);
            return (p2 - p1).normalized;
        }
        
        // 获取当前宽度（基于宽度曲线）
        public float GetWidthAtPoint(float t)
        {
            return width * widthCurve.Evaluate(t);
        }
    }
    
    public enum ConnectionCurveType
    {
        Straight,   // 直线
        Bezier,     // 贝塞尔曲线
        Arc,        // 圆弧
        Spline      // 样条曲线
    }
    
    public enum ConnectionStyle
    {
        Solid,      // 实线
        Dashed,     // 虚线
        Dotted,     // 点线
        Wave        // 波浪线
    }
    
    public enum ArrowType
    {
        Triangle,       // 三角形箭头
        Circle,         // 圆形箭头
        Diamond,        // 菱形箭头
        Custom          // 自定义箭头
    }
    
    public enum ArrowPosition
    {
        Start,          // 起点
        End,            // 终点
        Both,           // 两端
        Middle          // 中间
    }
}