using System;
using Core.Game.Chunk.Event;
using Core.Game.Chunk.Universe.Components;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World;
using Core.Game.View;
using GDFrameworkCore;
using GDFrameworkExtend.SingletonKit;
using GDFrameworkExtend.UIKit;
using Unity.VisualScripting;
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
        private UniverseSystem universeSystem;
        
        private UniverseDataModel _universeDataModel;
        
        private UniverseZoom _universeZoom;
        
        private UniversePointerChecker _universePointerChecker;
        
        private UI_UniversePanel _universePanel;
        
        public override void InitOwnedComponents()
        {
            universeSystem = this.GetSystem<UniverseSystem>();
            _universeDataModel = this.GetModel<UniverseDataModel>();

            base.InitOwnedComponents();
            universeSystem.SetInitialWorld();
        }

        protected override void OpenChunkPanel()
        {
            base.OpenChunkPanel();
            if (_universePanel == null)
            {
                _universePanel = UIKit.OpenPanel<UI_UniversePanel>();
            }
            
            if (_universePointerChecker == null)
            {
                _universePointerChecker = _universePanel.AddComponent<UniversePointerChecker>();
            }
            
            if (_universeZoom == null)
            {
                _universeZoom = _universePanel.AddComponent<UniverseZoom>();
            }
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