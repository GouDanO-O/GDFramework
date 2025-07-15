using System;
using System.Collections.Generic;
using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game.World
{
    /// <summary>
    /// 处理房间节点区域的移动和伸缩
    /// </summary>
    public class RoomZoom
    {
        private Room _room;
        
        public void InitRoomZoom(Room room,float zoomScaleRatio,Vector2 zoomScaleArea)
        {
            this._room = room;
            this._zoomScaleRatio = zoomScaleRatio;
            this._zoomScaleArea = zoomScaleArea;
            RegistEvent();
        }
        
        private float _curZoomScale;

        /// <summary>
        /// 进行伸缩的比例
        /// </summary>
        private float _zoomScaleRatio=0.25f;
        
        /// <summary>
        /// 能够进行缩放的范围
        /// </summary>
        private Vector2 _zoomScaleArea=new Vector2(0.25f,2f);
        
        private void RegistEvent()
        {
            
        }

        /// <summary>
        /// 滚动鼠标中间滚轮
        /// </summary>
        /// <param name="scrollData"></param>
        public void HandleMouseMiddleScroll(float curValue)
        {
            _curZoomScale += curValue * _zoomScaleRatio;
            if (_curZoomScale >= _zoomScaleArea.y)
            {
                _curZoomScale = _zoomScaleArea.y;
            }
            else if(_curZoomScale<=_zoomScaleArea.x)
            {
                _curZoomScale = _zoomScaleArea.x;
            }

            _room.ChangeContentRectLoaclScale(Vector3.one * _curZoomScale);
        }
    }
}