using System.Collections.Generic;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.FluentAPI;
using UnityEngine;
using UnityEngine.UI;

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

        protected Image BaseMapImage;

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            BaseMapImage = _contentRoot.Find("BaseMap").GetComponent<Image>();
            _contentRoot = transform.Find("ContentRoot/NodeRoot");
        }

        public override void ShowMap(WorldDtoDef parentDef)
        {
            base.ShowMap(parentDef);
            //TODO 这里根据配置里面的地图ID去查找对应的地图图片
            //BaseMapImage.sprite = parentDef.
        }

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