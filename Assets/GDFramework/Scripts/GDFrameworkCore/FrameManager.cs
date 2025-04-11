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
        [SerializeField] private EPlayMode playMode;
        
        [SerializeField] private bool willShowCheatWindow = false;

        [SerializeField] private bool willShowLogWindow = false;

        public EPlayMode PlayMode
        {
            get
            {
                return playMode;
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
       
        // protected void DrawGUI()
        // {
        //     if (!canShowGUI)
        //         return;
        //     
        //     var showCount = -1;
        //     if (willShowLogWindow)
        //     {
        //         showCount++;
        //         if (GUI.Button(new Rect(20 + showCount * 150, 0, 120, 30),
        //                 _logMonoUtility.isShowing ? "关闭日志系统" : "打开日志系统"))
        //         {
        //             _logMonoUtility.CheckButtonWillShow();
        //             if (_logMonoUtility.isShowing && _cheatMonoUtility)
        //                 if (_cheatMonoUtility.isShowing)
        //                     _cheatMonoUtility.CheckButtonWillShow();
        //         }
        //     }
        //
        //     if (willShowCheatWindow)
        //     {
        //         showCount++;
        //         if (GUI.Button(new Rect(20 + showCount * 150, 0, 120, 30),
        //                 _cheatMonoUtility.isShowing ? "关闭作弊系统" : "打开作弊系统"))
        //         {
        //             _cheatMonoUtility.CheckButtonWillShow();
        //             if (_cheatMonoUtility.isShowing && _logMonoUtility)
        //                 if (_logMonoUtility.isShowing)
        //                     _logMonoUtility.CheckButtonWillShow();
        //         }
        //     }
        // }
    }
}