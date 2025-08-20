using System.IO;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;

namespace Game.World
{
    public class WorldDataUtility  : IUtility
    {
        private WorldDataModel _worldDataModel;
        
        #region 解析世界数据
        
        public void LoadCompleteWorldData(WorldDataModel worldDataModel)
        {
            this._worldDataModel = worldDataModel;
            
        }

        #region 从路径中进行加载

        private void LoadWorldDataFromFilePath()
        {

        }

        #endregion

        #region 从AB包中进行加载

        private void LoadWorldDataFromAb()
        {

        }

        #endregion
        
        
        #endregion

        #region 保存世界数据

        /// <summary>
        /// 保存完整的世界数据
        /// </summary>
        public void SaveCompleteData()
        {
            if (this._worldDataModel != null)
            {
                SaveCompleteWorldData(this._worldDataModel);
            }
            else
            {
                LogMonoUtility.AddErrorLog("世界数据为空");
            }
        }
        
        public void SaveCompleteWorldData(WorldDataModel worldDataModel)
        {
            if (worldDataModel != null)
            {
                SaveWorldDataPersistent(worldDataModel);
            }
            else
            {
                LogMonoUtility.AddErrorLog("世界数据为空");
            }
        }

        /// <summary>
        /// 保存世界数据
        /// </summary>
        private void SaveWorldDataPersistent(WorldDataModel worldDataModel)
        {
            worldDataModel.SaveConfigData();
        }
        
        /// <summary>
        /// 保存所有区块数据
        /// </summary>
        private void SaveAllAreaBlockData()
        {
            
        }

        /// <summary>
        /// 保存所有房间数据
        /// </summary>
        private void SaveAllRoomData()
        {
            
        }

        /// <summary>
        /// 保存所有节点数据
        /// </summary>
        private void SaveAllNodeData()
        {
            
        }
        
        /// <summary>
        /// 保存当前区块数据
        /// </summary>
        /// <param name="areaBlockId"></param>
        public void SaveCurAreaBlockData(string areaBlockId)
        {
            
        }

        /// <summary>
        /// 保存当前房间数据
        /// </summary>
        /// <param name="roomId"></param>
        public void SaveCurRoomData(string roomId)
        {
            
        }

        /// <summary>
        /// 保存当前节点数据
        /// </summary>
        /// <param name="nodeId"></param>
        public void SaveCurNodeData(string nodeId)
        {
            
        }

        #endregion
    }
}