using Game.World.Object.Interface;

namespace Game.World.Object.Component
{
    public class ObjectRender : IObjectRender
    {
        public IWorldObject Owner { get; set; }
        public void Initialize(IWorldObject owner)
        {
            
        }
    }
}