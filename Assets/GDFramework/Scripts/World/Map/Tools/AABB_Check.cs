using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDFramework.World
{
    public class AABB_Check : IEquatable<AABB_Check>
    {
        public static readonly AABB_Check Zero = new AABB_Check();

        /// <summary>
        /// 获取相机视体的AABB的垂直方向的分层级别,默认是3个级别
        /// </summary>
        public const int DefaultGetCameraFrustumToAABBLevel = 3;

        /// <summary>
        /// 获取相机视体的AABB的水平方向的边距,默认是unity的0个units的大小
        /// </summary>
        public const float DefaultGetCameraFrustumToAABBHorPadding = 0;

        public float X, Y, W, H;
        
        public float Left
        {
            get => X;
            set => X = value;
        }

        public float Top
        {
            get => Y;
            set => Y = value;
        }

        public float Right
        {
            get => X + W;
            set => W = value - X;
        }

        public float Bottom
        {
            get => Y + H;
            set => H = value - Y;
        }

        public float CenterX
        {
            get => X + W * 0.5f;
            set => X = value - (W * 0.5f);
        }

        public float CenterY
        {
            get => Y + H * 0.5f;
            set => Y = value - (H * 0.5f);
        }

        public Vector2 Center
        {
            get => new Vector2(CenterX, CenterY);
            set
            {
                CenterX = value.x;
                CenterY = value.y;
            }
        }
        
        public float ExtentX
        {
            get => W * 0.5f;
            set => W = value * 2;
        }
        
        public float ExtentY
        {
            get => Y * 0.5f;
            set => H = value * 2;
        }

        public Vector2 Extent
        {
            get => new Vector2(ExtentX, ExtentY);
            set
            {
                ExtentX = value.x;
                ExtentY = value.y;
            }
        }

        public Vector2 Min
        {
            get => new Vector2(Left, Top);
            set
            {
                Left = value.x;
                Top = value.y;
            }
        }

        public Vector2 Max
        {
            get => new Vector2(Right, Bottom);
            set
            {
                Right = value.x;
                Bottom = value.y;
            }
        }

        public Vector2 TopLeft
        {
            get => new Vector2(Left, Top);
        }

        public Vector2 TopRight
        {
            get => new Vector2(Left, Top);
        }

        public Vector2 BottomLeft
        {
            get => new Vector2(Left, Bottom);
        }
        
        public Vector2 BottomRight
        {
            get => new Vector2(Right, Bottom);
        }

        public bool IsZero()
        {
            return Left == Right || Top == Bottom;
        }

        public void Set(float x, float y, float w, float h)
        {
            this.X = x;
            this.Y = y;
            this.W = w;
            this.H = h;
        }

        /// <summary>
        /// 当前 AABB 与 其他 的 AABB 是否有交集,并返回交集的 AABB
        /// </summary>
        /// <param name="other"></param>
        /// <param name="outAABB">返回交集的 AABB</param>
        /// <returns>如果当前 AABB 与 other 的 AABB 是否有交集，则返回 true</returns>
        public bool IsIntersect(ref AABB_Check other, out AABB_Check outAABB)
        {
            outAABB = new AABB_Check();
            outAABB.X = MathF.Max(Left, other.Left);
            outAABB.Right = MathF.Min(Right, other.Right);
            outAABB.Y = MathF.Max(Top, other.Top);
            outAABB.Bottom = MathF.Min(Bottom, other.Bottom);
            return !outAABB.IsZero();
        }

        /// <summary>
        /// 当前 AABB 与 其他 的 AABB 是否有交集
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsIntersect(ref AABB_Check other)
        {
            return X < other.Right && Y < other.Bottom && Right > other.X && Bottom > other.Y;
        }

        /// <summary>
        /// 当前 AABB 与 其他 的 AABB 是否有交集
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsIntersect(AABB_Check other)
        {
            return IsIntersect(ref other);
        }

        /// <summary>
        /// 是否完整包含另一个 AABB
        /// 做优化用,一般如果整个 AABB 都被另一个 AABB 包含就不用精确检测了
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Contains(ref AABB_Check other)
        {
            return other.X >= X && other.Y >= Y && other.Right <= Right && other.Bottom <= Bottom;
        }

        /// <summary>
        /// 是否完整包含另一个 AABB
        /// 做优化用,一般如果整个 AABB 都被另一个 AABB 包含就不用精确检测了
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Contains(AABB_Check other)
        {
            return Contains(ref other);
        }
        
        /// <summary>
        /// 是否包含一个 2D 点
        /// </summary>
        /// <param name="x">2D 点 x</param>
        /// <param name="y">2D 点 x</param>
        /// <returns>如果包含 2D 点，则返回 true</returns>
        public bool Contains(float x, float y)
        {
            return x >= Left && x < Right && y >= Top && y < Bottom;
        }
        
        /// <summary>
        /// 是否包含一个 2D 点
        /// </summary>
        /// <param name="pos">2D 点</param>
        /// <returns>如果包含 2D 点，则返回 true</returns>
        public bool Contains(Vector2 pos)
        {
            return Contains(ref pos);
        }
        
        public bool Contains(ref Vector2 pos)
        {
            return Contains(pos.x, pos.y);
        }
        public void Union(AABB_Check aabb)
        {
            Union(ref aabb);
        }
        /// <summary>
        /// 并集一个 AABB
        /// </summary>
        /// <param name="aabb">需要与之并集的 AABB</param>
        public void Union(ref AABB_Check aabb)
        {
            Union(aabb.Min);
            Union(aabb.Max);
        }
        /// <summary>
        /// 并集一个 点
        /// </summary>
        /// <param name="pos"></param>
        public void Union(Vector2 pos)
        {
            Union(ref pos);
        }
        
        public void Union(ref Vector2 pos)
        {
            Union(pos.x, pos.y);
        }
        
        public void Union(Vector3 pos)
        {
            Union(ref pos);
        }
        
        public void Union(ref Vector3 pos)
        {
            Union(pos.x, pos.z);
        }
        
        public void Union(float _x, float _z)
        {
            var src_x = X;
            var src_y = Y;
            var src_r = Right;
            var src_b = Bottom;

            X = MathF.Min(src_x,        _x);
            X = MathF.Min(X,            src_r);

            Y = MathF.Min(src_y,        _z);
            Y = MathF.Min(Y,            src_b);

            Right = MathF.Max(src_x,    _x);
            Right = MathF.Max(Right,    src_r);

            Bottom = MathF.Max(src_y,   _z);
            Bottom = MathF.Max(Bottom,  src_b);
        }
        
    /// <summary>
    /// 与多个 aabbs 是否有任意的并集
    /// </summary>
    /// <param name="aabbs">多个 aabbs</param>
    /// <returns>如果有任意的并集，返回 true</returns>
    public bool AnyIntersect(List<AABB_Check> aabbs)
    {
        foreach (var aabb in aabbs)
        {
            if (IsIntersect(aabb))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否被 多个 aabbs 中的其中一个全包含
    /// </summary>
    /// <param name="aabbs">多个 aabbs</param>
    /// <returns>如果被其中一个全包含，返回 true</returns>
    public bool AnyContainsBy(List<AABB_Check> aabbs)
    {
        foreach (var aabb in aabbs)
        {
            if (aabb.Contains(this))
            {
                return true;
            }
        }
        return false;
    }
    
    public bool Equals(AABB_Check other)
    {
        return X == other.X && Y == other.Y && W == other.W && H == other.H;
    }
    /// <summary>
    /// jave.lin : 让x,y为最小值，right,bottom 为最大值
    /// 因为部分 w, h 可以为负数，那么再部分计算就不太方便，所以可以统一转换成 x,y < w,h 的格式
    /// </summary>
    public void Reorder()
    {
        var src_x = X;
        var src_y = Y;
        var src_r = Right;
        var src_b = Bottom;

        X = Mathf.Min(src_x, src_r);
        Y = Mathf.Min(src_y, src_b);

        X = Mathf.Max(src_x, src_r);
        Y = Mathf.Max(src_y, src_b);
    }
    /// <summary>
    /// 获取 Camera 分层 level 的多个 aabb，如果 Camera 是一个正交投影，那么会无视 level 数值，直接返回一个 aabb
    /// </summary>
    /// <param name="cam">要获取多个 aabb 的 Camera</param>
    /// <param name="ret">结果</param>
    /// <param name="level">将 frustum 分解的层级数量</param>
    /// <param name="h_padding">添加水平边界间隔</param>
    public static void GetCameraAABBs(Camera cam, List<AABB_Check> ret, 
        int level = DefaultGetCameraFrustumToAABBLevel, float h_padding = DefaultGetCameraFrustumToAABBHorPadding)
    {
        ret.Clear();
        if (cam.orthographic)
        {
            var aabb = new AABB_Check();
            GetOrthorCameraAABB(cam, ref aabb, h_padding);
            ret.Add(aabb);
        }
        else
        {
            GetFrustumCameraAABBs(cam, ret, level, h_padding);
        }
    }
    
    public static void GetOrthorCameraAABB(Camera cam, ref AABB_Check aabb, float h_padding = DefaultGetCameraFrustumToAABBHorPadding)
    {
        System.Diagnostics.Debug.Assert(cam.orthographic == true);
        var far             = cam.farClipPlane;
        var near            = cam.nearClipPlane;
        var delta_fn        = far - near;
        var half_height     = cam.orthographicSize;
        var half_with       = cam.aspect * half_height;
        var forward         = cam.transform.forward;
        var right           = cam.transform.right;
        var up              = cam.transform.up;
        var start_pos       = cam.transform.position + forward * near;
        var top_left        = start_pos + forward * delta_fn + (-right * half_with) + (up * half_height);
        var top_right       = top_left + (right * (2 * half_with));
        var bottom_right    = top_right + (-up * (2 * half_height));
        var bottom_left     = bottom_right + (-right * (2 * half_with));

        var h_padding_vec   = right * h_padding;

        top_left            -= h_padding_vec;
        top_right           += h_padding_vec;
        bottom_right        += h_padding_vec;
        bottom_left         -= h_padding_vec;

        // 重置
        aabb.W = aabb.H = 0;
        aabb.X = start_pos.x;
        aabb.Y = start_pos.z;

        // 并集其他点
        aabb.Union(ref top_left);
        aabb.Union(ref top_right);
        aabb.Union(ref bottom_right);
        aabb.Union(ref bottom_left);
    }
    public static void GetFrustumCameraAABBs(Camera cam, List<AABB_Check> aabbs, int level = DefaultGetCameraFrustumToAABBLevel, float h_padding = DefaultGetCameraFrustumToAABBHorPadding)
    {
        // 计算椎体分段包围盒
        System.Diagnostics.Debug.Assert(cam.orthographic == false);
        System.Diagnostics.Debug.Assert(level > 0);
        // 相机的 frustum 如果构建，可以参考我以前的一篇文章：https://blog.csdn.net/linjf520/article/details/104761121#OnRenderImage_98
        var far                     = cam.farClipPlane;
        var near                    = cam.nearClipPlane;
        var tan                     = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var far_plane_half_height   = tan * far;
        var far_plane_half_with     = cam.aspect * far_plane_half_height;
        var near_plane_half_height  = tan * near;
        var near_plane_half_with    = cam.aspect * near_plane_half_height;

        var forward                 = cam.transform.forward;
        var right                   = cam.transform.right;
        var up                      = cam.transform.up;

        var far_top_left            = cam.transform.position + forward * far + (-right * far_plane_half_with) + (up * far_plane_half_height);
        var far_top_right           = far_top_left + (right * (2 * far_plane_half_with));
        var far_bottom_right        = far_top_right + (-up * (2 * far_plane_half_height));
        var far_bottom_left         = far_bottom_right + (-right * (2 * far_plane_half_with));

        var near_top_left           = cam.transform.position + forward * near + (-right * near_plane_half_with) + (up * near_plane_half_height);
        var near_top_right          = near_top_left + (right * (2 * near_plane_half_with));
        var near_bottom_right       = near_top_right + (-up * (2 * near_plane_half_height));
        var near_bottom_left        = near_bottom_right + (-right * (2 * near_plane_half_with));

        var n2f_top_left_vec        = far_top_left - near_top_left;
        var n2f_top_right_vec       = far_top_right - near_top_right;
        var n2f_bottom_right_vec    = far_bottom_right - near_bottom_right;
        var n2f_bottom_left_vec     = far_bottom_left - near_bottom_left;

        var h_padding_vec           = right * h_padding;

        for (int i = 0; i < level; i++)
        {
            var rate_start          = (float)i / level;
            var rate_end            = (float)(i + 1) / level;

            // near plane 四个角点
            var top_left_start      = near_top_left     + n2f_top_left_vec      * rate_start;
            var top_right_start     = near_top_right    + n2f_top_right_vec     * rate_start;
            var bottom_right_start  = near_bottom_right + n2f_bottom_right_vec  * rate_start;
            var bottom_left_start   = near_bottom_left  + n2f_bottom_left_vec   * rate_start;

            // 水平 padding
            top_left_start          -= h_padding_vec;
            top_right_start         += h_padding_vec;
            bottom_right_start      += h_padding_vec;
            bottom_left_start       -= h_padding_vec;

            // far plane 四个角点
            var top_left_end        = near_top_left     + n2f_top_left_vec      * rate_end;
            var top_right_end       = near_top_right    + n2f_top_right_vec     * rate_end;
            var bottom_right_end    = near_bottom_right + n2f_bottom_right_vec  * rate_end;
            var bottom_left_end     = near_bottom_left  + n2f_bottom_left_vec   * rate_end;

            // 水平 padding
            top_left_end            -= h_padding_vec;
            top_right_end           += h_padding_vec;
            bottom_right_end        += h_padding_vec;
            bottom_left_end         -= h_padding_vec;

            var aabb = new AABB_Check();
            aabb.Set(top_left_start.x, top_left_start.z, 0, 0);

            // 并集其他点
            aabb.Union(ref top_left_start);
            aabb.Union(ref top_right_start);
            aabb.Union(ref bottom_right_start);
            aabb.Union(ref bottom_left_start);
            aabb.Union(ref top_left_end);
            aabb.Union(ref top_right_end);
            aabb.Union(ref bottom_right_end);
            aabb.Union(ref bottom_left_end);
            aabbs.Add(aabb);
        }
    }
    /// <summary>
    /// Unity 的 Bounds 隐式转为 我们自己定义的 QTAABB 便于外部书写
    /// </summary>
    /// <param name="v">Unity 的 Bounds</param>
    public static implicit operator AABB_Check(Bounds v)
    {
        var b_min = v.min;
        var b_max = v.max;
        return new AABB_Check 
        {
            Min = new Vector2(b_min.x, b_min.z),
            Max = new Vector2(b_max.x, b_max.z),
        };
    }
    /// <summary>
    /// 便于 VS 中断点调式的简要显示 title 信息
    /// </summary>
    /// <returns>返回：便于 VS 中断点调式的简要显示 title 信息</returns>
    public override string ToString()
    {
        return base.ToString() + $", x:{X}, y:{Y}, w:{W}, h:{H}, right:{Right}, bottom:{Bottom}";
    }
    }
}