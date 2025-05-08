using System.Collections.Generic;
using System.Linq;
using GDFrameworkExtend.PoolKit;
using UnityEngine;

namespace GDFramework.World
{
    public class QuadTree<T>
    {
        /// <summary>
        /// 四叉树的叶子
        /// </summary>
        public class Leaf
        {
            public Branch Branch;

            public T Value;

            public AABB_Check Aabb;
        }

        /// <summary>
        /// 四叉树的枝干
        /// </summary>
        public class Branch
        {
            /// <summary>
            /// 所属的四叉树
            /// </summary>
            public QuadTree<T> BelongTree;

            /// <summary>
            /// 父枝干
            /// </summary>
            public Branch Parent;

            /// <summary>
            /// 该枝干
            /// </summary>
            public AABB_Check Aabb;

            /// <summary>
            /// 分支的四象限的AABB
            /// （先创建对象：空间换时间，省去后续的大量 != null 判断）
            /// </summary>
            public AABB_Check[] Aabbs = new AABB_Check[4];

            /// <summary>
            /// 分支的枝干
            /// </summary>
            public Branch[] Branches = new Branch[4];

            /// <summary>
            /// 拥有的叶子
            /// </summary>
            public List<Leaf> LeaveList = new List<Leaf>();

            /// <summary>
            /// 横跨多个枝干的叶子
            /// </summary>
            public List<Leaf> CrossBranchesLeaveList = new List<Leaf>();

            /// <summary>
            /// 枝干的深度
            /// </summary>
            public int Depth;

            /// <summary>
            /// 是否再次分过枝干
            /// </summary>
            public bool HasSplit;

            /// <summary>
            /// 是否被回收了
            /// </summary>
            public bool HasRecycled;
        }

        /// <summary>
        /// 四叉树可以设置的深度
        /// </summary>
        public const int MaxLimitDepthLevel = 32;

        /// <summary>
        /// 默认的最大深度
        /// </summary>
        public const int DefaulDepthLevel = 10;

        /// <summary>
        /// 默认一个枝干最大的叶子量
        /// </summary>
        public const int DefaultMaxLeafPerBranch = 50;

        /// <summary>
        /// 剔除距离
        /// 默认为无限大
        /// </summary>
        public float CullingDistance = float.NaN;

        /// <summary>
        /// 叶子表
        /// </summary>
        private Dictionary<T, Leaf> LeavesDict = new Dictionary<T, Leaf>();

        /// <summary>
        /// 根枝干
        /// </summary>
        public Branch Root;

        /// <summary>
        /// 最大层级
        /// </summary>
        public int MaxLevel;

        /// <summary>
        /// 叶子达到该数量时
        /// 会再次划分枝干
        /// </summary>
        private int _maxLeafPerBranch;

        private bool[] _insertHelper = new bool[4];

        private Stack<Branch> _branchesPool = new Stack<Branch>();

        private Stack<Leaf> _leafsPool = new Stack<Leaf>();

        private List<AABB_Check> _aabbHelpersPool = ListPool<AABB_Check>.Get();

        private Dictionary<T, AABB_Check> _dirtyLeaves = new Dictionary<T, AABB_Check>();

        // 更新叶子方法
        public void UpdateLeaf(T value, AABB_Check newAabb)
        {
            if (LeavesDict.TryGetValue(value, out Leaf leaf))
            {
                if (!newAabb.Equals(leaf.Aabb))
                {
                    // 记录旧AABB用于后续移除
                    _dirtyLeaves[value] = leaf.Aabb; 
                    Insert(value, newAabb);
                }
            }
        }

        // 批量移除旧位置的叶子
        public void ProcessDirtyLeaves()
        {
            foreach (var kvp in _dirtyLeaves)
            {
                RemoveLeaf(kvp.Key, kvp.Value);
            }
            _dirtyLeaves.Clear();
        }

        // 新增移除方法
        private void RemoveLeaf(T value, AABB_Check oldAabb)
        {
            if (LeavesDict.TryGetValue(value, out Leaf leaf))
            {
                if (leaf.Branch != null)
                {
                    leaf.Branch.LeaveList.Remove(leaf);
                    leaf.Branch.CrossBranchesLeaveList.Remove(leaf);
                }
                RecycleLeafToPool(leaf);
                LeavesDict.Remove(value);
            }
        }
        
        /// <summary>
        /// 构建四叉树
        /// </summary>
        /// <param name="aabb">整个四叉树的最大AABB</param>
        /// <param name="maxDepth">四叉树的最大深度</param>
        /// <param name="maxLeafPerBranch">四叉树单个叶子的最大数量</param>
        public QuadTree(
            AABB_Check aabb,
            int maxDepth = DefaulDepthLevel,
            int maxLeafPerBranch = DefaultMaxLeafPerBranch) : this(aabb.X, aabb.Y, aabb.W, aabb.H, maxDepth,
            maxLeafPerBranch)
        {
            
        }

        /// <summary>
        /// 构建四叉树
        /// </summary>
        /// <param name="x">整个四叉树的最大AABB的X</param>
        /// <param name="y">整个四叉树的最大AABB的Y</param>
        /// <param name="w">整个四叉树的最大AABB的W</param>
        /// <param name="h">整个四叉树的最大AABB的B</param>
        public QuadTree(float x, float y, float w, float h,
            int maxDepth = DefaulDepthLevel,
            int maxLeafPerBranch = DefaultMaxLeafPerBranch)
        {
            Reset(x, y, w, h, maxDepth, maxLeafPerBranch);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void DisposeThis()
        {
            Root = null;
            if (LeavesDict != null)
            {
                LeavesDict.Clear();
                LeavesDict = null;
            }

            if (_branchesPool != null)
            {
                _branchesPool.Clear();
                _branchesPool = null;
            }

            if (_leafsPool != null)
            {
                _leafsPool.Clear();
                _leafsPool = null;
            }

            if (_aabbHelpersPool != null)
            {
                ListPool<AABB_Check>.Release(_aabbHelpersPool);
                _aabbHelpersPool = null;
            }
        }

        /// <summary>
        /// 清理内部枝干结构到最初的状态
        /// </summary>
        public void Clear()
        {
            if (Root != null)
            {
                var src_aabb = Root.Aabb;
                RecycleBranchToPool(Root);
                LeavesDict.Clear();

                Root = GetBranchFromPool(null, 0, ref src_aabb);
            }
        }

        /// <summary>
        /// 插入（目前使用与 静态 QT 树的逻辑写法，所以插入处理成本比较大，但是在 Select 会大大提升性能）
        /// 如果要写动态 QT，那么 Insert 需要重新写一套逻辑，要尽可能的简单分割处理，那么时候可以另起一个测试项目来编写处理
        /// 现在的静态版本 QT 的插入时处理了跨 多枝干 的存放到父级节点的优化处理的，如果动态版本的话就不需要了
        /// 但是需要外部对筛选结果的去重
        /// </summary>
        /// <param name="value">插入的对象</param>
        /// <param name="aabb">插入对象对应的aabb</param>
        /// <returns>插入成功返回 true</returns>
        public bool Insert(T value, AABB_Check aabb)
        {
            return Insert(value, ref aabb);
        }

        /// <summary>
        /// 插入（目前使用与 静态 QT 树的逻辑写法，所以插入处理成本比较大，但是在 Select 会大大提升性能）
        /// 如果要写动态 QT，那么 Insert 需要重新写一套逻辑，要尽可能的简单分割处理，那么时候可以另起一个测试项目来编写处理
        /// 现在的静态版本 QT 的插入时处理了跨 多枝干 的存放到父级节点的优化处理的，如果动态版本的话就不需要了
        /// 但是需要外部对筛选结果的去重
        /// </summary>
        /// <param name="value">插入的对象</param>
        /// <param name="aabb">插入对象对应的aabb</param>
        /// <returns>插入成功返回 true</returns>
        public bool Insert(T value, ref AABB_Check aabb)
        {
            Leaf leaf;
            if (LeavesDict.TryGetValue(value, out leaf))
            {
                leaf.Aabb = aabb;
            }
            else
            {
                leaf = GetLeafFromPool(value, ref aabb);
                LeavesDict[value] = leaf;
            }

            return Insert(Root, leaf);
        }

        /// <summary>
        /// aabb范围筛选
        /// </summary>
        /// <param name="aabb">筛选的aabb范围</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        public void Select(AABB_Check aabb, List<T> ret)
        {
            Select(ref aabb, ret);
        }

        /// <summary>
        /// aabb范围筛选
        /// </summary>
        /// <param name="aabb">筛选的aabb范围</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        /// <param name="moreActuallySelect">是否进行更精准一些的筛选</param>
        public void Select(ref AABB_Check aabb, List<T> ret, bool moreActuallySelect = true)
        {
            ret.Clear();
            List<T> compeleteInAABB_ret = ListPool<T>.Get();
            
            SelectByAABB(ref aabb, ret, compeleteInAABB_ret, Root);

            if (moreActuallySelect)
            {
                for (int i = ret.Count - 1; i > -1; i--)
                {
                    if (!aabb.IsIntersect(LeavesDict[ret[i]].Aabb))
                    {
                        ret.RemoveAt(i);
                    }
                }
            }
            if (compeleteInAABB_ret.Count > 0)
            {
                ret.AddRange(compeleteInAABB_ret);
            }
            
            ListPool<T>.Release(compeleteInAABB_ret);
        }

        /// <summary>
        /// aabb范围筛选
        /// </summary>
        /// <param name="x">筛选的aabb范围的x</param>
        /// <param name="y">筛选的aabb范围的y</param>
        /// <param name="w">筛选的aabb范围的w</param>
        /// <param name="h">筛选的aabb范围的h</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        public void Select(float x, float y, float w, float h, List<T> ret)
        {
            Select(new AABB_Check { X = x, Y = y, W = w, H = h }, ret);
        }

        /// <summary>
        /// 点选
        /// </summary>
        /// <param name="pos">点选坐标</param>
        /// <param name="ret">点选的结构存放列表对象</param>
        public void Select(Vector2 pos, List<T> ret)
        {
            Select(ref pos, ret);
        }

        /// <summary>
        /// 点选
        /// </summary>
        /// <param name="pos">点选坐标</param>
        /// <param name="ret">点选的结构存放列表对象</param>
        /// <param name="moreActuallySelect">是否进行更精准一些的筛选</param>
        public void Select(ref Vector2 pos, List<T> ret, bool moreActuallySelect = true)
        {
            ret.Clear();
            SelectByPos(ref pos, ret, Root);
            if (moreActuallySelect)
            {
                for (int i = ret.Count - 1; i > -1; i--)
                {
                    if (!LeavesDict[ret[i]].Aabb.Contains(pos))
                    {
                        ret.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 点选
        /// </summary>
        /// <param name="x">点选坐标x</param>
        /// <param name="y">点选坐标y</param>
        /// <param name="ret">点选的结构存放列表对象</param>
        public void Select(float x, float y, List<T> ret)
        {
            Select(new Vector2(x, y), ret);
        }

        /// <summary>
        /// 多选
        /// </summary>
        /// <param name="aabbs">多个aabb范围</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        /// <param name="moreActuallySelect">是否进行更精准一些的筛选（注意：还是 AABB 的概要筛选，如果需要精准到自定义的碰撞筛选级别，这里最好传入 false，再让外部去筛选, 会增加消耗）</param>
        public void Select(List<AABB_Check> aabbs, List<T> ret, bool moreActuallySelect = true)
        {
            ret.Clear();
            List<T> compeleteInAABB_ret = ListPool<T>.Get();
            SelectByAABBS(aabbs, ret, compeleteInAABB_ret, Root);

            if (moreActuallySelect)
            {
                for (int i = ret.Count - 1; i > -1; i--)
                {
                    var intersect = false;
                    foreach (var aabb in aabbs)
                    {
                        if (aabb.IsIntersect(LeavesDict[ret[i]].Aabb))
                        {
                            intersect = true;
                            break;
                        }
                    }
                    if (!intersect)
                    {
                        ret.RemoveAt(i);
                    }
                }
            }

            if (compeleteInAABB_ret.Count > 0)
            {
                ret.AddRange(compeleteInAABB_ret);
            }
            ListPool<T>.Release(compeleteInAABB_ret);
        }

        /// <summary>
        /// aabb范围筛选，并根据 relactivePos 点与 QuadTree 的 cullingDistance 做距离剔除
        /// </summary>
        /// <param name="aabb">筛选的aabb范围</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        /// <param name="selectFromPos">与 QuadTree 的 cullingDistance 做距离剔除 的相关的点</param>
        public void Select(AABB_Check aabb, List<T> ret, Vector2 selectFromPos)
        {
            Select(ref aabb, ret, selectFromPos);
        }

        /// <summary>
        /// aabb范围筛选
        /// </summary>
        /// <param name="aabb">筛选的aabb范围</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        /// <param name="selectFromPos">与 QuadTree 的 cullingDistance 做距离剔除 的相关的点</param>
        /// <param name="moreActuallySelect">是否进行更精准一些的筛选</param>
        public void Select(ref AABB_Check aabb, List<T> ret, Vector2 selectFromPos, bool moreActuallySelect = true)
        {
            if (IsCullingDistance(ref aabb, selectFromPos))
            {
                ret.Clear();
                return;
            }

            Select(ref aabb, ret, moreActuallySelect);
        }

        /// <summary>
        /// aabb范围筛选
        /// </summary>
        /// <param name="x">筛选的aabb范围的x</param>
        /// <param name="y">筛选的aabb范围的y</param>
        /// <param name="w">筛选的aabb范围的w</param>
        /// <param name="h">筛选的aabb范围的h</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        /// <param name="selectFromPos">与 QuadTree 的 cullingDistance 做距离剔除 的相关的点</param>
        public void Select(float x, float y, float w, float h, List<T> ret, Vector2 selectFromPos)
        {
            if (IsCullingDistance(new AABB_Check { X = x, Y = y, W = w, H = h }, selectFromPos))
            {
                ret.Clear();
                return;
            }

            Select(x, y, w, h, ret);
        }

        /// <summary>
        /// 点选
        /// </summary>
        /// <param name="pos">点选坐标</param>
        /// <param name="ret">点选的结构存放列表对象</param>
        /// <param name="selectFromPos">与 QuadTree 的 cullingDistance 做距离剔除 的相关的点</param>
        public void Select(Vector2 pos, List<T> ret, Vector2 selectFromPos)
        {
            if (IsCullingDistance(pos, selectFromPos))
            {
                ret.Clear();
                return;
            }

            Select(ref pos, ret);
        }

        public void Select(ref Vector2 pos, List<T> ret, Vector2 selectFromPos, bool moreActuallySelect = true)
        {
            if (IsCullingDistance(pos, selectFromPos))
            {
                ret.Clear();
                return;
            }

            Select(ref pos, ret, moreActuallySelect);
        }

        /// <summary>
        /// 点选
        /// </summary>
        /// <param name="x">点选坐标x</param>
        /// <param name="y">点选坐标y</param>
        /// <param name="ret">点选的结构存放列表对象</param>
        /// <param name="selectFromPos">与 QuadTree 的 cullingDistance 做距离剔除 的相关的点</param>
        public void Select(float x, float y, List<T> ret, Vector2 selectFromPos)
        {
            Select(new Vector2(x, y), ret, selectFromPos);
        }

        /// <summary>
        /// 多选
        /// </summary>
        /// <param name="aabbs">多个aabb范围</param>
        /// <param name="ret">筛选的结构存放列表对象</param>
        /// <param name="selectFromPos">与 QuadTree 的 cullingDistance 做距离剔除 的相关的点</param>
        /// <param name="moreActuallySelect">是否进行更精准一些的筛选（注意：还是 AABB 的概要筛选，如果需要精准到自定义的碰撞筛选级别，这里最好传入 false，再让外部去筛选, 会增加消耗）</param>
        public void Select(List<AABB_Check> aabbs, List<T> ret, Vector2 selectFromPos, bool moreActuallySelect = true)
        {
            if (aabbs.Count == 0)
            {
                return;
            }

            _aabbHelpersPool.Clear();
            _aabbHelpersPool.AddRange(aabbs);
            for (int i = 0; i < aabbs.Count; i++)
            {
                if (IsCullingDistance(_aabbHelpersPool[i], selectFromPos))
                {
                    _aabbHelpersPool.RemoveRange(i, aabbs.Count - i);
                    break;
                }
            }

            if (_aabbHelpersPool.Count == 0)
            {
                return;
            }

            Select(_aabbHelpersPool, ret, moreActuallySelect);
        }

        private bool IsCullingDistance(Vector2 selectPos, Vector2 selectFormPos)
        {
            // culling by distance
            return IsCullingDistance(ref selectPos, selectFormPos);
        }

        private bool IsCullingDistance(ref Vector2 selectPos, Vector2 selectFormPos)
        {
            // culling by distance
            var cd = CullingDistance;
            if (!float.IsNaN(cd) && cd > 0)
            {
                var pow2cd = cd * cd;
                if (pow2cd < Vector2.SqrMagnitude(selectPos - selectFormPos))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCullingDistance(AABB_Check aabb, Vector2 relactivePos)
        {
            return IsCullingDistance(ref aabb, relactivePos);
        }

        private bool IsCullingDistance(ref AABB_Check aabb, Vector2 relactivePos)
        {
            // culling by distance
            var cd = CullingDistance;
            if (!float.IsNaN(cd) && cd > 0)
            {
                var pow2cd = cd * cd;

                var tl = aabb.TopLeft;
                var tr = aabb.TopRight;
                var br = aabb.BottomRight;
                var bl = aabb.BottomLeft;

                var culling_count = 0;
                if (pow2cd < Vector2.SqrMagnitude(tl - relactivePos))
                {
                    culling_count++;
                }

                if (pow2cd < Vector2.SqrMagnitude(tr - relactivePos))
                {
                    culling_count++;
                }

                if (pow2cd < Vector2.SqrMagnitude(br - relactivePos))
                {
                    culling_count++;
                }

                if (pow2cd < Vector2.SqrMagnitude(bl - relactivePos))
                {
                    culling_count++;
                }

                return (culling_count == 4);
            }

            return false;
        }

        private void SelectByAABB(ref AABB_Check aabb, List<T> ret, List<T> compeleteInAABB_ret, Branch branch)
        {
            if (branch == null)
            {
                return;
            }
            
            if (branch.Aabb.Contains(ref aabb))
            {
                // 完整包含了底下枝干的 aabb
                SelectAllValues(branch, compeleteInAABB_ret);
            }
            else
            {
                // 与部分的交集
                if (branch.Aabb.IsIntersect(ref aabb))
                {
                    foreach (var l in branch.LeaveList)
                    {
                        ret.Add(l.Value);
                    }
                    foreach (var l in branch.CrossBranchesLeaveList)
                    {
                        ret.Add(l.Value);
                    }
                    foreach (var b in branch.Branches)
                    {
                        SelectByAABB(ref aabb, ret, compeleteInAABB_ret, b);
                    }
                }
            }
        }
        
        private void SelectAllValues(Branch branch, List<T> ret)
        {
            if (branch == null)
            {
                return;
            }
            
            if (branch != null)
            {
                ret.AddRange(branch.LeaveList.Select(l => l.Value));
                ret.AddRange(branch.CrossBranchesLeaveList.Select(l => l.Value));
                foreach (var b in branch.Branches) 
                    SelectAllValues(b, ret);
            }

        }

        private void SelectByPos(ref Vector2 pos, List<T> ret, Branch branch)
        {
            if (branch == null)
            {
                return;
            }
            
            if (branch.Aabb.Contains(ref pos))
            {
                foreach (var l in branch.LeaveList)
                {
                    ret.Add(l.Value);
                }

                foreach (var l in branch.CrossBranchesLeaveList)
                {
                    ret.Add(l.Value);
                }

                foreach (var b in branch.Branches)
                {
                    SelectByPos(ref pos, ret, b);
                }
            }

        }

    private void SelectByAABBS(List<AABB_Check> aabbs, List<T> ret, List<T> compeleteInAABB_ret, Branch branch)
    {
        if (branch == null)
        {
            return;
        }

        if (branch.Aabb.AnyContainsBy(aabbs))
        {
            // 完整包含了底下枝干的 aabb
            SelectAllValues(branch, compeleteInAABB_ret);
        }
        else
        {
            // 与部分的交集
            if (branch.Aabb.AnyIntersect(aabbs))
            {
                foreach (var l in branch.LeaveList)
                {
                    ret.Add(l.Value);
                }
                foreach (var l in branch.CrossBranchesLeaveList)
                {
                    ret.Add(l.Value);
                }
                foreach (var b in branch.Branches)
                {
                    SelectByAABBS(aabbs, ret, compeleteInAABB_ret, b);
                }
            }
        }
    }

        private void SelectByAABBS(List<AABB_Check> aabbs, List<T> ret, Branch branch)
        {
            if (branch == null)
            {
                return;
            }

            if (branch.Aabb.AnyContainsBy(aabbs))
            {
                // 完整包含了底下枝干的 aabb
                SelectAllValues(branch, ret);
            }
            else
            {
                // 与部分的交集
                if (branch.Aabb.AnyIntersect(aabbs))
                {
                    foreach (var l in branch.LeaveList)
                    {
                        ret.Add(l.Value);
                    }

                    foreach (var l in branch.CrossBranchesLeaveList)
                    {
                        ret.Add(l.Value);
                    }

                    foreach (var b in branch.Branches)
                    {
                        SelectByAABBS(aabbs, ret, b);
                    }
                }
            }
        }

        private bool Insert(Branch branch, Leaf leaf)
        {
            var ret = false;
            if (!branch.Aabb.IsIntersect(ref leaf.Aabb))
            {
                // 不在该枝干管理范围外，则不处理插入
                //ret = false;
            }
            else
            {
                if (branch.HasSplit)
                {
                    // 将之前 叶子 的插入到 子枝干 上去
                    if (branch.LeaveList.Count > 0)
                    {
                        SrcLeavesInsertToSubBranches(branch);
                    }

                    ret = InsertSingleLeaf(branch, leaf);
                }
                else
                {
                    if (branch.Depth <= MaxLevel && (branch.LeaveList.Count + branch.CrossBranchesLeaveList.Count) >=
                        _maxLeafPerBranch)
                    {
                        // 已达最大深度限制，已超过对应的数量，那么再次细分该枝干
                        branch.HasSplit = true;
                        ret = Insert(branch, leaf);
                    }
                    else
                    {
                        // 未达最大深度限制，未超过对应的数量，那么插入该枝干
                        branch.LeaveList.Add(leaf);
                        leaf.Branch = branch;
                        ret = true;
                    }
                }
            }
            return ret;
        }

        private void SrcLeavesInsertToSubBranches(Branch branch)
        {
            for (int i = 0; i < branch.LeaveList.Count; i++)
            {
                var l = branch.LeaveList[i];
                InsertSingleLeaf(branch, l);
            }

            branch.LeaveList.Clear();
        }

        private bool InsertSingleLeaf(Branch branch, Leaf leaf)
        {
            var contains_with_branch_count = 0;
            var branch_idx = -1;
            for (int i = 0; i < 4; i++)
            {
                if (branch.Aabbs[i].IsIntersect(ref leaf.Aabb))
                {
                    contains_with_branch_count++;
                    _insertHelper[i] = true;
                    branch_idx = i;
                    if (branch.Branches[i] == null || branch.Branches[i].HasRecycled)
                    {
                        branch.Branches[i] = GetBranchFromPool(branch, branch.Depth + 1, ref branch.Aabbs[i]);
                    }
                }
            }

            var ret = false;
            if (contains_with_branch_count > 1)
            {
                // 与多个枝干有交集，就插入到父级的枝干上
                // 这里就不判断 maxLeafPerBranch 的限制了
                branch.CrossBranchesLeaveList.Add(leaf);
                leaf.Branch = branch;
                ret = true;
            }
            else
            {
                // 插入到对应的枝干上
                //System.Diagnostics.Debug.Assert(branch_idx > -1 && branch_idx < 4);
                if (branch_idx < 0 || branch_idx > 3) branch_idx = 3;
                ret = Insert(branch.Branches[branch_idx], leaf);
            }
            return ret;
        }

        private Branch GetBranchFromPool(Branch parent, int depth, ref AABB_Check aabb)
        {
            var ret = _branchesPool.Count > 0 ? _branchesPool.Pop() : new Branch();
            ret.BelongTree = this;
            ret.Parent = parent;
            ret.Depth = depth;
            ret.Aabb = aabb;
            ret.HasRecycled = false;
            float halfW = aabb.W * 0.5f;
            float halfH = aabb.H * 0.5f;
            float midX = aabb.X + halfW;
            float midY = aabb.Y + halfH;
            ret.Aabbs[0].Set(aabb.X, aabb.Y, halfW, halfH); // top-left
            ret.Aabbs[1].Set(midX, aabb.Y, halfW, halfH); // top-right
            ret.Aabbs[2].Set(midX, midY, halfW, halfH); // bottom-right
            ret.Aabbs[3].Set(aabb.X, midY, halfW, halfH); // bottom-left
            return ret;
        }

        private void RecycleBranchToPool(Branch branch)
        {
            if (branch == null)
            {
                return;
            }

            branch.BelongTree = null;
            branch.Parent = null;
            branch.HasSplit = false;
            branch.HasRecycled = true;
            _branchesPool.Push(branch);

            foreach (var l in branch.LeaveList)
            {
                RecycleLeafToPool(l);
            }

            branch.LeaveList.Clear();
            foreach (var l in branch.CrossBranchesLeaveList)
            {
                RecycleLeafToPool(l);
            }

            branch.CrossBranchesLeaveList.Clear();

            
            for (int i = 0; i < branch.Branches.Length; i++)
            {
                RecycleBranchToPool(branch.Branches[i]);
                branch.Branches[i] = null;
            }
        }

        private Leaf GetLeafFromPool(T value, ref AABB_Check aabb)
        {
            var ret = _leafsPool.Count > 0 ? _leafsPool.Pop() : new Leaf();
            ret.Value = value;
            ret.Aabb = aabb;
            return ret;
        }

        private void RecycleLeafToPool(Leaf leaf)
        {
            leaf.Branch = null;
            leaf.Value = default;
            _leafsPool.Push(leaf);
        }

        private void Reset(float x, float y, float w, float h,
            int maxLevel = DefaulDepthLevel, int maxLeafPerBranch = DefaultMaxLeafPerBranch)
        {
            System.Diagnostics.Debug.Assert(w != 0 || h != 0, "四叉树为Zero");
            System.Diagnostics.Debug.Assert(maxLevel < MaxLimitDepthLevel, $"四叉树无法超过这个深度: {MaxLimitDepthLevel}");

            this.MaxLevel = maxLevel;
            this._maxLeafPerBranch = maxLeafPerBranch;

            var aabb = new AABB_Check() { X = x, Y = y, W = w, H = h };
            if (Root != null)
            {
                Root.Aabb = aabb;
            }
            else
            {
                Root = GetBranchFromPool(null, 0, ref aabb);
            }
        }
        
        /// <summary>
        /// 动态扩展根节点AABB
        /// </summary>
        /// <param name="newAabb"></param>
        public void ExpandRoot(AABB_Check newAabb)
        {
            if (Root == null || !Root.Aabb.Contains(ref newAabb))
            {
                var unionAabb = Root.Aabb;
                unionAabb.Union(ref newAabb);
                Reset(unionAabb.X, unionAabb.Y, unionAabb.W, unionAabb.H, MaxLevel, _maxLeafPerBranch);
            }
        }
    }
}