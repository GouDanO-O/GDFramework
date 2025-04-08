/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using GDFrameworkCore;
using UnityEngine;

namespace GDFrameworkExtend.ResKit
{
    public class ResKit
    {
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        public static void CheckAutoInit()
        {
            if (PlatformCheck.IsEditor && AssetBundlePathHelper.SimulationMode)
            {
                Init();
            }
        }
#endif
        
        /// <summary>
        /// 初始化 ResKit
        /// </summary>
        public static void Init()
        {
            ResMgr.Init();
        }
        
        /// <summary>
        /// 异步初始化 ResKit,如果是 WebGL 平台，只支持异步初始化
        /// </summary>
        /// <returns></returns>
        public static IEnumerator InitAsync()
        {
            yield return ResMgr.InitAsync();
        }

        private static readonly Lazy<ResKit> mInstance = new Lazy<ResKit>(() => new ResKit().InternalInit());
        internal static ResKit Get => mInstance.Value;

        internal IOCContainer Container = new IOCContainer();

        ResKit InternalInit()
        {
            Container.Register<IZipFileHelper>(new ZipFileHelper());
            Container.Register<IBinarySerializer>(new BinarySerializer());
            return this;
        }
    }
}