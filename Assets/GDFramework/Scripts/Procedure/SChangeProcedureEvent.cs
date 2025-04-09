using System;
using Game.Procedure;


namespace GDFramework.Procedure
{
    public struct SChangeProcedureEvent
    {
        public readonly System.Type _procedureType;

        public SChangeProcedureEvent(Type procedureType)
        {
            _procedureType = procedureType;
        }
    }
}