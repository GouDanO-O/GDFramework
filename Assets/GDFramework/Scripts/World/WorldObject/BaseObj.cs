using GDFramework.LubanKit.Cfg.worldObj;
using UnityEngine;

namespace GDFramework.World.Object
{
    public abstract class BaseObj : MonoBehaviour,IObj
    {
        public int UniqueObjId { get; set; }

        public string ObjName { get; set; }

        public abstract void InitObj(WorldObj  objData);

        public abstract void DeInitObj();

        public abstract void UpdateObjLogic();

        public Vector3 GetCurrentPosition()
        {
            return transform.position;
        }
    }
}