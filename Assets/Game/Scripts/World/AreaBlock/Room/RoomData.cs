using System;
using GDFrameworkExtend.Data;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class RoomData : ConfigData
    {
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