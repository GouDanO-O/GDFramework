using System.Collections.Generic;
using GDFramework.Models;
using GDFramework.Models.Enums;
using GDFrameworkCore;
using UnityEngine;

namespace GDFramework.Utility
{
    [System.Serializable]
    public struct SLanguageDataWrapper
    {
        public SLanguageData[] data;
    }

    [System.Serializable]
    public struct SLanguageData
    {
        public string Key;

        public string Zh;

        public string En;
    }

    public class MultilingualUtility : BasicToolUtility
    {
        private MultilingualDataModel _multilingualDataModel;

        public override void InitUtility()
        {
            
        }

        public void InitLanguage(MultilingualDataModel multilingualData_Model)
        {
            _multilingualDataModel = multilingualData_Model;

            var languageTextAsset = _multilingualDataModel.LanguageTextAsset;
            if (languageTextAsset)
            {
                var jsonText = languageTextAsset.text;
                var languageDataArray = JsonUtility.FromJson<SLanguageDataWrapper>(jsonText).data;

                if(languageDataArray==null || languageDataArray.Length==0)
                    return;
                
                foreach (var entry in languageDataArray)
                {
                    var languageDictionary = new Dictionary<ELanguageType, string>
                    {
                        {
                            ELanguageType.SimplifiedChinese, entry.Zh
                        },
                        {
                            ELanguageType.English, entry.En
                        }
                    };
                    _multilingualDataModel.SetTranslation(entry.Key, languageDictionary);
                }
            }
            else
            {
                Debug.LogError("文本资源不存在");
            }
        }
    }
}