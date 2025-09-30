using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Game.Chunk.Room
{
    public class RoomPointerChecker : MonoBehaviour,ICanRegisterEvent,IPointerEnterHandler, IPointerExitHandler
    {
        private RoomManager roomManager;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        public void InitRoomPointChecker(RoomManager roomManager)
        {
            this.roomManager = roomManager;
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
            roomManager.isInRoomArea = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            roomManager.isInRoomArea = false;
        }

        /// <summary>
        /// 按住鼠标中键
        /// </summary>
        private void HandleMouseMiddleDown()
        {
            roomManager.isPressMouseMiddle = true;
        }

        /// <summary>
        /// 松开鼠标中键
        /// </summary>
        private void HandleMouseMiddleUp()
        {
            roomManager.isPressMouseMiddle = false;
        }
        
        /// <summary>
        /// 处理鼠标移动
        /// 只有按住鼠标中键且在当前区域时才能进行移动
        /// </summary>
        /// <param name="moveData"></param>
        private void HandleMouseMiddleMove(SInputEvent_MouseDrag moveData)
        {
            if (roomManager.isPressMouseMiddle && roomManager.isInRoomArea)
            {
                LogMonoUtility.AddLog("当鼠标在房间区域内进行移动:"+moveData.mousePos);
            }
        }

        private void HandleMouseMiddleScroll(SInputEvent_MouseMiddleScroll scrollData)
        {
            if(!roomManager.isInRoomArea)
                return;

            float curValue = scrollData.scrollValue.y;
            LogMonoUtility.AddLog("滚动:"+curValue);
            roomManager.HandleMouseMiddleScroll(curValue);
        }
    }
}