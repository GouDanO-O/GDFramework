using GDFramework.LubanKit.Cfg.worldObj;

namespace GDFramework.World.Object.Substance
{
    /// <summary>
    /// 物体
    /// 不可主动移动,拥有不完整的生命系统
    /// 即未拥有智能
    /// (部分物体可以被攻击销毁或者随着时间流逝而消失)
    /// </summary>
    public class Substance : BaseObj
    {
        private ObjLive _objLive;
        
        public override void InitObj(WorldObj objData)
        {
            
        }

        public override void DeInitObj()
        {
            
        }

        public override void UpdateObjLogic()
        {
            
        }
    }
}