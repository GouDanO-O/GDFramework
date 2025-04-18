using GDFramework.LubanKit.Cfg.worldObj;
using UnityEngine;

namespace GDFramework.World.Object
{
    public interface IObj
    { 
        int UniqueObjId { get; set; }
        
        string ObjName { get; set; }

        void InitObj(WorldObj objData);
        
        void DeInitObj();

        void UpdateObjLogic();
        
        Vector3 GetCurrentPosition();
    }
}