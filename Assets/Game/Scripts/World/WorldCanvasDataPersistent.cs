using System;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class WorldCanvasDataPersistent : ConfigData
    {
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