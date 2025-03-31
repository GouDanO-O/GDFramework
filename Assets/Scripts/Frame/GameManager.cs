using Frame.Game.Resource;
using Frame.Models;
using Frame.Procedure;
using Frame.Scene;
using Frame.Utility;
using Frame.View;
using QFramework;
using UnityEngine;

namespace Frame
{
    public class GameManager : MonoSingleton<GameManager>,IController
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
            _procedureManager = ProcedureManager.Instance;
            _procedureManager.RegisterProcedure(new Procedure_Launch());
            _procedureManager.RegisterProcedure(new Procedure_MainMenu());
            
            _procedureManager.ChangeProcedure<Procedure_Launch>();
        }
    }
}

