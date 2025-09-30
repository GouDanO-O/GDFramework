using UnityEngine;

namespace Core.World.Object.Interface
{
    public interface IWorldObject
    {
        string Id { get; }
        Vector3 Position { get; set; }

        T GetComponent<T>() where T : class, IComponent;
        
        bool HasComponent<T>() where T : class, IComponent;
    }
}