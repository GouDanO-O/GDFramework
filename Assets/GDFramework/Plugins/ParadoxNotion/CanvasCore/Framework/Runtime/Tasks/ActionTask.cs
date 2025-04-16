using System;
using System.Collections;
using ParadoxNotion.Services;
using ParadoxNotion.Serialization;
using ParadoxNotion.Serialization.FullSerializer;
using NodeCanvas.Framework.Internal;
using UnityEngine;


namespace NodeCanvas.Framework
{

    ///<summary>
    /// 行为的基类
    /// 扩展它来创建您自己的
    /// T是操作所需的agentType
    /// 通用版本，其中T是操作所需的AgentType（组件或接口）
    /// 对于GameObject，使用“Transform”
    /// </summary>
    abstract public class ActionTask<T> : ActionTask where T : class
    {
        sealed public override Type agentType => typeof(T);
        new public T agent => base.agent as T;
    }

    
    /// <summary>
    /// 所有动作的基类
    /// 扩展它来创建您自己的
    /// </summary>
#if UNITY_EDITOR //handles missing types
    [fsObject(Processor = typeof(fsRecoveryProcessor<ActionTask, MissingAction>))]
#endif
    public abstract class ActionTask : Task
    {
        private Status status = Status.Resting;
        private float timeStarted;
        private bool latch;

        ///<summary>
        /// 此操作运行的时间（以秒为单位）
        /// </summary>
        public float elapsedTime => ( isRunning ? ownerSystem.elapsedTime - timeStarted : 0 );

        ///<summary>
        /// 操作当前是否正在运行？
        /// </summary>
        public bool isRunning => status == Status.Running;

        ///<summary>
        /// 操作当前是否暂停？
        /// </summary>
        public bool isPaused { get; private set; }

        ///----------------------------------------------------------------------------------------------

        ///<summary>
        /// 用于调用提供回调的独立执行操作
        /// 这将使动作作为协程执行
        /// </summary>
        public void ExecuteIndependent(Component agent, IBlackboard blackboard, Action<Status> callback) {
            if ( !isRunning ) { MonoManager.current.StartCoroutine(IndependentActionUpdater(agent, blackboard, callback)); }
        }

       
        /// <summary>
        /// 内部更新程序,用于在带有回调参数的操作被调用时使用
        /// 这只在用户需要完全独立执行操作任务时才有用
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="blackboard"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator IndependentActionUpdater(Component agent, IBlackboard blackboard, Action<Status> callback) {
            while ( Execute(agent, blackboard) == Status.Running ) { yield return null; }
            if ( callback != null ) { callback(status); }
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>
        /// 勾出所提供的代理和黑板的动作
        /// </summary>
        public Status Execute(Component agent, IBlackboard blackboard) {

            if ( !isUserEnabled ) {
                return Status.Optional;
            }

            if ( isPaused ) {
                OnResume();
            }

            isPaused = false;
            if ( status == Status.Running ) {
                OnUpdate();
                latch = false;
                return status;
            }

            //latch用于能够在任何地方调用EndAction
            if ( latch ) {
                latch = false;
                return status;
            }

            if ( !Set(agent, blackboard) ) {
                latch = false;
                return Status.Failure;
            }

            timeStarted = ownerSystem.elapsedTime;
            status = Status.Running;
            OnExecute();
            if ( status == Status.Running ) {
                OnUpdate();
            }
            latch = false;
            return status;
        }

        ///<summary>
        /// 以成功或失败结束动作
        /// 以null结尾意味着这是一个取消/中断
        /// Null由外部系统使用
        /// 在它内部调用EndAction时应该使用true或false
        /// </summary>
        public void EndAction() { EndAction(true); }
        public void EndAction(bool success) { EndAction((bool?)success); }
        public void EndAction(bool? success) {

            if ( status != Status.Running ) {
                if ( success == null ) { latch = false; }
                return;
            }

            latch = success != null ? true : false;

            isPaused = false;
            status = success == null ? Status.Resting : ( success == true ? Status.Success : Status.Failure );
            OnStop(success == null);
        }

        ///<summary>
        /// 暂停动作更新并调用OnPause
        /// </summary>
        public void Pause() {

            if ( status != Status.Running ) {
                return;
            }

            isPaused = true;
            OnPause();
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>
        /// 在动作执行时调用一次
        /// </summary>
        virtual protected void OnExecute() { }
        
        ///<summary>
        /// 每一帧调用
        /// 如果和当动作正在运行，直到它结束
        /// </summary>
        virtual protected void OnUpdate() { }

        ///<summary>
        /// 当操作因任何原因结束时调用
        /// 参数表示操作是否被中断或正确完成
        /// </summary>
        virtual protected void OnStop(bool interrupted)
        {
            OnStop();
        }
        
        
        ///<summary>
        /// 当操作因任何原因结束时调用
        /// </summary>
        virtual protected void OnStop() { }
        
        ///<summary>
        /// 当动作暂停时调用
        /// </summary>
        virtual protected void OnPause() { }
        
        ///<summary>
        /// 当动作暂停后恢复时调用
        /// </summary>
        virtual protected void OnResume() { }
        ///----------------------------------------------------------------------------------------------

        [System.Obsolete("Use 'Execute'")]
        public Status ExecuteAction(Component agent, IBlackboard blackboard) { return Execute(agent, blackboard); }
    }
}