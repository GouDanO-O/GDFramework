using System;
using System.Collections.Generic;
using GDFramework.Input;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.World
{
    /// <summary>
    /// 有且仅能同时显示一个房间
    /// </summary>
    [RequireComponent(typeof(RoomScroll))]
    public class Room : MonoSingleton<Room>, IController
    {
        public RoomData roomData;

        private RoomZoom _roomZoom;

        private RoomScroll _roomScroll;

        private RoomPointerChecker _roomPointerChecker;

        /// <summary>
        /// 是否在房间的UI区域
        /// </summary>
        public bool isInRoomArea = false;

        /// <summary>
        /// 是否按住了鼠标中键
        /// </summary>
        public bool isPressMouseMiddle = false;

        private RectTransform _contentRectTransform;

        private Transform _contentRoot;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            InitData();
            RegistEvent();
        }

        private void InitData()
        {
            if (_roomPointerChecker == null)
            {
                _roomPointerChecker = this.AddComponent<RoomPointerChecker>();
                _roomPointerChecker.InitRoomPointChecker(this);
            }

            _roomZoom = new RoomZoom();
            _contentRectTransform = this.GetComponent<RectTransform>();

            _contentRoot = transform.Find("Viewport/Content");
        }


        private void RegistEvent()
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
        
        #region CheckPoint

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

        /// <summary>
        /// 滚动鼠标中间滚轮来缩放比例
        /// </summary>
        /// <param name="curValue"></param>
        public void HandleMouseMiddleScroll(float curValue)
        {
            _roomZoom.HandleMouseMiddleScroll(curValue);
        }

        #endregion
    }
}