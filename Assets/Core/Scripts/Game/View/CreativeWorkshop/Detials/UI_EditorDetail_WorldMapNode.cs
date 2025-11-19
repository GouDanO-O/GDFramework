using Core.Game.Chunk.Region.Data;
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
            dtoDef.IsLockInInitialWorld = isLocked;
        }

        protected override void ShowNodeDetail()
        {
            // TODO: 实现区域详情显示
        }

        protected override void LoadNodePosition()
        {
            SetNodePosition(dtoDef.InitialSpawnPos);
        }

        protected override void SaveNodePosition(Vector2 newPos)
        {
            dtoDef.InitialSpawnPos = newPos;
        }
    }
}