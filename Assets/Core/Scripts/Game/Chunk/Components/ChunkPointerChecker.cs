using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Game.Chunk.Components
{
    public abstract class ChunkPointerChecker : Component,ICanRegisterEvent,IPointerEnterHandler, IPointerExitHandler,ICanGetSystem
    {
        /// <summary>
        /// 是否在区块的UI区域
        /// </summary>
        public bool IsInChunkArea
        {
            get;
            set;
        }

        /// <summary>
        /// 是否按住了鼠标中键
        /// </summary>
        public bool IsPressMouseMiddle
        {
            get;
            set;
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        public virtual void InitPointChecker(ComponentController componentController)
        {
            RegisterEvent();
        }
        
        protected virtual void RegisterEvent()
        {
            this.RegisterEvent<SInputEvent_MouseMiddleDown>((data) =>
            {
                HandleMouseMiddleDown();
            });
            
            this.RegisterEvent<SInputEvent_MouseMiddleUp>((data) =>
            {
                HandleMouseMiddleUp();
            });

            this.RegisterEvent<SInputEvent_MouseDrag>((data) =>
            {
                HandleMouseMiddleMove(data);
            });

            this.RegisterEvent<SInputEvent_MouseMiddleScroll>((data) =>
            {
                HandleMouseMiddleScroll(data);
            });
        }

        protected virtual void UnregisterEvent()
        {
            this.UnRegisterEvent<SInputEvent_MouseMiddleDown>((data) =>
            {
                HandleMouseMiddleDown();
            });
            
            this.UnRegisterEvent<SInputEvent_MouseMiddleUp>((data) =>
            {
                HandleMouseMiddleUp();
            });

            this.UnRegisterEvent<SInputEvent_MouseDrag>(HandleMouseMiddleMove);
            this.UnRegisterEvent<SInputEvent_MouseMiddleScroll>(HandleMouseMiddleScroll);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsInChunkArea = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsInChunkArea = false;
        }

        /// <summary>
        /// 按住鼠标中键
        /// </summary>
        protected virtual void HandleMouseMiddleDown()
        {
            IsPressMouseMiddle = true;
        }

        /// <summary>
        /// 松开鼠标中键
        /// </summary>
        protected virtual void HandleMouseMiddleUp()
        {
            IsPressMouseMiddle = false;
        }
        
        /// <summary>
        /// 处理鼠标移动
        /// 只有按住鼠标中键且在当前区域时才能进行移动
        /// </summary>
        /// <param name="moveData"></param>
        protected virtual void HandleMouseMiddleMove(SInputEvent_MouseDrag moveData)
        {
            if (IsPressMouseMiddle && IsInChunkArea)
            {
                LogKit.Log("当鼠标在房间区域内进行移动:"+moveData.mousePos);
            }
        }

        /// <summary>
        /// 处理鼠标的滚动
        /// </summary>
        /// <param name="scrollData"></param>
        protected virtual void HandleMouseMiddleScroll(SInputEvent_MouseMiddleScroll scrollData)
        {
            if(!IsInChunkArea)
                return;

            float curValue = scrollData.scrollValue.y;
            LogKit.Log("滚动:"+curValue);
        }
    }
}