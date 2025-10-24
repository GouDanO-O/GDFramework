using System;
using System.Collections.Generic;
using Core.Game.Chunk.Components;
using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Room.Components
{
    /// <summary>
    /// 处理房间节点区域的移动和伸缩
    /// </summary>
    public class RoomZoom : ChunkZoom
    {
        private RoomManager _roomManager;
        
        private RoomComponentController _roomComponentController;

        public override void InitZoom(ComponentController componentController, float zoomScaleRatio, Vector2 zoomScaleArea)
        {
            this._roomManager = this.GetSystem<RoomManager>();
            this._roomComponentController = componentController as RoomComponentController;
            base.InitZoom(componentController, zoomScaleRatio, zoomScaleArea);
        }
        
        protected override void SetContentRect()
        {
            
        }
    }
}