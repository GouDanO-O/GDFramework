﻿using MoonSharp.Interpreter.Execution;
using MoonSharp.Interpreter.Execution.VM;

namespace MoonSharp.Interpreter.Tree.Statements
{
    internal class ChunkStatement : Statement, IClosureBuilder
    {
        private Statement m_Block;
        private RuntimeScopeFrame m_StackFrame;
        private SymbolRef m_Env;
        private SymbolRef m_VarArgs;

        public ChunkStatement(ScriptLoadingContext lcontext)
            : base(lcontext)
        {
            lcontext.Scope.PushFunction(this, true);
            m_Env = lcontext.Scope.DefineLocal(WellKnownSymbols.ENV);
            m_VarArgs = lcontext.Scope.DefineLocal(WellKnownSymbols.VARARGS);

            m_Block = new CompositeStatement(lcontext);

            if (lcontext.Lexer.Current.Type != TokenType.Eof)
                throw new SyntaxErrorException(lcontext.Lexer.Current, "<eof> expected near '{0}'",
                    lcontext.Lexer.Current.Text);

            m_StackFrame = lcontext.Scope.PopFunction();
        }


        public override void Compile(ByteCode bc)
        {
            var meta = bc.Emit_Meta("<chunk-root>", OpCodeMetadataType.ChunkEntrypoint);
            var metaip = bc.GetJumpPointForLastInstruction();

            bc.Emit_BeginFn(m_StackFrame);
            bc.Emit_Args(m_VarArgs);

            bc.Emit_Load(SymbolRef.Upvalue(WellKnownSymbols.ENV, 0));
            bc.Emit_Store(m_Env, 0, 0);
            bc.Emit_Pop();

            m_Block.Compile(bc);
            bc.Emit_Ret(0);

            meta.NumVal = bc.GetJumpPointForLastInstruction() - metaip;
        }

        public SymbolRef CreateUpvalue(BuildTimeScope scope, SymbolRef symbol)
        {
            return null;
        }
    }
}