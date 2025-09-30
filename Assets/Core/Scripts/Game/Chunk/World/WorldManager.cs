using UnityEngine;
using System.IO;
using GDFrameworkCore;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.World
{
    public class WorldManager : MonoSingleton<WorldManager>, IController
    {
        public WorldDataModel _currentWorldDataModel;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            InitWorldData();
            InitWorldComponent();
            InitAreaBlockData();
        }

        private void InitWorldData()
        {
            _currentWorldDataModel = this.GetModel<WorldDataModel>();
            _currentWorldDataModel.GetWorldData();
        }

        private void InitAreaBlockData()
        {
            Region.RegionManager.Instance.InitAreaBlock();
        }

        private void InitWorldComponent()
        {
            
        }
    }
}