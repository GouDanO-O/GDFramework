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
        }

        protected virtual void RegisterEvents()
        {
            
        }
    }
}