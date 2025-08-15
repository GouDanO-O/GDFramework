using System;
using System.Collections.Generic;
using System.IO;
using Game.Models.Resource;
using GDFramework.Utility;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class WorldDataModel : AbstractModel
    {
        private const string PersistentDataPath = "Assets/Game/Res/Configs/WorldData";
        
        private const string AreaBlockPersistentDataPath = PersistentDataPath+"/AreaBlock";
        
        [LabelText("世界固定数据")]
        public WorldDataPersistent worldDataPersistent;
        
        [LabelText("世界对局数据"),ReadOnly]
        public WorldDataTemporary worldDataTemporary;

        [LabelText("世界画布数据")]
        public WorldCanvasDataPersistent worldCanvasDataPersistent;
        
        /// <summary>
        /// 所有区块数据
        /// </summary>
        private Dictionary<string,AreaBlockData> _areaBlockDataDict = new Dictionary<string, AreaBlockData>();
        
        /// <summary>
        /// 当前区块房间数据
        /// </summary>
        private Dictionary<string,RoomData> _curRoomDataDict = new Dictionary<string, RoomData>();
        
        /// <summary>
        /// 当前房间所有节点数据
        /// </summary>
        private Dictionary<string,NodeData>  _curNodeDataDict = new Dictionary<string, NodeData>();
        
        private WorldDataUtility _worldDataUtility;

        protected override void OnInit()
        {
            _worldDataUtility = this.GetUtility<WorldDataUtility>();
        }

        public void GetWorldData()
        {
            _worldDataUtility.LoadCompleteWorldData(this);
        }

        public AreaBlockData GetCurrentAreaBlockData(string areaBlockId)
        {
            if (worldDataPersistent == null)
            {
                LogMonoUtility.AddErrorLog("世界固定数据为空");
                return null;
            }

            if (worldDataPersistent.areaBlockDatas.Count == 0)
            {
                LogMonoUtility.AddErrorLog("世界中的区块数据为空");
                return null;
            }

            if (_areaBlockDataDict.Count == 0)
            {
                LogMonoUtility.AddErrorLog("区块字典为空");
                return null;
            }
            
            if (_areaBlockDataDict.ContainsKey(areaBlockId))
            {
                return _areaBlockDataDict[areaBlockId];
            }

            LogMonoUtility.AddErrorLog("区块字典未包含该ID");
            return null;
        }
        
        /// <summary>
        /// 更换区块
        /// </summary>
        public void ChangeAreaBlock()
        {
            
        }
    }
}