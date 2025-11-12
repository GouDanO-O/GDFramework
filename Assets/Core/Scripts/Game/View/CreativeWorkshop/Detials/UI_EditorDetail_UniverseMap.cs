using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using TMPro;
using UnityEngine;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 宇宙星图编辑器
    /// </summary>
    public class UI_EditorDetail_UniverseMap : UI_Details,ICanGetModel
    {
        private Transform _universeMapWorldRoot;
        
        private GameObject _universeMapWorldNodePrefab;
        
        private List<UI_EditorDetail_UniverseMapWorldNode> _universeMapWorldNodeList = new List<UI_EditorDetail_UniverseMapWorldNode>();

        private Dictionary<string, UI_EditorDetail_UniverseMapWorldNode> _universeMapWorldNodeDict =
            new Dictionary<string, UI_EditorDetail_UniverseMapWorldNode>();
        
        private WorldDataModel _worldDataModel;

        private UI_EditorDetail_UniverseMapWorldNode _curFocusWorldNode;

        private UI_EditorDetail_UniverseMapWorldNode _curInitialWorldNode;

        private TextMeshProUGUI _curInitialWorldName;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        protected override void OnInit()
        {
            _universeMapWorldRoot = transform.Find("UniverseMapWorldRoot");
            _universeMapWorldNodePrefab = transform.Find("UniverseMapWorldNodePrefab").gameObject;
            
            _curInitialWorldName = transform.Find("CurInitialWorldName").GetComponent<TextMeshProUGUI>();
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
        public void ShowUniverseMap(UniverseDtoDef universeDtoDef)
        {
            ClearUniverseMap();

            for (int i = 0; i < universeDtoDef.WorldIdList.Count; i++)
            {
                string worldId =  universeDtoDef.WorldIdList[i];
                WorldDtoDef worldDtoDef = _worldDataModel.GetDefById(worldId);
                AddWorldNode(worldDtoDef);
            }
        }

        /// <summary>
        /// 添加世界
        /// </summary>
        /// <param name="worldDtoDef"></param>
        public void AddWorldNode(WorldDtoDef worldDtoDef)
        {
            UI_EditorDetail_UniverseMapWorldNode worldNode = Instantiate(_universeMapWorldNodePrefab, _universeMapWorldRoot.transform)
                .GetComponent<UI_EditorDetail_UniverseMapWorldNode>().Show();

            worldNode.SetWorldDto(this,worldDtoDef);
            _universeMapWorldNodeList.Add(worldNode);
            _universeMapWorldNodeDict.Add(worldDtoDef.DefId,worldNode);
        }

        /// <summary>
        /// 清空星图
        /// </summary>
        private void ClearUniverseMap()
        {
            for (int i = 0; i < _universeMapWorldNodeList.Count; i++)
            {
                _universeMapWorldNodeList[i].SetDestroy();
            }
            _universeMapWorldNodeList.Clear();
            _universeMapWorldNodeDict.Clear();
        }

        /// <summary>
        /// 管理星图中的世界节点的点击
        /// </summary>
        /// <param name="mapWorldNode"></param>
        public void ManageWorldSelect(UI_EditorDetail_UniverseMapWorldNode mapWorldNode)
        {
            for (int i = 0; i < _universeMapWorldNodeList.Count; i++)
            {
                UI_EditorDetail_UniverseMapWorldNode curNode =  _universeMapWorldNodeList[i];
                if (curNode != mapWorldNode)
                {
                    curNode.ChangeSelecting(false);
                }
            }

            _curFocusWorldNode = mapWorldNode;
        }

        /// <summary>
        /// 设置世界作为初始世界
        /// </summary>
        /// <param name="worldNode"></param>
        public void UpdateInitialWorld(UI_EditorDetail_UniverseMapWorldNode mapWorldNode)
        {
            for (int i = 0; i < _universeMapWorldNodeList.Count; i++)
            {
                UI_EditorDetail_UniverseMapWorldNode curNode =  _universeMapWorldNodeList[i];
                if (curNode != mapWorldNode)
                {
                    curNode.ChangeInitialWorld(false);
                }
            }

            _curInitialWorldName.text = mapWorldNode.GetThisWorldDtoDef().DefName;
            _curInitialWorldNode = mapWorldNode;
        }

        /// <summary>
        /// 获取当前焦点世界
        /// </summary>
        /// <returns></returns>
        public WorldDtoDef GetCurFocusWorldDtoDef()
        {
            return _curFocusWorldNode.GetThisWorldDtoDef();
        }

        /// <summary>
        /// 获取当前初始世界
        /// </summary>
        /// <returns></returns>
        public WorldDtoDef GetCurInitialWorldDtoDef()
        {
            return _curInitialWorldNode.GetThisWorldDtoDef();
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
                newIdList.Add(_universeMapWorldNodeList[i].GetThisWorldDtoDef().DefId);
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
                        newIdList.Add(worldNode.GetThisWorldDtoDef().DefId);
                    }
                }
                else
                {
                    if (!isLocking)
                    {
                        newIdList.Add(worldNode.GetThisWorldDtoDef().DefId);
                    }
                }
            }
            
            return newIdList;
        }
    }
}