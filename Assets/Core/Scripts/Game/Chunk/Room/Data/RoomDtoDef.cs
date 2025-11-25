using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Substance.Data;
using Core.Game.Chunk.Substance.Interface;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable,JsonObject]
    public class RoomDtoDef : ChunkDtoDef
    {
        [LabelText("房间宽度(瓦片数)")]
        [MinValue(5)]
        public int Width = 20;
        
        [LabelText("房间高度(瓦片数)")]
        [MinValue(5)]
        public int Height = 20;
        
        [LabelText("是否包含户外区域")]
        public bool HasOutdoorArea;

        public override string GetTypePrefix()
        {
            return "ROOM";
        }

        public override bool Validate(out string error)
        {
            if (!base.Validate(out error))
                return false;

            if (Width < 5 || Height < 5)
            {
                error = "房间尺寸不能小于 5x5";
                return false;
            }

            if (Width > 100 || Height > 100)
            {
                error = "房间尺寸不能大于 100x100";
                return false;
            }

            return true;
        }

        public override void GenerateDefId()
        {
            base.GenerateDefId();
        }

        public void AddEntityToTile(Vector2Int tileIndex, IEntityDtoDef entityDtoDef)
        {
            
        }
    }
}