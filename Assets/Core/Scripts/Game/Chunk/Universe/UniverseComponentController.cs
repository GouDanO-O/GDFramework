using Core.Game.Chunk.Event;
using Core.Game.Chunk.Universe.Data;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Universe
{
    public class UniverseComponentController : MonoBehaviour,IController
    {
        private UniverseManager _universeManager;
        
        private UniverseDataModel _universeDataModel;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void InitUniverseComponent()
        {
            this._universeManager = this.GetSystem<UniverseManager>();
            this._universeDataModel = this.GetModel<UniverseDataModel>();

            this.RegisterEvents();
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