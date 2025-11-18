using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using Core.Game.View.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 宇宙星图编辑器
    /// </summary>
    public class UI_EditorDetail_UniverseMap : UI_Details,ICanGetModel,ICanGetSystem,IUIViewScaleable
    {
        private Transform _contentRoot;
        
        private GameObject _mapNodePrefab;
        
        private List<UI_EditorDetail_UniverseMapNode> _universeMapWorldNodeList = new List<UI_EditorDetail_UniverseMapNode>();
        
        private WorldDataModel _worldDataModel;

        private UI_EditorDetail_UniverseMapNode _curFocusNode;

        private UI_EditorDetail_UniverseMapNode _curInitialNode;

        private EditorDataManager _editorDataManager;
        
        private TextMeshProUGUI _initialName;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        protected override void OnInit()
        {
            _contentRoot = transform.Find("ContentRoot");
            _mapNodePrefab = transform.Find("MapNodePrefab").gameObject;
            
            _initialName = transform.Find("InitialName").GetComponent<TextMeshProUGUI>();

            _editorDataManager = this.GetSystem<EditorDataManager>();
            _worldDataModel = this.GetModel<WorldDataModel>();
        }
        
        protected override void OnShow()
        {
            
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnClose()
        {
            
        }

        /// <summary>
        /// 展示宇宙星图
        /// </summary>
        public void ShowMap(UniverseDtoDef universeDtoDef)
        {
            ClearMap();

            for (int i = 0; i < universeDtoDef.WorldIdList.Count; i++)
            {
                string initialWorldId = universeDtoDef.InitialPlayerLocateWorldId;
                string worldId =  universeDtoDef.WorldIdList[i];
                WorldDtoDef worldDtoDef = _worldDataModel.GetDefById(worldId);
                AddMapNode(worldDtoDef,initialWorldId);
                _editorDataManager.StartTrackingWorld(worldDtoDef);
            }
        }

        /// <summary>
        /// 添加世界
        /// </summary>
        /// <param name="worldDtoDef"></param>
        public void AddMapNode(WorldDtoDef worldDtoDef,string initialWorldId)
        {
            UI_EditorDetail_UniverseMapNode node = Instantiate(_mapNodePrefab, _contentRoot.transform)
                .GetComponent<UI_EditorDetail_UniverseMapNode>().Show();
            
            node.SetWorldDto(this,worldDtoDef);
            if (worldDtoDef.DefId.Equals(initialWorldId))
            {
                node.SetThisWorldAsInitialWorld();
            }
            _universeMapWorldNodeList.Add(node);

        }

        public List<UI_EditorDetail_UniverseMapNode> GetCurUniverseWorldNodes()
        {
            return _universeMapWorldNodeList;
        }

        /// <summary>
        /// 清空星图
        /// </summary>
        private void ClearMap()
        {
            for (int i = 0; i < _universeMapWorldNodeList.Count; i++)
            {
                _universeMapWorldNodeList[i].SetDestroy();
            }
            _universeMapWorldNodeList.Clear();
        }

        /// <summary>
        /// 管理星图中的世界节点的点击
        /// </summary>
        /// <param name="mapNode"></param>
        public void ManageNodeSelect(UI_EditorDetail_UniverseMapNode mapNode)
        {
            for (int i = 0; i < _universeMapWorldNodeList.Count; i++)
            {
                UI_EditorDetail_UniverseMapNode curNode =  _universeMapWorldNodeList[i];
                if (curNode != mapNode)
                {
                    curNode.ChangeSelecting(false);
                }
            }

            _curFocusNode = mapNode;
        }

        /// <summary>
        /// 设置世界作为初始世界
        /// </summary>
        /// <param name="worldNode"></param>
        public void UpdateInitialNode(UI_EditorDetail_UniverseMapNode mapNode)
        {
            for (int i = 0; i < _universeMapWorldNodeList.Count; i++)
            {
                UI_EditorDetail_UniverseMapNode curNode =  _universeMapWorldNodeList[i];
                if (curNode != mapNode)
                {
                    curNode.ChangeInitialWorld(false);
                }
            }

            _initialName.text = mapNode.GetThisNodeDtoDef().DefName;
            _curInitialNode = mapNode;

            _editorDataManager.GetFocusedUniverse().InitialPlayerLocateWorldId =
                mapNode.GetThisNodeDtoDef().DefId;
        }

        /// <summary>
        /// 获取当前焦点世界
        /// </summary>
        /// <returns></returns>
        public WorldDtoDef GetCurFocusNodeDtoDef()
        {
            return _curFocusNode.GetThisNodeDtoDef();
        }

        /// <summary>
        /// 获取当前初始世界
        /// </summary>
        /// <returns></returns>
        public WorldDtoDef GetCurInitialNodeDtoDef()
        {
            return _curInitialNode.GetThisNodeDtoDef();
        }

        /// <summary>
        /// 获取当前所有的世界ID
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurOwnedWorldDtoDefId()
        {
            List<string> newIdList = new List<string>();
            for (int i = 0; i < _universeMapWorldNodeList.Count; i++)
            {
                newIdList.Add(_universeMapWorldNodeList[i].GetThisNodeDtoDef().DefId);
            }
            
            return newIdList;
        }

        /// <summary>
        /// 获取当前锁定和非锁定状态的所有世界ID
        /// </summary>
        /// <param name="isLocking">True获取锁定状态,false获取非锁定状态</param>
        /// <returns></returns>
        public List<string> GetCurIsLockingWorldDtoDefID(bool isLocking)
        {
            List<string> newIdList = new List<string>();

            foreach (var worldNode in _universeMapWorldNodeList)
            {
                if (worldNode.GetThisWorldIsLocking())
                {
                    if (isLocking)
                    {
                        newIdList.Add(worldNode.GetThisNodeDtoDef().DefId);
                    }
                }
                else
                {
                    if (!isLocking)
                    {
                        newIdList.Add(worldNode.GetThisNodeDtoDef().DefId);
                    }
                }
            }
            
            return newIdList;
        }
    }
}