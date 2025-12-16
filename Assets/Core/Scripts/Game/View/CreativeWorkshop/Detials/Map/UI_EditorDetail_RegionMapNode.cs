using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;
using GDFrameworkExtend.UIKit;
using UnityEngine;

namespace Core.Game.View.Details
{
    public class UI_EditorDetail_RegionMapNode : UI_EditorDetail_MapNode<DungeonDtoDef>
    {
        private UI_EditorDetail_RegionMap _regionMap;
        
        protected override void OnDataSet<TMap>(TMap map)
        {
            _regionMap = map as UI_EditorDetail_RegionMap;
        }
        
        protected override void OnNodeSelected()
        {
            editorDataManager.UpdateFocusDungeon(GetDtoDef());
            _regionMap?.ManageNodeSelect(this);
        }
        
        public override void SetThisNodeAsInitial()
        {
            base.SetThisNodeAsInitial();
            _regionMap?.UpdateInitialNode(this);
        }
        
        protected override void UpdateInitialNodeUI(bool isInitial)
        {
            if (isInitial)
            {
                ChangeInitialPlayerLocateNodeDes.text = "初始副本";
            }
            else
            {
                ChangeInitialPlayerLocateNodeDes.text = "设置为初始副本";
            }
        }
        
        protected override void OnLockStateChanged(bool isLocked)
        {
            if (isLocked)
            {
                editorDataManager.GetFocusedRegion().RemoveInitialShowingDungeon(dtoDef.DefId);
            }
            else
            {
                editorDataManager.GetFocusedRegion().AddInitialShowingDungeon(dtoDef.DefId);
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
                        UIKit.OpenPanel<UI_Editor_DungeonPanel>();
                    }
                });
            }
            else
            {
                UIKit.OpenPanel<UI_Editor_DungeonPanel>();
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