using System;
using GDFramework.Models;
using GDFramework.Procedure;
using GDFrameworkExtend.SingletonKit;
using UnityEngine;
using YooAsset;

namespace GDFrameworkCore
{
    /// <summary>
    /// 框架管理器
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class FrameManager : MonoSingleton<FrameManager>, IController, ICanSendEvent
    {
        [SerializeField] private EPlayMode yooAssetPlayMode;
        
        [SerializeField] private bool willShowCheatWindow = false;

        [SerializeField] private bool willShowLogWindow = false;

        public EPlayMode YooAssetPlayMode
        {
            get
            {
                return yooAssetPlayMode;
            }
        }

        public bool WillShowCheatWindow
        {
            get
            {
                return willShowCheatWindow;
            }
        }

        public bool WillShowLogWindow
        {
            get
            {
                return willShowLogWindow;
            }
        }
        
        protected ProcedureManager _procedureManager;
        
        protected GameDataModel _gameDataModel;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        protected FrameManager()
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

        #region Init
        
        /// <summary>
        /// 初始化管理类和组件
        /// </summary>
        protected virtual void InitComponent()
        {
            _gameDataModel = this.GetModel<GameDataModel>();
            InitProcedure();
            SetInitialProcedure();
        }

        /// <summary>
        /// 初始化流程
        /// </summary>
        protected virtual void InitProcedure()
        {
            _procedureManager = new ProcedureManager();
            _procedureManager.Init();
            _procedureManager.RegisterFsmNode(new LoadResProcedure());
            _procedureManager.RegisterFsmNode(new InitialFrameProcedure());
        }

        /// <summary>
        /// 设置起始流程
        /// 默认为框架加载流程
        /// </summary>
        public void SetInitialProcedure()
        {
            this.SendEvent<SChangeProcedureEvent>(new SChangeProcedureEvent(typeof(LoadResProcedure)));
        }
        

        #endregion
    }
}