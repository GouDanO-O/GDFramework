using System;
using Game.Procedure;


namespace GDFramework_Core.Procedure
{
    public struct SChangeProcedureEvent
    {
        public readonly EProcedureType _procedureType;

        public SChangeProcedureEvent(EProcedureType procedureType)
        {
            _procedureType = procedureType;
        }
    }
}