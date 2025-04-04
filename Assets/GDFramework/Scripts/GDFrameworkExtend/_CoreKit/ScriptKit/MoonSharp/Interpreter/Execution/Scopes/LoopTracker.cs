﻿using MoonSharp.Interpreter.DataStructs;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Execution
{
    internal interface ILoop
    {
        void CompileBreak(ByteCode bc);
        bool IsBoundary();
    }


    internal class LoopTracker
    {
        public FastStack<ILoop> Loops = new(16384);
    }
}