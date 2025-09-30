using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node.Action;
using GDFramework.Asset;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Node
{
    [Serializable]
    public class NodeDtoDef : ChunkDtoDef
    {
        [LabelText("节点贴图"),AssetIDSelector(EAssetGroupType.Sprite)]
        public string spriteId;

        [LabelText("节点的宽高比例(默认宽高为120*40)")]
        public Vector2 nodeScale = Vector2.one;
        
        [LabelText("节点触发时会发生的效果")]
        public ActionTriggerData actionTriggerData;
    }
}