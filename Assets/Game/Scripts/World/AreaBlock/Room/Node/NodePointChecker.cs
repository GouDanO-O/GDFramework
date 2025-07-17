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
            if (_node.CanInteract())
            {
                _node.OnClickNodeEvent.Invoke();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_node.CanMoveable())
            {
                _node.OnDragNodeEvent.Invoke(eventData.position);
            }
        }
    }
}