using Game.World.Object.Interface;

namespace Game.World.Object.Component
{
    public class BaseMindComponent : IMindComponent
    {
        public IWorldObject Owner { get; set; }
        public void Initialize(IWorldObject owner)
        {
            
        }

        public EMindLevel Level { get; }
        
        public void MakeDecision()
        {
            
        }
    }
}