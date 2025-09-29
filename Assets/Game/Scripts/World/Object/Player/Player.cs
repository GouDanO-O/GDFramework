using System;
using Game.World.Object;
using GDFrameworkCore;
using GDFrameworkExtend.SingletonKit;

namespace Game.World.Player
{
    /// <summary>
    /// 玩家并非是一个,而是一个团队
    /// 一个团队里面可以有多个角色
    /// 每个角色都是独立的一条生命
    /// </summary>
    public class Player
    {
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
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


        }
    }
}