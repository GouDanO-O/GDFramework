using System.Collections.Generic;
using GDFramework_Core.Models;
using GDFramework_Core.Models.Enums;
using GDFramework;
using QFramework;
using UnityEngine;

namespace GDFramework_Core.Utility
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
    
    public class Multilingual_Utility : IUtility
    {
        private MultilingualData_Model _multilingualData_Model;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
        
        public void InitLanguage(MultilingualData_Model multilingualData_Model)
        {
            _multilingualData_Model = multilingualData_Model;

            TextAsset languageTextAsset = _multilingualData_Model.LanguageTextAsset;
            if (languageTextAsset)
            {
                string jsonText = languageTextAsset.text;
                SLanguageData[] languageDataArray = JsonUtility.FromJson<SLanguageDataWrapper>(jsonText).languageData;
                
                foreach (var entry in languageDataArray)
                {
                    var languageDictionary = new Dictionary<ELanguageType, string>
                    {
                        {
                            ELanguageType.SimplifiedChinese, entry.SimplifiedChinese
                        },
                        {
                            ELanguageType.English, entry.English
                        }
                    };
                    _multilingualData_Model.SetTranslation(entry.Key, languageDictionary);
                }
            }
            else
            {
                Debug.LogError("文本资源不存在");
            }
        }
    }
}