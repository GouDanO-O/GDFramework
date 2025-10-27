using Core.Game.Chunk.Substance.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance
{
    /// <summary>
    /// 物体
    /// </summary>
    public class Substance : MonoBehaviour
    {
        [LabelText("物体数据")]
        public SubstanceData SubstanceData;
    }
}