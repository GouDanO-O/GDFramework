using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.U2D;

namespace Game
{
    public class Resouces_Utility : IUtility
    {
        private ResLoader resLoader;

        public void InitLoader()
        {
            if (resLoader == null)
            {
                ResKit.Init();
                resLoader = ResLoader.Allocate();
            }
        }

        /// <summary>
        /// 加载图集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void LoadSpriteAtlasAsync(string name, Action<SpriteAtlas> action)
        {
            resLoader.Add2Load(name, (succeed, res) => {
                if (succeed)
                {
                    action?.Invoke(res.Asset.As<SpriteAtlas>());
                }
            });
            resLoader.LoadAsync();
        }

        /// <summary>
        /// 加载AB精灵图
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void LoadSpritesAsync(string name, Action<Sprite> action)
        {
            resLoader.Add2Load(name, (succeed, res) => {
                if (succeed)
                {
                    action?.Invoke(res.Asset.As<Sprite>());
                }
            });
            resLoader.LoadAsync();
        }

        /// <summary>
        /// 加载音频
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void LoadAudioAsync(string name, Action<AudioClip> action)
        {
            resLoader.Add2Load(name, (succeed, res) => {
                if (succeed)
                {
                    action?.Invoke(res.Asset.As<AudioClip>());
                }
            });
            resLoader.LoadAsync();
        }

        /// <summary>
        /// 加载预制体
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void LoadPrefabAsync(string name, Action<GameObject> action)
        {
            resLoader.Add2Load(name, (succeed, res) => {
                if (succeed)
                {
                    action?.Invoke(res.Asset.As<GameObject>());
                }
            });
            resLoader.LoadAsync();
        }

        /// <summary>
        /// 加载ScriptableObject
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void LoadScriptObjAsync<T>(string name, Action<T> action) where T : ScriptableObject
        {
            resLoader.Add2Load(name, (succeed, res) => {
                if (succeed)
                {
                    T loadedObject = res.Asset as T;
                    if (loadedObject != null)
                    {
                        action?.Invoke(loadedObject);
                    }
                    else
                    {
                        Debug.LogError($"错误的加载类型:{typeof(T)}.");
                    }
                }
                else
                {
                    Debug.LogError($"加载错误:{name}");
                }
            });

            resLoader.LoadAsync();
        }
        
        /// <summary>
        /// 加载Json
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void LoadJsonAsync(string name, Action<TextAsset> action)
        {
            resLoader.Add2Load(name, (succeed, res) => {
                if (succeed)
                {
                    action?.Invoke(res.Asset.As<TextAsset>());
                }
            });
            resLoader.LoadAsync();
        }
    }
}

