using System.Collections.Generic;
using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using UnityEngine.UI;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_RegionMap : UI_EditorDetail_Map<RegionDtoDef,DungeonDtoDef,UI_EditorDetail_RegionMapNode>
    {
        private DungeonDataModel _dungeonDataModel;
        
        protected Image BaseMapImage;

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            BaseMapImage = _contentRoot.Find("BaseMap").GetComponent<Image>();
            _contentRoot = transform.Find("ContentRoot/NodeRoot");
        }
        
        protected override void InitializeModels()
        {
            _dungeonDataModel = this.GetModel<DungeonDataModel>();
        }

        protected override List<string> GetChildIds(RegionDtoDef parentDef)
        {
            return parentDef.DungeonIdList;
        }

        protected override string GetInitialChildId(RegionDtoDef parentDef)
        {
            return parentDef.InitialPlayerLocateDungeonId;
        }

        protected override void SetInitialChildId(RegionDtoDef parentDef, string childId)
        {
            parentDef.InitialPlayerLocateDungeonId = childId;
        }

        protected override DungeonDtoDef GetChildDef(string defId)
        {
            return _dungeonDataModel.GetDefById(defId);
        }

        protected override List<string> GetInitialShowingListDtoDef()
        {
            return _currentParentDef.InitialShowingDungeonIdList;
        }

        protected override void StartTrackingNode(DungeonDtoDef childDef)
        {
            _editorDataManager.StartTrackingDungeon(childDef);
        }
    }
}