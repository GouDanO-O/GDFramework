using UnityEngine;
using System.IO;
using GDFrameworkCore;
using Core.Game.Chunk.World.Data;
namespace Core.Game.Chunk.World
{
    public class WorldManager : AbstractSystem
    {
        private WorldData _curWorldData;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit()
        {
            InitWorldData();
            InitWorldComponent();
        }

        private void InitWorldData()
        {

        }

        private void InitWorldComponent()
        {
            
        }
    }
}