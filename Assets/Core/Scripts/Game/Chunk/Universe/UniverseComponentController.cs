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
    public class UniverseComponentController : MonoSingleton<UniverseComponentController>,IController
    {
        private UniverseManager _universeManager;
        
        private UniverseDataModel _universeDataModel;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            InitUniverseComponent();
        }

        public void InitUniverseComponent()
        {
            this._universeManager = this.GetSystem<UniverseManager>();
            this._universeDataModel = this.GetModel<UniverseDataModel>();
            this.RegisterEvents();
            
            this._universeManager.SetInitialWorld();
        }
        
        private void RegisterEvents()
        {
            this.RegisterEvent<SOnChangeWorldEvent>((data) =>
            {
                
            });
        }
        
        private void TryChangeWorld(SOnChangeWorldEvent eventData)
        {
            
        }
    }
}