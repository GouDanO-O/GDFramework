using GDFramework.Models;
using GDFramework.Models.Enums;
using GDFrameworkCore;

namespace GDFramework.Multilingual
{
    public class MultilingualManager : AbstractSystem
    {
        private MultilingualDataModel _multilingualData_Model;

        protected override void OnInit()
        {
            _multilingualData_Model = this.GetModel<MultilingualDataModel>();
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