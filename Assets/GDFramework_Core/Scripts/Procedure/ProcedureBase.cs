using System;
using Unity.VisualScripting;

namespace GDFramework_Core.Procedure
{
    public abstract class ProcedureBase : IProcedure
    {
        /// <summary>
        /// 流程初始化
        /// </summary>
        public abstract void OnInit();

        /// <summary>
        /// 进入的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool OnEnterCondition()
        {
            return true;
        }

        /// <summary>
        /// 进入流程
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// 流程轮询
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// 离开的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool OnExitCondition()
        {
            return true;
        }

        /// <summary>
        /// 离开流程
        /// </summary>
        public abstract void OnExit();

        /// <summary>
        /// 流程销毁
        /// </summary>
        public abstract void OnDeinit();
    }
}