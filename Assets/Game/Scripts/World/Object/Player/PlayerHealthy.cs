using GDFrameworkCore;
using GDFrameworkExtend.StorageKit;

namespace Game.World.Player
{
    public class PlayerHealthy : AbstractSystem
    {
        private PlayerHealthyModel _playerHealthyModel;
        
        protected override void OnInit()
        {
            
        }

        public void InitPlayerHealthy()
        {
            
            _playerHealthyModel = this.GetModel<PlayerHealthyModel>();
            this.GetSystem<StorageKit>().RegisterSaveableObject(_playerHealthyModel);
            
            _playerHealthyModel.InitPlayerHealthyModel();
            
            _playerHealthyModel.IsDeath.Register((value) =>
            {
                PlayerDeath(value);
            });
        }

        private void  PlayerDeath(bool isDeath)
        {
            if (isDeath)
            {
                
            }
        }
        
        
    }
}