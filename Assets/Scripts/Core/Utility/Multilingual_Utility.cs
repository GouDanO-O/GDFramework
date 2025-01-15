using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace Game
{
    public class Multilingual_Utility : IUtility,ICanGetModel,ICanGetUtility
    {
        private MultilingualData_Model m_MultilingualData_Model;
        
        private TextAsset m_TextAsset;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
        
        public void InitLanguage()
        {
            m_MultilingualData_Model = this.GetModel<MultilingualData_Model>();

            TextAsset languageTextAsset = m_MultilingualData_Model.languageTextAsset;
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
                    m_MultilingualData_Model.SetTranslation(entry.Key, languageDictionary);
                }
            }
            else
            {
                Debug.LogError("文本资源不存在");
            }
        }
        



    }
}