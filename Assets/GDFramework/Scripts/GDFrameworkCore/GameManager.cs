using System;
using Game.Procedure;
using GDFramework.Models;
using GDFramework.Procedure;
using GDFrameworkCore;

using GDFrameworkExtend.SingletonKit;

namespace GDFrameworkCore
{
    public partial class GameManager : MonoSingleton<GameManager>, IController, ICanSendEvent
    {
        private ProcedureManager _procedureManager;

        private GameDataModel _gameDataModel;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        private GameManager()
        {
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitComponent();
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
            _procedureManager = this.GetSystem<ProcedureManager>();
            _procedureManager.RegisterProcedure(EProcedureType.InitialGame,new InitialGameProcedure());
            _procedureManager.RegisterProcedure(EProcedureType.Launch, new LaunchProcedure());
            _procedureManager.RegisterProcedure(EProcedureType.MainMenu, new MainMenuProcedure());

            this.SendEvent<SChangeProcedureEvent>(new SChangeProcedureEvent(EProcedureType.InitialGame));
        }
    }
}