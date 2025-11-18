using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.View.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using TMPro;
using UnityEngine;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 地图编辑器基类
    /// TParentDef: 父级 DtoDef 类型 (如 UniverseDtoDef)
    /// TChildDef: 子级 DtoDef 类型 (如 WorldDtoDef)
    /// TNode: 地图节点类型 (如 UI_EditorDetail_UniverseMapNode)
    /// </summary>
    public abstract class UI_EditorDetail_Map<TParentDef, TChildDef, TNode> 
        : UI_Details, ICanGetModel, ICanGetSystem, IUIViewScaleable
        where TParentDef : ChunkDtoDef
        where TChildDef : ChunkDtoDef
        where TNode : UI_EditorDetail_MapNode<TChildDef>
    {
        #region 组件引用
        
        protected Transform _contentRoot;
        protected GameObject _mapNodePrefab;
        protected TextMeshProUGUI _initialName;
        protected EditorDataManager _editorDataManager;
        
        #endregion

        #region 数据管理
        
        protected List<TNode> _mapNodeList = new List<TNode>();
        protected TNode _curFocusNode;
        protected TNode _curInitialNode;
        protected TParentDef _currentParentDef;
        
        #endregion

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit()
        {
            InitializeComponents();
            InitializeModels();
        }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        protected virtual void InitializeComponents()
        {
            _contentRoot = transform.Find("ContentRoot");
            _mapNodePrefab = transform.Find("MapNodePrefab").gameObject;
            _initialName = transform.Find("InitialName").GetComponent<TextMeshProUGUI>();
            _editorDataManager = this.GetSystem<EditorDataManager>();
        }

        /// <summary>
        /// 初始化数据模型 (子类实现)
        /// </summary>
        protected abstract void InitializeModels();

        protected override void OnShow() { }
        protected override void OnStart() { }
        protected override void OnClose() { }

        #region 地图显示

        /// <summary>
        /// 显示地图
        /// </summary>
        public virtual void ShowMap(TParentDef parentDef)
        {
            _currentParentDef = parentDef;
            ClearMap();

            var childIds = GetChildIds(parentDef);
            var initialId = GetInitialChildId(parentDef);

            foreach (var childId in childIds)
            {
                TChildDef childDef = GetChildDef(childId);
                if (childDef != null)
                {
                    AddMapNode(childDef, initialId);
                }
            }
        }

        /// <summary>
        /// 添加地图节点
        /// </summary>
        public virtual TNode AddMapNode(TChildDef childDef, string initialId)
        {
            TNode node = CreateNode(childDef);
            if (node == null) return null;

            node.SetDto(this, childDef);
            
            if (childDef.DefId.Equals(initialId))
            {
                node.SetThisNodeAsInitial();
            }

            _mapNodeList.Add(node);
            StartTrackingNode(childDef);

            return node;
        }

        /// <summary>
        /// 创建节点实例 (子类实现)
        /// </summary>
        protected virtual TNode CreateNode(TChildDef childDef)
        {
            GameObject nodeObj = Instantiate(_mapNodePrefab, _contentRoot);
            TNode node = nodeObj.GetComponent<TNode>();
            if (node != null)
            {
                node.Show();
            }
            return node;
        }

        /// <summary>
        /// 清空地图
        /// </summary>
        protected virtual void ClearMap()
        {
            foreach (var node in _mapNodeList)
            {
                node?.SetDestroy();
            }
            _mapNodeList.Clear();
            _curFocusNode = null;
            _curInitialNode = null;
        }

        #endregion

        #region 节点管理

        /// <summary>
        /// 管理节点选中状态
        /// </summary>
        public virtual void ManageNodeSelect(TNode selectedNode)
        {
            foreach (var node in _mapNodeList)
            {
                node.ChangeSelectingNode(node == selectedNode);
            }
            _curFocusNode = selectedNode;
        }

        /// <summary>
        /// 更新初始节点
        /// </summary>
        public virtual void UpdateInitialNode(TNode newInitialNode)
        {
            foreach (var node in _mapNodeList)
            {
                node.ChangeInitialNode(node == newInitialNode);
            }

            _curInitialNode = newInitialNode;
            _initialName.text = newInitialNode.GetDtoDef().DefName;

            // 更新父级数据
            SetInitialChildId(_currentParentDef, newInitialNode.GetDtoDef().DefId);
        }

        #endregion

        #region 数据获取

        /// <summary>
        /// 获取当前焦点节点的 Def
        /// </summary>
        public TChildDef GetCurFocusDef() => _curFocusNode?.GetDtoDef();

        /// <summary>
        /// 获取当前初始节点的 Def
        /// </summary>
        public TChildDef GetCurInitialDef() => _curInitialNode?.GetDtoDef();

        /// <summary>
        /// 获取所有节点
        /// </summary>
        public List<TNode> GetAllNodes() => _mapNodeList;

        /// <summary>
        /// 获取所有节点ID
        /// </summary>
        public List<string> GetCurOwnedDtoDefId()
        {
            var ids = new List<string>();
            foreach (var node in _mapNodeList)
            {
                ids.Add(node.GetDtoDef().DefId);
            }
            return ids;
        }

        /// <summary>
        /// 获取指定锁定状态的节点ID列表
        /// </summary>
        public List<string> GetCurIsLockingDtoDefID(bool isLocking)
        {
            var ids = new List<string>();
            foreach (var node in _mapNodeList)
            {
                if (node.GetThisNodeIsLocking() == isLocking)
                {
                    ids.Add(node.GetDtoDef().DefId);
                }
            }
            return ids;
        }

        #endregion

        #region 抽象方法 (子类必须实现)

        /// <summary>
        /// 获取子级Def列表
        /// </summary>
        protected abstract List<string> GetChildIds(TParentDef parentDef);

        /// <summary>
        /// 获取初始子级ID
        /// </summary>
        protected abstract string GetInitialChildId(TParentDef parentDef);

        /// <summary>
        /// 设置初始子级ID
        /// </summary>
        protected abstract void SetInitialChildId(TParentDef parentDef, string childId);

        /// <summary>
        /// 根据ID获取子级Def
        /// </summary>
        protected abstract TChildDef GetChildDef(string defId);

        /// <summary>
        /// 开始追踪节点数据
        /// </summary>
        protected abstract void StartTrackingNode(TChildDef childDef);

        #endregion
    }
}