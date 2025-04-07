using System.Collections;
using GDFramework_Core.Scripts.GDFrameworkCore;
using GDFramework_Core.Utility;
using GDFramework;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GDFramework_Core.Scene
{
    /// <summary>
    /// 场景名称
    /// </summary>
    public enum ESceneName : byte
    {
        Menu = 1,
        GameScene = 2,
        TestScene = 3
    }

    public class SceneManager : AbstractSystem
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
            Main.Interface.GetUtility<CoroutineMonoUtility>().StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        private IEnumerator LoadSceneCoroutine(ESceneName sceneName)
        {
            OnSceneLoadStart?.Invoke();
            var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)sceneName);

            while (asyncOperation.progress < 0.9f)
            {
                OnSceneLoading?.Invoke(asyncOperation.progress);
                yield return null;
            }

            while (!asyncOperation.isDone) yield return null;

            OnSceneLoadComplete?.Invoke(sceneName);
        }
    }
}