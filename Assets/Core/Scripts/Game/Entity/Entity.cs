using Core.Game.Chunk.Substance.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance
{
    /// <summary>
    /// 实体
    /// 只能在房间的瓦片上放置
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        [LabelText("实体数据")]
        public EntityData EntityData;
    }
}