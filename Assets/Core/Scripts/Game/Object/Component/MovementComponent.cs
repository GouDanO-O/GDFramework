using Core.World.Object.Interface;
using UnityEngine;

namespace Core.World.Object.Component
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