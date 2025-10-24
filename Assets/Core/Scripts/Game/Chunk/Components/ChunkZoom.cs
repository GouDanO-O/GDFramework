using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Components
{
    public abstract class ChunkZoom : Component,ICanGetSystem
    {
        protected float CurZoomScale;

        /// <summary>
        /// 进行伸缩的比例
        /// </summary>
        protected float ZoomScaleRatio=0.25f;
        
        /// <summary>
        /// 能够进行缩放的范围
        /// </summary>
        protected Vector2 ZoomScaleArea=new Vector2(0.25f,2f);
        
        protected RectTransform ContentRectTransform;
        
        public virtual void InitZoom(ComponentController componentController,float zoomScaleRatio,Vector2 zoomScaleArea)
        {
            ZoomScaleRatio = zoomScaleRatio;
            ZoomScaleArea = zoomScaleArea;

            SetContentRect();
            RegistEvent();
        }
        
        protected abstract void SetContentRect();
        
        
        protected virtual void RegistEvent()
        {
            
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        /// <summary>
        /// 滚动鼠标中间滚轮
        /// </summary>
        /// <param name="scrollData"></param>
        public void HandleMouseMiddleScroll(float scrollData)
        {
            CurZoomScale += scrollData * ZoomScaleRatio;
            if (CurZoomScale >= ZoomScaleArea.y)
            {
                CurZoomScale = ZoomScaleArea.y;
            }
            else if(CurZoomScale<=ZoomScaleArea.x)
            {
                CurZoomScale = ZoomScaleArea.x;
            }

            ChangeContentRectLocalScale(Vector3.one * CurZoomScale);
        }

        /// <summary>
        /// 更改房间区域的伸缩比例
        /// </summary>
        /// <param name="willChangeValue"></param>
        public void ChangeContentRectLocalScale(Vector3 willChangeValue)
        {
            ContentRectTransform.localScale = willChangeValue;
        }
        
        /// <summary>
        /// 获取房间内容的伸缩比例
        /// </summary>
        /// <returns></returns>
        public float GetContentLocalScale()
        {
            return ContentRectTransform.localScale.x;
        }
    }
}