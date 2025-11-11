using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using UnityEngine;

namespace Core.Game.View.Details
{
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
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        protected override void OnInit()
        {
            _universeMapWorldRoot = transform.Find("UniverseMapWorldRoot");
            _universeMapWorldNodePrefab = transform.Find("UniverseMapWorldNodePrefab").gameObject;
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

            _curInitialWorldNode = mapWorldNode;
        }

        public WorldDtoDef GetCurFocusWorldDtoDef()
        {
            return _curFocusWorldNode.GetThisWorldDtoDef();
        }

        public WorldDtoDef GetCurInitialWorldDtoDef()
        {
            return _curInitialWorldNode.GetThisWorldDtoDef();
        }
    }
}