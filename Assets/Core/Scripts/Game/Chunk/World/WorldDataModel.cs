using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Game.Chunk.Interface;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Core.Game.Chunk.World
{
    public class WorldDataModel : AbstractModel
    {
        [LabelText("当前持有的世界数据")]
        private WorldData curHoldingWorldData;
        
        [LabelText("所有的世界数据")]
        private Dictionary<string,WorldData> worldDataDict = new Dictionary<string,WorldData>();
        
        [LabelText("所有的数据")]
        private Dictionary<string,IChunkData> allDataDict = new Dictionary<string,IChunkData>();

        private WorldDataUtility _worldDataUtility;

        protected override void OnInit()
        {
            _worldDataUtility = this.GetUtility<WorldDataUtility>();
        }

        public void GetWorldData()
        {

        }

        public void SaveWorldData()
        {

        }

        public void SaveConfigData()
        {
            
        }
    }
}