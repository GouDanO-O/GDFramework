using UnityEngine;
using System.IO;
using GDFrameworkCore;
using System.Collections.Generic;
using Core.Game.Chunk.Region;
using GDFramework.Utility;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.World
{
    public class WorldManager : MonoSingleton<WorldManager>, IController
    {
        private WorldDataModel _currentWorldDataModel;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            InitWorldData();
            InitWorldComponent();
        }

        private void InitWorldData()
        {
            _currentWorldDataModel = this.GetModel<WorldDataModel>();
            _currentWorldDataModel.GetWorldData();
        }

        private void InitWorldComponent()
        {
            
        }
    }
}