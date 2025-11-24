using System.Collections.Generic;
using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_DungeonMap : UI_EditorDetail_Map<RegionDtoDef, DungeonDtoDef,UI_EditorDetail_DungeonMapNode>
    {
        protected override void InitializeModels()
        {
            throw new System.NotImplementedException();
        }

        protected override List<string> GetChildIds(RegionDtoDef parentDef)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetInitialChildId(RegionDtoDef parentDef)
        {
            throw new System.NotImplementedException();
        }

        protected override void SetInitialChildId(RegionDtoDef parentDef, string childId)
        {
            throw new System.NotImplementedException();
        }

        protected override DungeonDtoDef GetChildDef(string defId)
        {
            throw new System.NotImplementedException();
        }

        protected override List<string> GetInitialShowingListDtoDef()
        {
            throw new System.NotImplementedException();
        }

        protected override void StartTrackingNode(DungeonDtoDef childDef)
        {
            throw new System.NotImplementedException();
        }
    }
}