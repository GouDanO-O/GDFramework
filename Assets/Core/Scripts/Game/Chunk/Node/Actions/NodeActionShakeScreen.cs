using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Node.Action
{
    public class NodeActionShakeScreen : NodeAction
    {
        [LabelText("震动强度"), Range(1, 10)]
        public int strength = 5;
        
        public override void Execute()
        {
            
        }
    }
}