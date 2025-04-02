using System;
using System.Collections.Generic;
using GDFramework_Core.Utility;
using GDFramework;
using QFramework;
using UnityEngine;

namespace GDFramework_Core.Procedure
{
    public class ProcedureManager : Singleton<ProcedureManager>
    {
        private Dictionary<Type, ProcedureBase> _procedureDict = new  Dictionary<Type, ProcedureBase>();
        
        private ProcedureBase _currentProcedure;
        
        private ProcedureBase _lastProcedure;

        private ProcedureManager()
        {
            
        }
        
        /// <summary>
        /// 当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                return _currentProcedure;
            }
        }

        /// <summary>
        /// 上一个流程
        /// </summary>
        public ProcedureBase LastProcedure
        {
            get
            {
                return _lastProcedure;
            }
        }
        
        public bool Initialized { get; set; }
        
        public void Init()
        {
            
        }

        public void Deinit()
        {
            
        }

        /// <summary>
        /// 单例初始化
        /// </summary>
        public void OnSingletonInit()
        {
            _procedureDict = new Dictionary<Type, ProcedureBase>();
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            
        }
        
        /// <summary>
        /// 获取流程
        /// </summary>
        public T GetProcedure<T>() where T : ProcedureBase
        {
            Type procedureType = typeof(T);
            if (_procedureDict.TryGetValue(procedureType, out ProcedureBase procedure))
            {
                return (T)procedure;
            }

            return null;
        }
        
        /// <summary>
        /// 注册流程
        /// </summary>
        public void RegisterProcedure(ProcedureBase procedure)
        {
            if (procedure == null)
            {
                Log_Utility.AddLog("流程为空");
                return;
            }

            Type procedureType = procedure.GetType();
            if (_procedureDict.ContainsKey(procedureType))
            {
                Log_Utility.AddLog($"'{procedureType.FullName}'流程已注册");
                return;
            }

            procedure.OnInit();
            _procedureDict.Add(procedureType, procedure);
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        public void ChangeProcedure<T>() where T : ProcedureBase
        {
            Type procedureType = typeof(T);
            if (!_procedureDict.TryGetValue(procedureType, out ProcedureBase procedure))
            {
                Log_Utility.AddErrorLog($"无法改变流程,'{procedureType.FullName}'不存在");
                return;
            }

            if (_lastProcedure != null)
            {
                //当上一个流程的退出条件不满足时,无法进行切换
                if (!_lastProcedure.OnExitCondition())
                {
                    return;
                }
                _lastProcedure.OnExit();
            }
            
            //当要切换的进程条件不满足时
            if (!procedure.OnEnterCondition())
            {
                return;
            }
            
            _lastProcedure = _currentProcedure;
            _currentProcedure = procedure;

            _currentProcedure.OnEnter();
        }

        /// <summary>
        /// 更新流程
        /// </summary>
        public void UpdateProcedure()
        {
            if (_currentProcedure == null)
            {
                return;
            }
            
            _currentProcedure.OnUpdate();
        }
    }
}