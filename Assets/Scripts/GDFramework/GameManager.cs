
using GDFramework.Models;
using GDFramework.Procedure;
using GDFramework.Scene;
using GDFramework.Utility;
using GDFramework.View;
using QFramework;
using UnityEngine;

namespace GDFramework
{
    public partial class GameManager : MonoSingleton<GameManager>,IController
    {
        private ProcedureManager _procedureManager;
        
        private GameData_Model _gameData_Model;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        private GameManager()
        {
            
        }
        
        private void Awake()
        {
            InitComponent();
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 初始化管理类和组件
        /// </summary>
        protected void InitComponent()
        {
            RegisterProcedure();
        }

        /// <summary>
        /// 注册流程
        /// </summary>
        protected void RegisterProcedure()
        {
            _procedureManager = ProcedureManager.Instance;
            _procedureManager.RegisterProcedure(new Procedure_Launch());
            _procedureManager.RegisterProcedure(new Procedure_MainMenu());
            
            _procedureManager.ChangeProcedure<Procedure_Launch>();
        }
    }
}

