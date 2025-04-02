using GDFramework_Core.Event;
using GDFramework_Core.Models;
using GDFramework_Core.Models.Enums;
using GDFramework_Core.Utility;
using QFramework;
using UnityEngine;

namespace GDFramework_Core.Game.Multilingual
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

