using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class RoomDataPersistent : ConfigData
    {
        public string roomId;
        
        public string roomName;

        public string roomDes;
        
        public List<NodeData> NodeDatas = new List<NodeData>();
        
        /// <summary>
        /// 进行伸缩的比例
        /// </summary>
        public float zoomScaleRatio = 0.25f;
        
        /// <summary>
        /// 能够进行缩放的范围
        /// </summary>
        public Vector2 zoomScaleArea = new Vector2(0.25f,2f);
    }
}