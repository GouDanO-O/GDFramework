using Game.World.Object.Interface;
using UnityEngine;

namespace Game.World.Object.Component
{
    public class MovementComponent : IMovementComponent
    {
        public IWorldObject Owner { get; set; }
        public void Initialize(IWorldObject owner)
        {
            
        }

        public float Speed { get; set; }
        public void MoveTo(Vector3 target)
        {
            
        }

        public void Stop()
        {
            
        }
    }
}