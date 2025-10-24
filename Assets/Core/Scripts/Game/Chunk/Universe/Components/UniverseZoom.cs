using Core.Game.Chunk.Components;
using Core.Game.View;
using GDFrameworkExtend.UIKit;
using UnityEngine;

namespace Core.Game.Chunk.Universe.Components
{
    public class UniverseZoom : ChunkZoom
    {
        protected override void SetContentRect()
        {
            ContentRectTransform = UIKit.GetPanel<UI_UniversePanel>().GetComponent<RectTransform>();
        }
    }
}