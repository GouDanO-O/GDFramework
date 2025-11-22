using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 宇宙星图编辑器
    /// 父级: UniverseDtoDef
    /// 子级: WorldDtoDef
    /// 节点: UI_EditorDetail_UniverseMapNode
    /// </summary>
    public class UI_EditorDetail_UniverseMap 
        : UI_EditorDetail_Map<UniverseDtoDef, WorldDtoDef, UI_EditorDetail_UniverseMapNode>
    {
        private WorldDataModel _worldDataModel;

        protected override void InitializeModels()
        {
            _worldDataModel = this.GetModel<WorldDataModel>();
        }

        protected override List<string> GetChildIds(UniverseDtoDef parentDef)
        {
            return parentDef.WorldIdList;
        }

        protected override string GetInitialChildId(UniverseDtoDef parentDef)
        {
            return parentDef.InitialPlayerLocateWorldId;
        }

        protected override void SetInitialChildId(UniverseDtoDef parentDef, string childId)
        {
            parentDef.InitialPlayerLocateWorldId = childId;
        }

        protected override WorldDtoDef GetChildDef(string defId)
        {
            return _worldDataModel.GetDefById(defId);
        }
        
        protected override List<string> GetInitialShowingListDtoDef()
        {
            return _currentParentDef.InitialShowingWorldIdList;
        }

        protected override void StartTrackingNode(WorldDtoDef childDef)
        {
            _editorDataManager.StartTrackingWorld(childDef);
        }
    }
}