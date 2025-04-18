using GDFramework.LubanKit.Cfg.worldObj;

namespace GDFramework.World.Object
{
    /// <summary>
    /// 生物
    /// 生物可以主动移动,拥有完整的生命系统
    /// 即拥有一定的智能
    /// </summary>
    public class Biology : BaseObj
    {
        private ObjLive _objLive;
        
        private ObjMovement _objMovement;
        
        private ObjAI _objAI;
        
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