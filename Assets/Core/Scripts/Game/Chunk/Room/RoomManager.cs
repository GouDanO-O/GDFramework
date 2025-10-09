using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Room.Data;
using GDFrameworkCore;

namespace Core.Game.Chunk.Room
{
    /// <summary>
    /// 有且仅能同时显示一个房间
    /// </summary>
    public class RoomManager : AbstractSystem
    {
        private RoomData _roomData;

        private RoomComponentController _roomComponentController;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit()
        {
            
        }

        public void InitRoom(RegionData curRegionData)
        {
            RegistEvent();
            InitRoomComponent();
            InitRoomData(curRegionData);
            LoadCurRoomNodeData();
        }
        
        private void RegistEvent()
        {

        }

        private void InitRoomComponent()
        {
            if (_roomComponentController == null)
            {
                
            }
        }

        private void InitRoomData(RegionData curRegionData)
        {

        }

        private void InitNodeData()
        {
            
        }
        
        #region RoomManage

        /// <summary>
        /// 更换房间
        /// </summary>
        public void ChangeRoom()
        {

        }

        /// <summary>
        /// 读取当前房间的数据
        /// </summary>
        public void LoadCurRoomNodeData()
        {
            SaveCurRoomNodeData();
            ClearCurRoomNodeData();
        }

        /// <summary>
        /// 存储当前房间的节点数据
        /// </summary>
        public void SaveCurRoomNodeData()
        {
            
        }

        /// <summary>
        /// 清除当前房间节点数据
        /// </summary>
        public void ClearCurRoomNodeData()
        {

        }

    #endregion
        

    }
}