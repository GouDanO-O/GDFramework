/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using GDFramework_Core.Scripts.GDFrameworkCore;
using UnityEngine;

namespace GDFrameworkExtend.CoreKit
{
    public class PlayerPrefsFloatProperty : BindableProperty<float>
    {
        public PlayerPrefsFloatProperty(string saveKey, float defaultValue = 0.0f)
        {
            mValue =  PlayerPrefs.GetFloat(saveKey, defaultValue);

            this.Register(value => PlayerPrefs.SetFloat(saveKey, value));
        }
    }
}