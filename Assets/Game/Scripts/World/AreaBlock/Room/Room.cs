using System;
using System.Collections.Generic;
using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.World
{
    [Serializable]
    public class RoomData : PersistentData
    {
        /// <summary>
        /// 进行伸缩的比例
        /// </summary>
        public float zoomScaleRatio=0.25f;
        
        /// <summary>
        /// 能够进行缩放的范围
        /// </summary>
        public Vector2 zoomScaleArea=new Vector2(0.25f,2f); 
    }
    
    [RequireComponent(typeof(RoomScroll))]
    public class Room : MonoSingleton<Room>,ICanRegisterEvent,IPointerEnterHandler, IPointerExitHandler
    {
        public RoomData roomData;
        
        private RoomZoom _roomZoom;

        private RoomScroll _roomScroll;
        
        /// <summary>
        /// 是否在房间的UI区域
        /// </summary>
        public bool isInRoomArea =false;

        /// <summary>
        /// 是否按住了鼠标中键
        /// </summary>
        public bool isPressMouseMiddle = false;

        public RectTransform contentRectTransform;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        private void Start()
        {
            InitData();
            RegistEvent();
        }

        private void InitData()
        {
            _roomZoom = new RoomZoom();
            _roomZoom.InitRoomZoom(this,roomData.zoomScaleRatio,roomData.zoomScaleArea);
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
            isInRoomArea = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isInRoomArea = false;
        }

        /// <summary>
        /// 按住鼠标中键
        /// </summary>
        private void HandleMouseMiddleDown()
        {
            isPressMouseMiddle = true;
        }

        /// <summary>
        /// 松开鼠标中键
        /// </summary>
        private void HandleMouseMiddleUp()
        {
            isPressMouseMiddle = false;
        }
        
        /// <summary>
        /// 处理鼠标移动
        /// 只有按住鼠标中键且在当前区域时才能进行移动
        /// </summary>
        /// <param name="moveData"></param>
        private void HandleMouseMiddleMove(SInputEvent_MouseDrag moveData)
        {
            if (isPressMouseMiddle && isInRoomArea)
            {
                LogMonoUtility.AddLog("当鼠标在房间区域内进行移动:"+moveData.mousePos);
            }
        }

        private void HandleMouseMiddleScroll(SInputEvent_MouseMiddleScroll scrollData)
        {
            if(!isInRoomArea)
                return;

            float curValue = scrollData.scrollValue.y;
            LogMonoUtility.AddLog("滚动:"+curValue);
            _roomZoom.HandleMouseMiddleScroll(curValue);
        }

        /// <summary>
        /// 更改房间区域的伸缩比例
        /// </summary>
        /// <param name="willChangeValue"></param>
        public void ChangeContentRectLoaclScale(Vector3 willChangeValue)
        {
            contentRectTransform.localScale = willChangeValue;
        }

        /// <summary>
        /// 获取房间内容的伸缩比例
        /// </summary>
        /// <returns></returns>
        public float GetContentLoaclScale()
        {
            return contentRectTransform.localScale.x;
        }
    }
}