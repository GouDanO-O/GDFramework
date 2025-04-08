using System;
using Unity.VisualScripting;

namespace GDFramework.Procedure
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