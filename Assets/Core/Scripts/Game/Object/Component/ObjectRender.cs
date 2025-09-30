using Core.World.Object.Interface;

namespace Core.World.Object.Component
{
    public class ObjectRender : IObjectRender
    {
        public IWorldObject Owner { get; set; }
        public void Initialize(IWorldObject owner)
        {
            
        }
    }
}