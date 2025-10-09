using UnityEngine;

namespace Core.Game.Chunk.Room.Components
{
    public class RoomNodes : MonoBehaviour
    {
        private RoomComponentController _roomComponentController;
        
        private Transform _contentRoot;

        public void InitRoomNodes(RoomComponentController roomComponentController)
        {
            this._roomComponentController = roomComponentController;
            _contentRoot = transform.Find("Viewport/Content");
        }
    }
}