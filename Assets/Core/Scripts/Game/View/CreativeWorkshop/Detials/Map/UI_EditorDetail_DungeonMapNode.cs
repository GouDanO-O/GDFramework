using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Room.Data;
using Core.Game.Chunk.Room.Grid;
using GDFrameworkExtend.UIKit;
using UnityEngine;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_DungeonMapNode : UI_EditorDetail_MapNode<RoomDtoDef>
    {
        private UI_EditorDetail_DungeonMap _dungeonMap;
        
        protected override void OnDataSet<TMap>(TMap map)
        {
            _dungeonMap = map as UI_EditorDetail_DungeonMap;
        }
        
        protected override void OnNodeSelected()
        {
            editorDataManager.UpdateFocusRoom(GetDtoDef());
            _dungeonMap?.ManageNodeSelect(this);
        }
        
        public override void SetThisNodeAsInitial()
        {
            base.SetThisNodeAsInitial();
            _dungeonMap?.UpdateInitialNode(this);
        }
        
        protected override void OnLockStateChanged(bool isLocked)
        {
            if (isLocked)
            {
                editorDataManager.GetFocusedDungeon().RemoveInitialShowingRoom(dtoDef.DefId);
            }
            else
            {
                editorDataManager.GetFocusedDungeon().AddInitialShowingRoom(dtoDef.DefId);
            }
        }

        protected override void ShowNodeDetail()
        {
            if (editorDataManager.HasAnyChangeDidNotSave())
            {
                UIKit.OpenPanel<UI_TipsWindow>(UILevel.PopUI, new UI_TipsWindowData()
                {
                    TipsString = $"当前有未保存的数据\n{editorDataManager.GetChangeSummary()}",
                    CancelString = "取消",
                    SureString = "保存并打开详情",
                    SureAction = () =>
                    {
                        editorDataManager.UpdateAllTrackedSnapshots();
                        UIKit.OpenPanel<UI_Editor_RoomPanel>(new UI_Editor_RoomPanelData()
                        {
                            PresetConfig = new RoomGridConfig(50,50)
                        });
                    }
                });
            }
            else
            {
                UIKit.OpenPanel<UI_Editor_RoomPanel>();
            }
        }
        
        protected override void UpdateInitialNodeUI(bool isInitial)
        {
            if (isInitial)
            {
                ChangeInitialPlayerLocateNodeDes.text = "初始房间";
            }
            else
            {
                ChangeInitialPlayerLocateNodeDes.text = "设置为初始房间";
            }
        }

        protected override void LoadNodePosition()
        {
            SetNodePosition(dtoDef.InitialSpawnedPosition);
        }

        protected override void SaveNodePosition(Vector2 newPos)
        {
            dtoDef.InitialSpawnedPosition = newPos;
        }
    }
}