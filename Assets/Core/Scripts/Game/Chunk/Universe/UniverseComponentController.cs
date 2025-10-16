using System;
using Core.Game.Chunk.Event;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World;
using GDFrameworkCore;
using GDFrameworkExtend.SingletonKit;
using UnityEngine;

namespace Core.Game.Chunk.Universe
{
    /// <summary>
    /// 宇宙管理器
    /// 用于集中管理宇宙中所有的世界
    /// 宇宙只负责管理世界
    /// </summary>
    public class UniverseComponentController : ComponentController
    {
        private UniverseManager _universeManager;
        
        private UniverseDataModel _universeDataModel;

        public override void InitOwnedComponents()
        {
            _universeManager = this.GetSystem<UniverseManager>();
            _universeDataModel = this.GetModel<UniverseDataModel>();

            base.InitOwnedComponents();
            _universeManager.SetInitialWorld();
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            this.RegisterEvent<SOnChangeWorldEvent>((data) =>
            {
                
            });
        }
        
    }
}