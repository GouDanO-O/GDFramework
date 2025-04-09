using System;
using System.Collections.Generic;
using Game.Procedure;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;

namespace GDFramework.Procedure
{
    public class ProcedureManager : AbstractSystem
    {
        private readonly Dictionary<Type, ProcedureBase> _procedureDict = new();

        private ProcedureBase _currentProcedure;
        private ProcedureBase _lastProcedure;

        public bool Initialized { get; set; }

        public ProcedureBase CurrentProcedure => _currentProcedure;
        public ProcedureBase LastProcedure    => _lastProcedure;

        protected override void OnInit()
        {
            // ② 监听“切流程”事件
            this.RegisterEvent<SChangeProcedureEvent>(evt => ChangeProcedure(evt._procedureType));
        }

        protected void DeInit()
        {
            this.UnRegisterEvent<SChangeProcedureEvent>(evt => { });
        }

        #region 注册 / 获取

        ///<summary>
        /// 根据 Type 取流程
        /// </summary>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase =>
            GetProcedure(typeof(T));

        public ProcedureBase GetProcedure(Type type)
        {
            if (_procedureDict.TryGetValue(type, out var p)) return p;
            LogMonoUtility.AddLog($"流程 {type.Name} 未注册");
            return null;
        }

        ///<summary>
        /// 注册流程（Type = procedure.GetType()）
        /// </summary>
        public void RegisterProcedure(ProcedureBase procedure)
        {
            if (procedure == null)
            {
                LogMonoUtility.AddLog("流程为空");
                return;
            }

            var type = procedure.GetType();
            if (_procedureDict.ContainsKey(type))
            {
                LogMonoUtility.AddLog($"流程 {type.Name} 已注册");
                return;
            }

            procedure.OnInit();
            _procedureDict.Add(type, procedure);
        }

        #endregion

        #region 切换 / 更新

        /// <summary>
        /// 切换流程（通过 Type）
        /// </summary>
        public void ChangeProcedure<T>() where T : ProcedureBase =>
            ChangeProcedure(typeof(T));

        public void ChangeProcedure(Type type)
        {
            if (!_procedureDict.TryGetValue(type, out var next))
            {
                LogMonoUtility.AddErrorLog($"无法切换，流程 {type.Name} 未注册");
                return;
            }

            if (_currentProcedure != null)
            {
                if (!_currentProcedure.OnExitCondition()) 
                    return;
                _currentProcedure.OnExit();
                _lastProcedure = _currentProcedure;
            }

            if (!next.OnEnterCondition())
                return;

            _currentProcedure = next;
            _currentProcedure.OnEnter();
            Debug.Log($"切换流程: {type.Name}");
        }

        public void UpdateProcedure() => _currentProcedure?.OnUpdate();

        #endregion
    }
}
