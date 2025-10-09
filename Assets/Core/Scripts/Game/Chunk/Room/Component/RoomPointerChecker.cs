using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Game.Chunk.Room.Components
{
    public class RoomPointerChecker : Component,ICanRegisterEvent,IPointerEnterHandler, IPointerExitHandler,ICanGetSystem
    {
        private RoomManager _roomManager;
        
        private RoomComponentController _roomComponentController;
        
        /// <summary>
        /// 是否在房间的UI区域
        /// </summary>
        private bool _isInRoomArea = false;

        /// <summary>
        /// 是否按住了鼠标中键
        /// </summary>
        private bool _isPressMouseMiddle = false;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void InitRoomPointChecker(RoomComponentController roomComponentController)
        {
            this._roomManager = this.GetSystem<RoomManager>();
            this._roomComponentController = roomComponentController;
            this.RegistEvent();
        }
        
        private void RegistEvent()
        {
            this.RegisterEvent<SInputEvent_MouseMiddleDown>((data) =>
            {
                this.HandleMouseMiddleDown();
            });
            
            this.RegisterEvent<SInputEvent_MouseMiddleUp>((data) =>
            {
                this.HandleMouseMiddleUp();
            });

            this.RegisterEvent<SInputEvent_MouseDrag>((data) =>
            {
                this.HandleMouseMiddleMove(data);
            });

            this.RegisterEvent<SInputEvent_MouseMiddleScroll>((data) =>
            {
                this.HandleMouseMiddleScroll(data);
            });
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            this._isInRoomArea = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this._isInRoomArea = false;
        }

        /// <summary>
        /// 按住鼠标中键
        /// </summary>
        private void HandleMouseMiddleDown()
        {
            this._isPressMouseMiddle = true;
        }

        /// <summary>
        /// 松开鼠标中键
        /// </summary>
        private void HandleMouseMiddleUp()
        {
            this._isPressMouseMiddle = false;
        }
        
        /// <summary>
        /// 处理鼠标移动
        /// 只有按住鼠标中键且在当前区域时才能进行移动
        /// </summary>
        /// <param name="moveData"></param>
        private void HandleMouseMiddleMove(SInputEvent_MouseDrag moveData)
        {
            if (this._isPressMouseMiddle && this._isInRoomArea)
            {
                LogMonoUtility.AddLog("当鼠标在房间区域内进行移动:"+moveData.mousePos);
            }
        }

        private void HandleMouseMiddleScroll(SInputEvent_MouseMiddleScroll scrollData)
        {
            if(!this._isInRoomArea)
                return;

            float curValue = scrollData.scrollValue.y;
            LogMonoUtility.AddLog("滚动:"+curValue);
            this._roomComponentController.HandleMouseMiddleScroll(curValue);
        }
        
        public bool IsInRoomArea()
        {
            return this._isInRoomArea;
        }

        public bool IsPressMouseMiddle()
        {
            return this._isPressMouseMiddle;
        }
    }
}