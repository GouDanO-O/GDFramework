using System;
using GDFramework_General.Procedure;
using QFramework;

namespace GDFramework_Core.Procedure
{
    public struct SChangeProcedureEvent
    {
        public EProcedureType _procedureType;
        
        public SChangeProcedureEvent(EProcedureType procedureType)
        {
            _procedureType = procedureType;
        }
    }
}