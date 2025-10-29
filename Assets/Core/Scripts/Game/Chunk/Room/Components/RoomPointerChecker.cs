using Core.Game.Chunk.Components;
using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Game.Chunk.Room.Components
{
    public class RoomPointerChecker : ChunkPointerChecker
    {
        private RoomSystem roomSystem;
        
        private RoomComponentController _roomComponentController;

        public override void InitPointChecker(ComponentController componentController)
        {
            this.roomSystem = this.GetSystem<RoomSystem>();
            this._roomComponentController = componentController as RoomComponentController;
            base.InitPointChecker(componentController);
        }
    }
}