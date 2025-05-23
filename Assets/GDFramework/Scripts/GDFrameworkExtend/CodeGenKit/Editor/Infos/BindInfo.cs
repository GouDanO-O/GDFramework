/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;

namespace GDFrameworkExtend.CodeGenKit
{
    /// <summary>
    /// 存储一些Mark相关的信息
    /// </summary>
    [Serializable]
    public class BindInfo
    {
        public string TypeName;

        public string PathToRoot;

        public IBindOld BindScript;
        
        public string MemberName;
    }

    [Serializable]
    public class BindInfoGroup
    {
        
    }
}
#endif