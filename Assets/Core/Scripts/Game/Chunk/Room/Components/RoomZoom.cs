using System;
using System.Collections.Generic;
using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Room.Components
{
    /// <summary>
    /// 处理房间节点区域的移动和伸缩
    /// </summary>
    public class RoomZoom : Component,ICanGetSystem
    {
        private RoomManager _roomManager;
        
        private RoomComponentController _roomComponentController;
        
        private float _curZoomScale;

        /// <summary>
        /// 进行伸缩的比例
        /// </summary>
        private float _zoomScaleRatio=0.25f;
        
        /// <summary>
        /// 能够进行缩放的范围
        /// </summary>
        private Vector2 _zoomScaleArea=new Vector2(0.25f,2f);
        
        private RectTransform _contentRectTransform;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        public void InitRoomZoom(RoomComponentController roomComponentController,float zoomScaleRatio,Vector2 zoomScaleArea)
        {
            this._roomManager = this.GetSystem<RoomManager>();
            this._roomComponentController = roomComponentController;
            this._zoomScaleRatio = zoomScaleRatio;
            this._zoomScaleArea = zoomScaleArea;
            
            this._contentRectTransform  = this.GetComponent<RectTransform>();
            this.RegistEvent();
        }
        
        private void RegistEvent()
        {
            
        }

        /// <summary>
        /// 滚动鼠标中间滚轮
        /// </summary>
        /// <param name="scrollData"></param>
        public void HandleMouseMiddleScroll(float scrollData)
        {
            _curZoomScale += scrollData * _zoomScaleRatio;
            if (_curZoomScale >= _zoomScaleArea.y)
            {
                _curZoomScale = _zoomScaleArea.y;
            }
            else if(_curZoomScale<=_zoomScaleArea.x)
            {
                _curZoomScale = _zoomScaleArea.x;
            }

            this.ChangeContentRectLoaclScale(Vector3.one * _curZoomScale);
        }

        /// <summary>
        /// 更改房间区域的伸缩比例
        /// </summary>
        /// <param name="willChangeValue"></param>
        public void ChangeContentRectLoaclScale(Vector3 willChangeValue)
        {
            _contentRectTransform.localScale = willChangeValue;
        }

        /// <summary>
        /// 获取房间内容的伸缩比例
        /// </summary>
        /// <returns></returns>
        public float GetContentLoaclScale()
        {
            return _contentRectTransform.localScale.x;
        }
    }
}