using System;
using System.Collections.Generic;
using Game.Procedure;
using GDFramework_Core.Scripts.GDFrameworkCore;
using GDFramework_Core.Utility;
using GDFramework;
using GDFrameworkExtend.CoreKit;
using UnityEngine;

namespace GDFramework_Core.Procedure
{
    public class ProcedureManager : AbstractSystem
    {
        private Dictionary<EProcedureType, ProcedureBase> _procedureDict;

        private ProcedureBase _currentProcedure;

        private ProcedureBase _lastProcedure;

        public bool Initialized { get; set; }
        
        /// <summary>
        /// 当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure => _currentProcedure;

        /// <summary>
        /// 上一个流程
        /// </summary>
        public ProcedureBase LastProcedure => _lastProcedure;
        
        protected override void OnInit()
        {
            _procedureDict = new Dictionary<EProcedureType, ProcedureBase>();
            this.RegisterEvent<SChangeProcedureEvent>((eventData) =>
            {
                ChangeProcedure(eventData._procedureType);
            });
        }

        protected void DeInit()
        {
            this.UnRegisterEvent<SChangeProcedureEvent>((eventData) => { });
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        public ProcedureBase GetProcedure(EProcedureType procedureType)
        {
            if (_procedureDict.TryGetValue(procedureType, out var procedure)) 
                return procedure;

            LogMonoUtility.AddLog("流程为空");
            return null;
        }

        /// <summary>
        /// 注册流程
        /// </summary>
        public void RegisterProcedure(EProcedureType procedureType, ProcedureBase procedure)
        {
            if (procedure == null)
            {
                LogMonoUtility.AddLog("流程为空");
                return;
            }

            if (_procedureDict.ContainsKey(procedureType))
            {
                LogMonoUtility.AddLog($"'{procedureType}'流程已注册");
                return;
            }

            procedure.OnInit();
            _procedureDict.Add(procedureType, procedure);
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        public void ChangeProcedure(EProcedureType procedureType)
        {
            if (!_procedureDict.TryGetValue(procedureType, out var procedure))
            {
                LogMonoUtility.AddErrorLog($"无法改变流程,'{procedureType}'不存在");
                return;
            }

            if (_lastProcedure != null)
            {
                //当上一个流程的退出条件不满足时,无法进行切换
                if (!_lastProcedure.OnExitCondition()) 
                    return;
                _lastProcedure.OnExit();
            }

            //当要切换的进程条件不满足时
            if (!procedure.OnEnterCondition()) 
                return;

            _lastProcedure = _currentProcedure;
            _currentProcedure = procedure;
            _currentProcedure.OnEnter();
            Debug.Log("切换流程:"+procedure);
        }

        /// <summary>
        /// 更新流程
        /// </summary>
        public void UpdateProcedure()
        {
            if (_currentProcedure == null) 
                return;

            _currentProcedure.OnUpdate();
        }
    }
}