using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;
using Core.Game.View.Details.Interface;
using GDFrameworkCore;
using TMPro;
using UnityEngine;

namespace Core.Game.View.Details
{
    public abstract class UI_EditorDetail_Map : UI_Details,ICanGetModel,ICanGetSystem
    {
        protected Transform MapRoot;

        protected GameObject MapNodePrefab;

        protected List<IUI_EditorDetail_MapNode> MapNodeList = new List<IUI_EditorDetail_MapNode>();

        protected IUI_EditorDetail_MapNode CurFocusNode;

        protected IUI_EditorDetail_MapNode CurInitialNode;

        protected EditorDataManager EditorDataManager;

        protected TextMeshProUGUI CurInitialNodeName;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        protected override void OnInit()
        {
            MapRoot = transform.Find("MapRoot");
            CurInitialNodeName = transform.Find("CurInitialNodeName").GetComponent<TextMeshProUGUI>();
            
            MapNodePrefab = transform.Find("MapNodePrefab").gameObject;
            
            EditorDataManager = this.GetSystem<EditorDataManager>();
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
        /// 添加节点
        /// </summary>
        /// <param name="dtoDef"></param>
        /// <param name="initialWorldId"></param>
        public abstract void AddMapNode(IChunkDtoDef dtoDef, string initialWorldId);

        /// <summary>
        /// 从对应的地图数据中获取对应的ChunkDef
        /// </summary>
        /// <param name="defId"></param>
        /// <returns></returns>
        protected abstract IChunkDtoDef GetMapModelNodeId(string defId);
        
        /// <summary>
        /// 展示地图
        /// </summary>
        /// <param name="chunkDtoDef"></param>
        public virtual void ShowMap(IChunkDtoDef chunkDtoDef)
        {
            ClearMap();

            for (int i = 0; i < chunkDtoDef.OwnedChildDtoDefID.Count; i++)
            {
                AddMapNode(GetMapModelNodeId(chunkDtoDef.OwnedChildDtoDefID[i]),chunkDtoDef.PlayerInitialLocateChildDtoDefId);
            }
        }

        /// <summary>
        /// 清空地图
        /// </summary>
        private void ClearMap()
        {
            for (int i = 0; i < MapNodeList.Count; i++)
            {
                MapNodeList[i].SetDestroy();
            }
            MapNodeList.Clear();
        }

        /// <summary>
        /// 管理节点的选中
        /// </summary>
        /// <param name="mapNode"></param>
        public void ManageMapNodeSelect(IUI_EditorDetail_MapNode mapNode)
        {
            for (int i = 0; i < MapNodeList.Count; i++)
            {
                IUI_EditorDetail_MapNode curNode =  MapNodeList[i];
                if (curNode != mapNode)
                {
                    curNode.ChangeSelecting(false);
                }
            }

            CurFocusNode = mapNode;
        }
        
        /// <summary>
        /// 设置节点作为初始节点
        /// </summary>
        /// <param name="mapNode"></param>
        public void UpdateInitialNode(IUI_EditorDetail_MapNode mapNode)
        {
            for (int i = 0; i < MapNodeList.Count; i++)
            {
                IUI_EditorDetail_MapNode curNode =  MapNodeList[i];
                if (curNode != mapNode)
                {
                    curNode.ChangeInitialNode(false);
                }
            }

            CurInitialNodeName.text = mapNode.GetThisNodeDtoDef().DefName;
            CurFocusNode = mapNode;

            EditorDataManager.GetFocusedUniverse().PlayerInitialLocateChildDtoDefId =
                mapNode.GetThisNodeDtoDef().DefId;
        }
        
        /// <summary>
        /// 获取当前初始节点
        /// </summary>
        /// <returns></returns>
        public IChunkDtoDef GetCurInitialDtoDef()
        {
            return CurInitialNode.GetThisNodeDtoDef();
        }
        
        /// <summary>
        /// 获取当前焦点节点
        /// </summary>
        /// <returns></returns>
        public IChunkDtoDef GetCurFocusDtoDef()
        {
            return CurInitialNode.GetThisNodeDtoDef();
        }
        
        /// <summary>
        /// 获取当前所有的节点的DefID
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurOwnedNodeDtoDefId()
        {
            List<string> newIdList = new List<string>();
            for (int i = 0; i < MapNodeList.Count; i++)
            {
                newIdList.Add(MapNodeList[i].GetThisNodeDtoDef().DefId);
            }
            
            return newIdList;
        }

        public List<IUI_EditorDetail_MapNode> GetCurMapNodes()
        {
            return MapNodeList;
        }
        
        /// <summary>
        /// 获取当前锁定和非锁定状态的所有节点ID
        /// </summary>
        /// <param name="isLocking">True获取锁定状态,false获取非锁定状态</param>
        /// <returns></returns>
        public List<string> GetCurIsLockingNodeDtoDefID(bool isLocking)
        {
            List<string> newIdList = new List<string>();

            foreach (var mapNode in MapNodeList)
            {
                if (mapNode.GetThisNodeIsLocking())
                {
                    if (isLocking)
                    {
                        newIdList.Add(mapNode.GetThisNodeDtoDef().DefId);
                    }
                }
                else
                {
                    if (!isLocking)
                    {
                        newIdList.Add(mapNode.GetThisNodeDtoDef().DefId);
                    }
                }
            }
            
            return newIdList;
        }
    }
}