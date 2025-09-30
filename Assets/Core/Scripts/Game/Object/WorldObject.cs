using System;
using System.Collections.Generic;
using System.Linq;
using Core.World.Object.Interface;
using UnityEngine;

namespace Core.World.Object
{
    /// <summary>
    /// 世界中所有物体的基类
    /// </summary>
    public class WorldObject : MonoBehaviour,IWorldObject
    {
        public string Id { get; private set; }
        
        public Vector3 Position { get; set; }
    
        private Dictionary<Type, IComponent> _components = new();
        
        public WorldObject(string id)
        {
            Id = id;
        }
        
        public T GetComponent<T>() where T : class, IComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
                return component as T;
            return null;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            return _components.ContainsKey(typeof(T));
        }
        
        public void AddComponent(IComponent component)
        {
            var type = component.GetType().GetInterfaces()
                .FirstOrDefault(i => i != typeof(IComponent) && typeof(IComponent).IsAssignableFrom(i));
        
            if (type != null)
            {
                _components[type] = component;
                component.Initialize(this);
            }
        }
    }
}