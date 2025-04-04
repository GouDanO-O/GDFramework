﻿#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.VsCodeDebugger;
using MoonSharp.VsCodeDebugger.SDK;

namespace MoonSharp.VsCodeDebugger.DebuggerLogic
{
    internal class AsyncDebugger : IDebugger
    {
        private static object s_AsyncDebuggerIdLock = new();
        private static int s_AsyncDebuggerIdCounter = 0;

        private object m_Lock = new();
        private IAsyncDebuggerClient m_Client__;
        private DebuggerAction m_PendingAction = null;

        private List<WatchItem>[] m_WatchItems;
        private Dictionary<int, SourceCode> m_SourcesMap = new();
        private Dictionary<int, string> m_SourcesOverride = new();
        private Func<SourceCode, string> m_SourceFinder;


        public DebugService DebugService { get; private set; }

        public Regex ErrorRegex { get; set; }

        public Script Script { get; private set; }

        public bool PauseRequested { get; set; }

        public string Name { get; set; }

        public int Id { get; private set; }


        public AsyncDebugger(Script script, Func<SourceCode, string> sourceFinder, string name)
        {
            lock (s_AsyncDebuggerIdLock)
            {
                Id = s_AsyncDebuggerIdCounter++;
            }

            m_SourceFinder = sourceFinder;
            ErrorRegex = new Regex(@"\A.*\Z");
            Script = script;
            m_WatchItems = new List<WatchItem>[(int)WatchType.MaxValue];
            Name = name;

            for (var i = 0; i < m_WatchItems.Length; i++)
                m_WatchItems[i] = new List<WatchItem>(64);
        }


        public IAsyncDebuggerClient Client
        {
            get => m_Client__;
            set
            {
                lock (m_Lock)
                {
                    if (m_Client__ != null && m_Client__ != value) m_Client__.Unbind();

                    if (value != null)
                        for (var i = 0; i < Script.SourceCodeCount; i++)
                            if (m_SourcesMap.ContainsKey(i))
                                value.OnSourceCodeChanged(i);

                    m_Client__ = value;
                }
            }
        }

        DebuggerAction IDebugger.GetAction(int ip, SourceRef sourceref)
        {
            PauseRequested = false;

            lock (m_Lock)
            {
                if (Client != null) Client.SendStopEvent();
            }

            while (true)
            {
                lock (m_Lock)
                {
                    if (Client == null) return new DebuggerAction() { Action = DebuggerAction.ActionType.Run };

                    if (m_PendingAction != null)
                    {
                        var action = m_PendingAction;
                        m_PendingAction = null;
                        return action;
                    }
                }

                Sleep(10);
            }
        }


        public void QueueAction(DebuggerAction action)
        {
            while (true)
            {
                lock (m_Lock)
                {
                    if (m_PendingAction == null)
                    {
                        m_PendingAction = action;
                        break;
                    }
                }

                Sleep(10);
            }
        }

        private void Sleep(int v)
        {
#if DOTNET_CORE
			System.Threading.Tasks.Task.Delay(10).Wait();
#else
            System.Threading.Thread.Sleep(10);
#endif
        }

        private DynamicExpression CreateDynExpr(string code)
        {
            try
            {
                return Script.CreateDynamicExpression(code);
            }
            catch (Exception ex)
            {
                return Script.CreateConstantDynamicExpression(code, DynValue.NewString(ex.Message));
            }
        }

        List<DynamicExpression> IDebugger.GetWatchItems()
        {
            return new List<DynamicExpression>();
        }

        bool IDebugger.IsPauseRequested()
        {
            return PauseRequested;
        }

        void IDebugger.RefreshBreakpoints(IEnumerable<SourceRef> refs)
        {
        }

        void IDebugger.SetByteCode(string[] byteCode)
        {
        }

        void IDebugger.SetSourceCode(SourceCode sourceCode)
        {
            m_SourcesMap[sourceCode.SourceID] = sourceCode;

            var invalidFile = false;

            var file = m_SourceFinder(sourceCode);

            if (!string.IsNullOrEmpty(file))
                try
                {
                    if (!File.Exists(file))
                        invalidFile = true;
                }
                catch
                {
                    invalidFile = true;
                }
            else
                invalidFile = true;

            if (invalidFile)
            {
                file = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".lua");
                File.WriteAllText(file, sourceCode.Code + GetFooterForTempFile());
                m_SourcesOverride[sourceCode.SourceID] = file;
            }
            else if (file != sourceCode.Name)
            {
                m_SourcesOverride[sourceCode.SourceID] = file;
            }


            lock (m_Lock)
            {
                if (Client != null)
                    Client.OnSourceCodeChanged(sourceCode.SourceID);
            }
        }

        private string GetFooterForTempFile()
        {
            return "\n\n" +
                   "----------------------------------------------------------------------------------------------------------\n" +
                   "-- This file has been generated by the debugger as a placeholder for a script snippet stored in memory. --\n" +
                   "-- If you restart the host process, the contents of this file are not valid anymore.                    --\n" +
                   "----------------------------------------------------------------------------------------------------------\n";
        }

        public string GetSourceFile(int sourceId)
        {
            if (m_SourcesOverride.ContainsKey(sourceId))
                return m_SourcesOverride[sourceId];
            else if (m_SourcesMap.ContainsKey(sourceId))
                return m_SourcesMap[sourceId].Name;
            return null;
        }

        public bool IsSourceOverride(int sourceId)
        {
            return m_SourcesOverride.ContainsKey(sourceId);
        }


        void IDebugger.SignalExecutionEnded()
        {
            lock (m_Lock)
            {
                if (Client != null)
                    Client.OnExecutionEnded();
            }
        }

        bool IDebugger.SignalRuntimeException(ScriptRuntimeException ex)
        {
            lock (m_Lock)
            {
                if (Client == null)
                    return false;
            }

            Client.OnException(ex);
            PauseRequested = ErrorRegex.IsMatch(ex.Message);
            return PauseRequested;
        }

        void IDebugger.Update(WatchType watchType, IEnumerable<WatchItem> items)
        {
            var list = m_WatchItems[(int)watchType];

            list.Clear();
            list.AddRange(items);

            lock (m_Lock)
            {
                if (Client != null)
                    Client.OnWatchesUpdated(watchType);
            }
        }


        public List<WatchItem> GetWatches(WatchType watchType)
        {
            return m_WatchItems[(int)watchType];
        }

        public SourceCode GetSource(int id)
        {
            if (m_SourcesMap.ContainsKey(id))
                return m_SourcesMap[id];

            return null;
        }

        public SourceCode FindSourceByName(string path)
        {
            // we use case insensitive match - be damned if you have files which differ only by 
            // case in the same directory on Unix.
            path = path.Replace('\\', '/').ToUpperInvariant();

            foreach (var kvp in m_SourcesOverride)
                if (kvp.Value.Replace('\\', '/').ToUpperInvariant() == path)
                    return m_SourcesMap[kvp.Key];

            return m_SourcesMap.Values.FirstOrDefault(s => s.Name.Replace('\\', '/').ToUpperInvariant() == path);
        }

        void IDebugger.SetDebugService(DebugService debugService)
        {
            DebugService = debugService;
        }

        public DynValue Evaluate(string expression)
        {
            var expr = CreateDynExpr(expression);
            return expr.Evaluate();
        }

        DebuggerCaps IDebugger.GetDebuggerCaps()
        {
            return DebuggerCaps.CanDebugSourceCode | DebuggerCaps.HasLineBasedBreakpoints;
        }
    }
}

#endif