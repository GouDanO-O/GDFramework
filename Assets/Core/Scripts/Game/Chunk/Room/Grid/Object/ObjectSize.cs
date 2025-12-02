using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 物品尺寸
    /// </summary>
    [Serializable]
    public struct ObjectSize
    {
        [LabelText("宽度(X轴格数)")]
        [MinValue(1)]
        [JsonProperty]
        public int Width;

        [LabelText("深度(Z轴格数)")]
        [MinValue(1)]
        [JsonProperty]
        public int Depth;

        [LabelText("高度(Y轴,米)")]
        [MinValue(0.1f)]
        [JsonProperty]
        public float Height;

        public ObjectSize(int width, int depth, float height = 1f)
        {
            Width = width;
            Depth = depth;
            Height = height;
        }

        public static ObjectSize One => new ObjectSize(1, 1, 1f);

        /// <summary>
        /// 根据旋转获取实际占用尺寸
        /// </summary>
        public ObjectSize GetRotatedSize(ObjectRotation rotation)
        {
            switch (rotation)
            {
                case ObjectRotation.Deg90:
                case ObjectRotation.Deg270:
                    return new ObjectSize(Depth, Width, Height);
                default:
                    return this;
            }
        }

        public override string ToString()
        {
            return $"{Width}x{Depth}x{Height:F1}";
        }
    }
}