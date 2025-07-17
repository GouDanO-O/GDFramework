using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.World
{
    public class RoomPointerChecker : MonoBehaviour,ICanRegisterEvent,IPointerEnterHandler, IPointerExitHandler
    {
        private Room _room;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        public void InitRoomPointChecker(Room room)
        {
            this._room = room;
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
            _room.isInRoomArea = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _room.isInRoomArea = false;
        }

        /// <summary>
        /// 按住鼠标中键
        /// </summary>
        private void HandleMouseMiddleDown()
        {
            _room.isPressMouseMiddle = true;
        }

        /// <summary>
        /// 松开鼠标中键
        /// </summary>
        private void HandleMouseMiddleUp()
        {
            _room.isPressMouseMiddle = false;
        }
        
        /// <summary>
        /// 处理鼠标移动
        /// 只有按住鼠标中键且在当前区域时才能进行移动
        /// </summary>
        /// <param name="moveData"></param>
        private void HandleMouseMiddleMove(SInputEvent_MouseDrag moveData)
        {
            if (_room.isPressMouseMiddle && _room.isInRoomArea)
            {
                LogMonoUtility.AddLog("当鼠标在房间区域内进行移动:"+moveData.mousePos);
            }
        }

        private void HandleMouseMiddleScroll(SInputEvent_MouseMiddleScroll scrollData)
        {
            if(!_room.isInRoomArea)
                return;

            float curValue = scrollData.scrollValue.y;
            LogMonoUtility.AddLog("滚动:"+curValue);
            _room.HandleMouseMiddleScroll(curValue);
        }
    }
}