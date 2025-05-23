/****************************************************************************
 * Copyright (c) 2018.3 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/


using GDFrameworkExtend.EventKit;
using GDFrameworkExtend.PoolKit;

namespace GDFrameworkExtend.UIKit
{
    using System;

    /// <summary>
    /// manager基类
    /// </summary>
    public abstract class QMgrBehaviour : QMonoBehaviour, IManager
    {
        private readonly Lazy<EnumEventSystem> mEventSystem =
            new Lazy<EnumEventSystem>(ObjectFactory.CreateNonPublicConstructorObject<EnumEventSystem>);

        #region IManager

        public virtual void Init()
        {
        }

        #endregion

        public abstract int ManagerId { get; }

        public override IManager Manager
        {
            get { return this; }
        }

        public void RegisterEvent<T>(T msgId, Action<int,object[]> process) where T : IConvertible
        {
            mEventSystem.Value.Register(msgId, process);
        }

        public void UnRegisterEvent<T>(T msgEvent, Action<int,object[]> process) where T : IConvertible
        {
            mEventSystem.Value.UnRegister(msgEvent, process);
        }

        public override void SendMsg(IMsg msg)
        {
            if (msg.ManagerID == ManagerId)
            {
                Process(msg.EventID, msg);
            }
            else
            {
                QMsgCenter.Instance.SendMsg(msg);
            }
        }

        public override void SendEvent<T>(T eventId)
        {
            SendMsg(QMsg.Allocate(eventId));
        }


        // 来了消息以后,通知整个消息链
        protected override void ProcessMsg(int eventId, QMsg msg)
        {
            mEventSystem.Value.Send(msg.EventID, msg);
        }

        protected override void OnBeforeDestroy()
        {
            if (mEventSystem.IsValueCreated)
            {
                mEventSystem.Value.UnRegisterAll();
            }
        }
    }
}