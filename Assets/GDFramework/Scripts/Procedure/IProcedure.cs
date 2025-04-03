using System;
using Unity.VisualScripting;

namespace GDFramework_Core.Procedure
{
    public interface IProcedure
    {
        void OnInit();

        bool OnEnterCondition();

        void OnEnter();

        void OnUpdate();

        bool OnExitCondition();

        void OnExit();

        void OnDeinit();
    }
}