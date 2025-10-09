using System;
using System.IO;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.World.Data
{
    [Serializable,JsonObject]
    public struct WorldCanvasDataPersistent
    {
        /// <summary>
        /// 房间里面控制画布进行伸缩的比例
        /// </summary>
        [LabelText("房间里面控制画布进行伸缩的比例")]
        public float zoomScaleRatio;
        
        /// <summary>
        /// 能够进行缩放的范围
        /// </summary>
        [LabelText("房间里面控制画布能够进行缩放的范围"),JsonConverter(typeof(Vector2JsonConverter))]
        public Vector2 zoomScaleArea;
    }
}