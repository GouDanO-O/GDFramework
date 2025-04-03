/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


namespace GDFrameworkExtend._CoreKit.ActionKit
{
    internal class Callback : IAction
    {
        private Callback()
        {
        }

        private System.Action mCallback;

        private static SimpleObjectPool<Callback> mSimpleObjectPool = new(() => new Callback(), null, 10);

        public static Callback Allocate(System.Action callback)
        {
            var callbackAction = mSimpleObjectPool.Allocate();
            callbackAction.ActionID = ActionKit.ID_GENERATOR++;
            callbackAction.Reset();
            callbackAction.Deinited = false;
            callbackAction.mCallback = callback;
            return callbackAction;
        }

        public bool Paused { get; set; }
        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            mCallback?.Invoke();
            this.Finish();
        }

        public void OnExecute(float dt)
        {
        }

        public void OnFinish()
        {
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mCallback = null;
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<Callback>(mSimpleObjectPool, this));
            }
        }

        public void Reset()
        {
            Paused = false;
            Status = ActionStatus.NotStart;
        }
    }

    public static class CallbackExtension
    {
        public static ISequence Callback(this ISequence self, System.Action callback)
        {
            return self.Append(Action.Callback.Allocate(callback));
        }
    }
}