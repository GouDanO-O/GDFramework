using GDFramework.Utility;
using GDFrameworkCore;

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
        public void SaveCompleteWorldData()
        {
            if (this._worldDataModel != null)
            {
                
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
                
            }
            else
            {
                LogMonoUtility.AddErrorLog("世界数据为空");
            }
        }

        private void SaveWorldData()
        {
            
        }

        private void SaveAreaBlockData()
        {
            
        }

        private void SaveRoomData()
        {
            
        }

        private void SaveNodeData()
        {
            
        }

        public void SaveCurWorldPersistentData()
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