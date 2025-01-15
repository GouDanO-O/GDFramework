using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// 场景名称
    /// </summary>
    public enum ESceneName : byte
    {
        Menu=1,
        GameScene=2,
        TestScene=3
    }
    
    public class SceneLoader : AbstractSystem
    {
        public UnityAction<ESceneName> onLoadScene;

        public UnityAction OnSceneLoadStart;

        public UnityAction<float> OnSceneLoading;

        public UnityAction<ESceneName> OnSceneLoadComplete;

        protected override void OnInit()
        {
            onLoadScene += LoadSceneAsync;
        }

        protected override void OnDeinit()
        {
            base.OnDeinit();
            onLoadScene -= LoadSceneAsync;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        private void LoadSceneAsync(ESceneName sceneName)
        {
            Main.Interface.GetUtility<Coroutine_Utility>().StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        private IEnumerator LoadSceneCoroutine(ESceneName sceneName)
        {
            OnSceneLoadStart?.Invoke();
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)sceneName);

            while (asyncOperation.progress < 0.9f)  // 进度小于0.9时
            {
                OnSceneLoading?.Invoke(asyncOperation.progress);
                yield return null;
            }
            
            while (!asyncOperation.isDone)  // 场景最终激活完成
            {
                yield return null;
            }
            
            OnSceneLoadComplete?.Invoke(sceneName);
        }

    }
}

