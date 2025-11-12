using Core.Game.Chunk.Substance.Classify.Biology.Interface;
using Core.Game.Chunk.Substance.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Substance.Classify.Biology.Data
{
    public class BiologyDtoDef : EntityDtoDef,IBiologyMovementDetDef
    {
        [LabelText("移动速度")]
        public float MoveSpeed { get; set; }
    }
}