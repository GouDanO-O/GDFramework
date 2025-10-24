using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk
{
    public abstract class ComponentController : MonoBehaviour,IController
    {
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public virtual void InitOwnedComponents()
        {
            RegisterEvents();
            CheckOwnedComponents();
            OpenChunkPanel();
        }

        protected virtual void CheckOwnedComponents()
        {
            
        }

        protected virtual void RegisterEvents()
        {
            
        }

        protected virtual void OpenChunkPanel()
        {
            
        }

        protected virtual void CloseChunkPanel()
        {
            
        }
    }
}