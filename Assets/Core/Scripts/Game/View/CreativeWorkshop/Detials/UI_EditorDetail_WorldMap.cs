using System.Collections.Generic;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using TMPro;
using UnityEngine;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_WorldMap : UI_Details,ICanGetSystem,ICanGetModel
    {
        private Transform _contentRoot;

        private GameObject _mapNodePrefab;

        private List<UI_EditorDetail_WorldMapNode> _mapNodeList = new List<UI_EditorDetail_WorldMapNode>();

        private WorldDataModel _worldDataModel;

        private RegionDataModel _regionDataModel;

        private UI_EditorDetail_WorldMapNode _curFocusNode;

        private UI_EditorDetail_WorldMapNode _curInitialNode;

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
            _regionDataModel = this.GetModel<RegionDataModel>();
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
        /// 展示世界地图
        /// </summary>
        public void ShowMap(WorldDtoDef curDtoDef)
        {
            ClearMap();

            for (int i = 0; i < curDtoDef.RegionIdList.Count; i++)
            {
                string initialId = curDtoDef.InitialPlayerLocateRegionId;
                string defId = curDtoDef.RegionIdList[i];
                RegionDtoDef regionDtoDef = _regionDataModel.GetDefById(defId);
                AddNode(regionDtoDef, initialId);
                _editorDataManager.StartTrackingRegion(regionDtoDef);
            }
        }

        /// <summary>
        /// 添加世界
        /// </summary>
        /// <param name="regionDtoDef"></param>
        public void AddNode(RegionDtoDef regionDtoDef, string initialId)
        {
            UI_EditorDetail_WorldMapNode node = Instantiate(_mapNodePrefab, _contentRoot.transform)
                .GetComponent<UI_EditorDetail_WorldMapNode>().Show();

            node.SetDto(this, regionDtoDef);
            if (regionDtoDef.DefId.Equals(initialId))
            {
                node.SetThisAsInitial();
            }

            _mapNodeList.Add(node);
        }

        public List<UI_EditorDetail_WorldMapNode> GetCurWorldRegionNodes()
        {
            return _mapNodeList;
        }

        /// <summary>
        /// 清空星图
        /// </summary>
        private void ClearMap()
        {
            for (int i = 0; i < _mapNodeList.Count; i++)
            {
                _mapNodeList[i].SetDestroy();
            }

            _mapNodeList.Clear();
        }

        /// <summary>
        /// 管理节点的点击
        /// </summary>
        /// <param name="mapNode"></param>
        public void ManageNodeSelect(UI_EditorDetail_WorldMapNode mapNode)
        {
            for (int i = 0; i < _mapNodeList.Count; i++)
            {
                UI_EditorDetail_WorldMapNode curNode = _mapNodeList[i];
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
        public void UpdateInitialNode(UI_EditorDetail_WorldMapNode mapNode)
        {
            for (int i = 0; i < _mapNodeList.Count; i++)
            {
                UI_EditorDetail_WorldMapNode curNode = _mapNodeList[i];
                if (curNode != mapNode)
                {
                    curNode.ChangeInitial(false);
                }
            }

            _initialName.text = mapNode.GetThisDtoDef().DefName;
            _curInitialNode = mapNode;

            _editorDataManager.GetFocusedUniverse().InitialPlayerLocateWorldId =
                mapNode.GetThisDtoDef().DefId;
        }

        /// <summary>
        /// 获取当前焦点
        /// </summary>
        /// <returns></returns>
        public RegionDtoDef GetCurFocusDtoDef()
        {
            return _curFocusNode.GetThisDtoDef();
        }

        /// <summary>
        /// 获取当前初始节点
        /// </summary>
        /// <returns></returns>
        public RegionDtoDef GetCurInitialDtoDef()
        {
            return _curInitialNode.GetThisDtoDef();
        }

        /// <summary>
        /// 获取当前所有的ID
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurOwnedDtoDefId()
        {
            List<string> newIdList = new List<string>();
            for (int i = 0; i < _mapNodeList.Count; i++)
            {
                newIdList.Add(_mapNodeList[i].GetThisDtoDef().DefId);
            }

            return newIdList;
        }

        /// <summary>
        /// 获取当前锁定和非锁定状态的所有ID
        /// </summary>
        /// <param name="isLocking">True获取锁定状态,false获取非锁定状态</param>
        /// <returns></returns>
        public List<string> GetCurIsLockingDtoDefID(bool isLocking)
        {
            List<string> newIdList = new List<string>();

            foreach (var mapNode in _mapNodeList)
            {
                if (mapNode.GetThisIsLocking())
                {
                    if (isLocking)
                    {
                        newIdList.Add(mapNode.GetThisDtoDef().DefId);
                    }
                }
                else
                {
                    if (!isLocking)
                    {
                        newIdList.Add(mapNode.GetThisDtoDef().DefId);
                    }
                }
            }

            return newIdList;
        }
    }
}