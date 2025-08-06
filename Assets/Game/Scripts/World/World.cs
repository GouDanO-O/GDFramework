using UnityEngine;
using System.IO;
using GDFrameworkCore;
using System.Collections.Generic;
using Game.Models.Resource;
using GDFramework.Utility;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;

namespace Game.World
{
    public class World : MonoSingleton<World>, IController
    {
        [ShowInInspector]
        private WorldDataModel _currentWorldDataModel;
        
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
            _currentWorldDataModel.LoadCompleteWorldData();
        }

        private void InitAreaBlockData()
        {
            AreaBlock.Instance.InitAreaBlock();
        }

        private void InitWorldComponent()
        {
            
        }
        
        [Button("解析世界数据json")]
        public void SetWorldData(TextAsset curAsset)
        {
            this._currentWorldDataModel.LoadCompleteWorldData(curAsset);
        }
    }
}