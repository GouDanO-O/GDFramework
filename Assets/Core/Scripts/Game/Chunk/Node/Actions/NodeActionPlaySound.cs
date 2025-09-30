using System;
using GDFramework.Asset;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Node.Action
{
    [Serializable]
    public class NodeActionPlaySound : NodeAction
    {
        [LabelText("音频剪辑ID"), AssetIDSelector(EAssetGroupType.Music)]
        public string audioClipId;
        
        public override void Execute()
        {
            
        }
    }
}