using Core.Game.Chunk.Room.Components;
using Core.Game.Chunk.Room.Data;
using GDFrameworkCore;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Game.Chunk.Room
{
    public class RoomComponentController : MonoBehaviour,IController
    {
        private RoomData _roomData;
        
        private RoomZoom _roomZoom;

        private RoomScroll _roomScroll;

        private RoomPointerChecker _roomPointerChecker;
        
        private RoomNodes _roomNodes;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        public void InitRoomComponent()
        {
            if (_roomPointerChecker == null)
            {
                _roomPointerChecker = this.AddComponent<RoomPointerChecker>();
                _roomPointerChecker.InitRoomPointChecker(this);
            }

            if (_roomNodes == null)
            {
                _roomNodes = this.AddComponent<RoomNodes>();
                _roomNodes.InitRoomNodes(this);
            }

            _roomZoom = new RoomZoom();


        }
        
        #region CheckPoint

        /// <summary>
        /// 滚动鼠标中间滚轮来缩放比例
        /// </summary>
        /// <param name="curValue"></param>
        public void HandleMouseMiddleScroll(float curValue)
        {
            _roomZoom.HandleMouseMiddleScroll(curValue);
        }

        /// <summary>
        /// 当前焦点是否在房间区域
        /// </summary>
        /// <returns></returns>
        public bool IsInRoomArea()
        {
            return _roomPointerChecker.IsInRoomArea();
        }

        /// <summary>
        /// 当前是否按住鼠标中键
        /// </summary>
        /// <returns></returns>
        public bool IsPressMouseMiddle()
        {
            return _roomPointerChecker.IsPressMouseMiddle();
        }

        #endregion
    }
}