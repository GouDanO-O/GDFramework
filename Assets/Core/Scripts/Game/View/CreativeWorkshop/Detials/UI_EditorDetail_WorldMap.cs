using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_WorldMap : UI_EditorDetail_Map
    {
        private WorldDataModel _worldDataModel;
        
        protected override void OnInit()
        {
            base.OnInit();
            _worldDataModel = this.GetModel<WorldDataModel>();
        }

        public override void AddMapNode(IChunkDtoDef dtoDef, string initialWorldId)
        {
            
        }

        protected override IChunkDtoDef GetMapModelNodeId(string defId)
        {
            return _worldDataModel.GetDefById(defId);
        }
    }
}