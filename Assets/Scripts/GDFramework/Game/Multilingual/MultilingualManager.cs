using GDFramework.Event;
using GDFramework.Models;
using GDFramework.Models.Enums;
using GDFramework.Utility;
using QFramework;
using UnityEngine;

namespace GDFramework.Game.Multilingual
{
    public class MultilingualManager : AbstractSystem
    {
        private MultilingualData_Model _multilingualData_Model;
        
        protected override void OnInit()
        {
            _multilingualData_Model = this.GetModel<MultilingualData_Model>();
        }
        
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="newLanguageType"></param>
        public void ChangeLanguage(ELanguageType newLanguageType)
        {
            _multilingualData_Model.WillChangeLanguageType = newLanguageType;
            this.SendEvent(new SOnChangeLanguageEvent()
            {
                WillChangeLanguage = newLanguageType
            });
        }
    }
}

