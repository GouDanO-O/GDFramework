using Core.World.Object.Interface;

namespace Core.World.Object.Component
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