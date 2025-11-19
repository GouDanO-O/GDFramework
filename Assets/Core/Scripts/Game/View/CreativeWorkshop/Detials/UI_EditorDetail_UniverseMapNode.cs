using Core.Game.Chunk.World.Data;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.UIKit;
using UnityEngine;

namespace Core.Game.View.Details
{
    /// <summary>
    /// 宇宙星图中的世界节点
    /// </summary>
    public class UI_EditorDetail_UniverseMapNode : UI_EditorDetail_MapNode<WorldDtoDef>
    {
        private UI_EditorDetail_UniverseMap _universeMap;

        protected override void OnDataSet<TMap>(TMap map)
        {
            _universeMap = map as UI_EditorDetail_UniverseMap;
        }

        protected override void OnNodeSelected()
        {
            _universeMap?.ManageNodeSelect(this);
        }

        protected override void UpdateInitialNodeUI(bool isInitial)
        {
            if (isInitial)
            {
                ChangeInitialPlayerLocateNodeDes.text = "初始世界";
            }
            else
            {
                ChangeInitialPlayerLocateNodeDes.text = "设置为初始世界";
            }
        }

        public override void SetThisNodeAsInitial()
        {
            base.SetThisNodeAsInitial();
            _universeMap?.UpdateInitialNode(this);
        }

        protected override void OnLockStateChanged(bool isLocked)
        {
            dtoDef.IsLockInInitialUniverse = isLocked;
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
                        UIKit.OpenPanel<UI_Editor_WorldPanel>();
                    }
                });
            }
            else
            {
                UIKit.OpenPanel<UI_Editor_WorldPanel>();
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