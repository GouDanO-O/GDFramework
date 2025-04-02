using System;
using System.Collections;
using QFramework;
using UnityEngine;

namespace GDFramework.Utility
{
    public class Coroutine_Utility : BasicTool_Utility_Mono
    {
        protected override void InitUtility()
        {
            Main.Interface.RegisterUtility(this);
        }

        /// <summary>
        /// 异步生成UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level"></param>
        public void SpawnUI_Async<T>(UILevel level=UILevel.Common, Action complete = null) where T : UIPanel
        {
            StartCoroutine(IemSpawnUI_Async<T>(level, complete));
        }

        protected IEnumerator IemSpawnUI_Async<T>(UILevel level=UILevel.Common, Action complete=null) where T : UIPanel
        {
            yield return UIKit.OpenPanelAsync<T>(level);
            complete?.Invoke();
        }
        
        /// <summary>
        /// 开始一个协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public void StartRoutine(IEnumerator routine)
        {
            StartCoroutine(routine);
        }

        /// <summary>
        /// 开始一个协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public Coroutine GetAndStartRoutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        /// <summary>
        /// 开始一个协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public void StartRoutine_Action(IEnumerator routine)
        {
            ActionKit.Coroutine(()=>routine).Start(this);
        }

        /// <summary>
        /// 开始一个协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public IAction GetAndStartRoutine_Action(IEnumerator routine)
        {
            return ActionKit.Coroutine(() => routine);
        }

        // 停止协程
        public void StopRoutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        // 停止协程
        public void StopRoutine(IActionController coroutine)
        {
            if (coroutine != null)
            {
                coroutine.Deinit();
                coroutine.Recycle();
            }
        }

        // 停止所有协程
        public void StopAllRoutines()
        {
            StopAllCoroutines();
        }
    }
}

