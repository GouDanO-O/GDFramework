using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GDFrameworkExtend.Data
{
    [Serializable]
    public abstract class ConfigData : PersistentData
    {
        public virtual void SaveConfigData()
        {

        }
        
    }
}