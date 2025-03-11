using Frame.Game.Resource;
using Frame.Models;
using Frame.Scene;
using Frame.Utility;
using Frame.View;
using QFramework;
using UnityEngine;

namespace Frame
{
    public class GameManager : MonoSingleton<GameManager>,IController
    {
        private SceneLoader m_SceneLoader;
        
        private ResourcesManager m_ResourcesManager;
        
        private GameData_Model m_GameData_Model;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
        
        private void Awake()
        {
            InitComponent();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_ResourcesManager.InitialLoad();
        }

        /// <summary>
        /// 初始化管理类和组件
        /// </summary>
        protected void InitComponent()
        {
            m_SceneLoader = GetArchitecture().GetSystem<SceneLoader>();
            m_SceneLoader.OnSceneLoadStart += LoadSceneStart;
            m_SceneLoader.OnSceneLoading += LoadingScene;
            m_SceneLoader.OnSceneLoadComplete += LoadSceneComplete;

            m_ResourcesManager = GetArchitecture().GetSystem<ResourcesManager>();
            m_ResourcesManager.onFirstLoadComplete += DataLoadComplete;
        }
        
        /// <summary>
        /// 初始数据加载完成
        /// </summary>
        private void DataLoadComplete()
        {
            EnterMenu();
        }
        
        /// <summary>
        /// 进入菜单
        /// </summary>
        protected void EnterMenu()
        { 
            m_SceneLoader.onLoadScene.Invoke(ESceneName.Menu);
        }
        
        /// <summary>
        /// 开始加载场景
        /// </summary>
        protected void LoadSceneStart()
        {

            Debug.Log("开始加载场景");
        }

        /// <summary>
        /// 加载中
        /// </summary>
        /// <param name="progress"></param>
        protected void LoadingScene(float progress)
        {

            Debug.Log("加载场景中"+progress);
        }

        /// <summary>
        /// 加载场景结束
        /// </summary>
        protected void LoadSceneComplete(ESceneName sceneName)
        {

            if (sceneName == ESceneName.Menu)
            {
                GetArchitecture().GetSystem<ViewManager>().EnterMenu();
            }
            else if(sceneName == ESceneName.GameScene || sceneName == ESceneName.TestScene)
            {
                GetArchitecture().GetSystem<ViewManager>().EnterGame();
            }
            Debug.Log("加载场景结束");
        }

        public void ADCallBack(string message)
        {
            this.GetUtility<Sdk_Utility>().ADCallBack(message);
        }
    }
}

