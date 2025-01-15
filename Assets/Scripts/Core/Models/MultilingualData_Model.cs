using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct SLanguageDataWrapper
    {
        public SLanguageData[] languageData;
    }
        
    [System.Serializable]
    public struct SLanguageData
    {
        public string Key;
            
        public string SimplifiedChinese;
            
        public string English;
    }
    
    public class MultilingualData_Model : AbstractModel
    {
        public TextAsset languageTextAsset;
        
        public ELanguageType willChangeLanguageType = ELanguageType.SimplifiedChinese;
        
        public Dictionary<string, Dictionary<ELanguageType, string>> translations = new Dictionary<string, Dictionary<ELanguageType, string>>();
        
        private Resouces_Utility m_ResoucesUtility;
        
        protected override void OnInit()
        {
            m_ResoucesUtility = this.GetUtility<Resouces_Utility>();
        }

        public void SetTranslation(string key, Dictionary<ELanguageType, string> languageData)
        {
            translations[key] = languageData;
        }
        
        /// <summary>
        /// 获取当前多语言文本
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMultilingual_Text(string key)
        {
            if (translations.Count == 0)
                return "";
            
            if (translations.ContainsKey(key) && translations[key].ContainsKey(willChangeLanguageType))
            {
                return translations[key][willChangeLanguageType];
            }

            return "";
        }
        
        /// <summary>
        /// 获取当前多语言图片--从Resources目录读取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Sprite GetMultilingual_Sprite_Res(string key)
        {
            string folderName = willChangeLanguageType.ToString();
            string path = $"Multilingual/{folderName}/{key}";
            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite == null) 
            {
                Debug.LogWarning($"Sprite not found at path: {path}");
            }
            return sprite;
        }
        
        /// <summary>
        /// 获取当前多语言图片--AB包
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void GetMultilingual_Sprite_AB(string key,Action<Sprite> callback)
        {
            string folderName = willChangeLanguageType.ToString();
            string path = $"Multilingual/{folderName}/{key}";
            m_ResoucesUtility.LoadSpritesAsync(path,callback);
        }
    }
}