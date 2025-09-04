using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Models.Resource;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public class WorldDataModel : AbstractModel
    {
        [LabelText("当前持有的世界数据")]
        public WorldDto WorldDto;

        private WorldDataUtility _worldDataUtility;

        protected override void OnInit()
        {
            _worldDataUtility = this.GetUtility<WorldDataUtility>();
        }

        public void GetWorldData()
        {
#if UNITY_EDITOR
            _worldDataUtility = new WorldDataUtility();
#endif
            _worldDataUtility.LoadCompleteWorldData(this);
        }

        public void SaveWorldData()
        {
#if UNITY_EDITOR
            _worldDataUtility = new WorldDataUtility();
#endif
            _worldDataUtility.SaveCompleteWorldData(this);
        }

        public void SaveConfigData()
        {
            
        }
    }
}