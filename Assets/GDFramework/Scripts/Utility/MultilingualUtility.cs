﻿using System.Collections.Generic;
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

    public class MultilingualUtility : IUtility
    {
        private MultilingualDataModel _multilingualData_Model;

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        public void InitLanguage(MultilingualDataModel multilingualData_Model)
        {
            _multilingualData_Model = multilingualData_Model;

            var languageTextAsset = _multilingualData_Model.LanguageTextAsset;
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