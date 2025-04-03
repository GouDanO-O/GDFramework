/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using GDFramework_Core.Scripts.GDFrameworkExtend._CoreKit.ActionKit.Scripts.Framework;
using UnityEngine;

namespace GDFramework_Core.Scripts.GDFrameworkExtend._CoreKit.ActionKit.Scripts.ShortCut
{
    public static class AudioSourceShortCutExtensions
    {
        public static IAction PlayAction(this AudioSource self)
        {
            return ActionKit.Custom<AudioSource>(api =>
            {
                api.OnStart(() =>
                {
                    api.Data = self;
                    self.Play();
                }).OnExecute(_ =>
                {
                    if (api.Data && !api.Data.isPlaying) api.Finish();
                });
            });
        }
    }
}