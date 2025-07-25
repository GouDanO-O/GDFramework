using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.World
{
    public class NodePointChecker : MonoBehaviour,IPointerClickHandler,IDragHandler
    {
        private Node _node;

        public void InitNodePointChecker(Node node)
        {
            this._node = node;
            this.RegisterEvents();
        }

        private void RegisterEvents()
        {
            
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_node != null && _node.CanInteract())
            {
                _node.TriggerInteraction();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_node != null && _node.CanMoveable())
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _node.transform.parent as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPoint
                );
                
                _node.OnDragNodeEvent?.Invoke(localPoint);
            }
        }
    }
}