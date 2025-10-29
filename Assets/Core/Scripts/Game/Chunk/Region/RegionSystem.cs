using System.Collections.Generic;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Components;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region
{
    /// <summary>
    /// 每个区块里面包含有多个房间
    /// 区块都必定会有入口,但是不一定会有出口
    /// 同时,也可能一个区块具有多个入口或者多个出口
    /// </summary>
    public class RegionSystem : ChunkSystem
    {
        protected override string ComponentControllerPath { get; }
        protected override void InitChunkDataModel()
        {
            
        }

        protected override void SpawnComponentController()
        {
            
        }

        public override void SaveAllData()
        {
            
        }

    }
}