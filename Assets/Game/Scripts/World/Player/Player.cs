using System;
using GDFrameworkCore;
using GDFrameworkExtend.SingletonKit;

namespace Game.World.Player
{
    public class Player : MonoSingleton<Player>,IController
    {
        private PlayerHealthy _playerHealthy;
        
        private PlayerInventory _playerInventory;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        private void Start()
        {
            InitPlayer();
        }

        /// <summary>
        /// 初始化玩家
        /// </summary>
        public void InitPlayer()
        {
            InitData();
        }

        private void InitData()
        {
            _playerHealthy = this.GetSystem<PlayerHealthy>();
            _playerHealthy.InitPlayerHealthy();
            _playerInventory = this.GetSystem<PlayerInventory>();
        }
    }
}