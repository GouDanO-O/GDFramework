using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Game.Chunk.Room
{
    public class RoomScroll : ScrollRect
    {
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                eventData.button = PointerEventData.InputButton.Left;
                base.OnEndDrag(eventData);
            }
        }
    }
}