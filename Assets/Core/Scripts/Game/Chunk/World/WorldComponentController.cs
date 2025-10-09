using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.World
{
    public class WorldComponentController : MonoBehaviour,IController
    {
        private WorldData _curWorldData;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
    }
}