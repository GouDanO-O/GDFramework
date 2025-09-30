using Core.World.Object.Interface;
using UnityEngine;

namespace Core.World.Object.Component
{
    /// <summary>
    /// 物体渲染器
    /// </summary>
    public class ObjectRenderComponent : IObjectRender
    {
        public IWorldObject Owner { get; set; }
        public void Initialize(IWorldObject owner)
        {
            
        }
    }
}