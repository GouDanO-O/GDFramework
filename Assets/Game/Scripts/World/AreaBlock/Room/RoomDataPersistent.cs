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
        [LabelText("房间名称")]
        public string roomName;
        
        [LabelText("房间ID")]
        public string roomId;
        
        [LabelText("房间描述")]
        public string roomDes;
        
        [LabelText("房间里面拥有的互动节点")]
        public List<NodeData> NodeDatas = new List<NodeData>();
        
        /// <summary>
        /// 房间里面控制画布进行伸缩的比例
        /// </summary>
        [LabelText("房间里面控制画布进行伸缩的比例")]
        public float zoomScaleRatio = 0.25f;
        
        /// <summary>
        /// 能够进行缩放的范围
        /// </summary>
        [LabelText("房间里面控制画布能够进行缩放的范围")]
        public Vector2 zoomScaleArea = new Vector2(0.25f,2f);
    }
}