using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.UIKit;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 宇宙星图编辑器
    /// </summary>
    public class UI_EditorDetail_UniverseMap : UI_EditorDetail_Map
    {
        private WorldDataModel _worldDataModel;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        protected override void OnInit()
        {
            base.OnInit();
            
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

        public override void AddMapNode(IChunkDtoDef dtoDef, string initialWorldId)
        {
            UI_EditorDetail_UniverseMapWorldNode worldNode = Instantiate(MapNodePrefab, MapRoot.transform)
                .GetComponent<UI_EditorDetail_UniverseMapWorldNode>().Show();
            
            worldNode.SetMapNodeDto(this,dtoDef);
            if (dtoDef.DefId.Equals(initialWorldId))
            {
                //worldNode.SetThisWorldAsInitialWorld();
            }
            MapNodeList.Add(worldNode);
        }

        protected override IChunkDtoDef GetMapModelNodeId(string defId)
        {
            return _worldDataModel.GetDefById(defId);
        }
        
    }
}