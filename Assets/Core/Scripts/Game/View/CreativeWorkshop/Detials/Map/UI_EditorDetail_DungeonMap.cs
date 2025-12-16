using System.Collections.Generic;
using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Room.Data;
using GDFrameworkCore;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_DungeonMap : UI_EditorDetail_Map<DungeonDtoDef, RoomDtoDef,UI_EditorDetail_DungeonMapNode>
    {
        private RoomDataModel _roomDataModel;
        
        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            _contentRoot = transform.Find("ContentRoot/NodeRoot");
        }
        
        protected override void InitializeModels()
        {
            _roomDataModel = this.GetModel<RoomDataModel>();
        }
        
        protected override List<string> GetChildIds(DungeonDtoDef parentDef)
        {
            return parentDef.RoomIdList;
        }

        protected override string GetInitialChildId(DungeonDtoDef parentDef)
        {
            return parentDef.InitialPlayerLocateRoomId;
        }

        protected override void SetInitialChildId(DungeonDtoDef parentDef, string childId)
        {
            parentDef.InitialPlayerLocateRoomId = childId;
        }

        protected override RoomDtoDef GetChildDef(string defId)
        {
            return _roomDataModel.GetDefById(defId);
        }

        protected override List<string> GetInitialShowingListDtoDef()
        {
            return _currentParentDef.InitialShowingRoomIdList;
        }

        protected override void StartTrackingNode(RoomDtoDef childDef)
        {
            _editorDataManager.StartTrackingRoom(childDef);
        }
    }
}