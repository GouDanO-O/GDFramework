using GDFrameworkCore;

namespace Game.World.Player
{
    public class PlayerInventory : AbstractSystem
    {
        private PlayerInventoryModel _playerInventoryModel;
        
        protected override void OnInit()
        {
            InitPlayerInventory();
        }

        private void InitPlayerInventory()
        {
            _playerInventoryModel = this.GetModel<PlayerInventoryModel>();
        }
    }
}