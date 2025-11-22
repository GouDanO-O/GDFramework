using Core.Game.Chunk.Region.Data;
using GDFrameworkExtend.UIKit;
using UnityEngine;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 世界地图中的区域节点
    /// </summary>
    public class UI_EditorDetail_WorldMapNode : UI_EditorDetail_MapNode<RegionDtoDef>
    {
        private UI_EditorDetail_WorldMap _worldMap;

        protected override void OnDataSet<TMap>(TMap map)
        {
            _worldMap = map as UI_EditorDetail_WorldMap;
        }

        protected override void OnNodeSelected()
        {
            _worldMap?.ManageNodeSelect(this);
        }

        protected override void UpdateInitialNodeUI(bool isInitial)
        {
            if (isInitial)
            {
                ChangeInitialPlayerLocateNodeDes.text = "初始区域";
            }
            else
            {
                ChangeInitialPlayerLocateNodeDes.text = "设置为初始区域";
            }
        }

        public override void SetThisNodeAsInitial()
        {
            base.SetThisNodeAsInitial();
            _worldMap?.UpdateInitialNode(this);
        }

        protected override void OnLockStateChanged(bool isLocked)
        {
            if (isLocked)
            {
                editorDataManager.GetFocusedWorld().RemoveInitialShowingRegion(dtoDef.DefId);
            }
            else
            {
                editorDataManager.GetFocusedWorld().AddInitialShowingRegion(dtoDef.DefId);
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
                        UIKit.OpenPanel<UI_Editor_RegionPanel>();
                    }
                });
            }
            else
            {
                UIKit.OpenPanel<UI_Editor_RegionPanel>();
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