using System.Collections.Generic;
using Core.Game;
using Core.Game.Goods;
using Core.World.Object.Component;
using GDFrameworkCore;

namespace Core.World.Object.PlayerTeam
{
    /// <summary>
    /// 队伍共享库存
    /// 包含队伍中所有队员的库存的引用
    /// </summary>
    public class PlayerTeamInventory : AbstractModel
    {
        private Dictionary<string,InventoryComponent> teamInventory = new Dictionary<string, InventoryComponent>();
        
        protected override void OnInit()
        {
            
        }
        
        public bool DoesInventoryHasThisGoods(Goods goods)
        {
            return DoesInventoryHasThisGoods(goods,1);
        }
        
        public bool DoesInventoryHasThisGoods(Goods goods,int count)
        {
            return true;
        }
    }
}