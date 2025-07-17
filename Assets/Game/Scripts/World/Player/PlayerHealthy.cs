using GDFrameworkCore;

namespace Game.World.Player
{
    public class PlayerHealthy : AbstractSystem
    {
        private PlayerHealthyModel _playerHealthyModel;
        
        protected override void OnInit()
        {
            InitPlayerHealthy();
        }

        private void InitPlayerHealthy()
        {
            _playerHealthyModel = this.GetModel<PlayerHealthyModel>();

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