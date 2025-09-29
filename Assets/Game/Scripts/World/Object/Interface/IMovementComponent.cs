using UnityEngine;

namespace Game.World.Object.Interface
{
    public interface IMovementComponent : IComponent
    {
        float Speed { get; set; }
        
        void MoveTo(Vector3 target);
        
        void Stop();
    }
}