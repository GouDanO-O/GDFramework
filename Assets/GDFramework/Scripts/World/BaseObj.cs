using UnityEngine;

namespace GDFramework.World
{
    public abstract class BaseObj : MonoBehaviour,IObj
    {
        public int UniqueObjId { get; set; }

        public string ObjName { get; set; }

        public abstract void InitObj();

        public abstract void DeInitObj();

        public abstract void UpdateObjLogic();
    }
}