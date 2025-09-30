using Core.Game.Goods;
using Core.World.Object.Interface;

namespace Core.World.Object.Component
{
    public class InventoryComponent : IInventoryComponent
    {
        public IWorldObject Owner { get; set; }
        public void Initialize(IWorldObject owner)
        {
            
        }

        bool DoesInventoryHasThisGoods(Goods goods)
        {
            return true;
        }
    }
}