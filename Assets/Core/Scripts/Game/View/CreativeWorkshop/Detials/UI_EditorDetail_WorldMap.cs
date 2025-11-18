using System.Collections.Generic;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 世界地图编辑器
    /// 父级: WorldDtoDef
    /// 子级: RegionDtoDef
    /// 节点: UI_EditorDetail_WorldMapNode
    /// </summary>
    public class UI_EditorDetail_WorldMap 
        : UI_EditorDetail_Map<WorldDtoDef, RegionDtoDef, UI_EditorDetail_WorldMapNode>
    {
        private RegionDataModel _regionDataModel;

        protected override void InitializeModels()
        {
            _regionDataModel = this.GetModel<RegionDataModel>();
        }

        protected override List<string> GetChildIds(WorldDtoDef parentDef)
        {
            return parentDef.RegionIdList;
        }

        protected override string GetInitialChildId(WorldDtoDef parentDef)
        {
            return parentDef.InitialPlayerLocateRegionId;
        }

        protected override void SetInitialChildId(WorldDtoDef parentDef, string childId)
        {
            parentDef.InitialPlayerLocateRegionId = childId;
        }

        protected override RegionDtoDef GetChildDef(string defId)
        {
            return _regionDataModel.GetDefById(defId);
        }

        protected override void StartTrackingNode(RegionDtoDef childDef)
        {
            _editorDataManager.StartTrackingRegion(childDef);
        }
    }
}