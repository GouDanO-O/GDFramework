using GDFramework_Core.Models.Enums;
using QFramework;

namespace GDFramework_Core.Multilingual
{
    public class SelectLanguageCommand : AbstractCommand
    {
        private ELanguageType _languageType;
        
        public SelectLanguageCommand(ELanguageType willChangeLanguage)
        {
            _languageType = willChangeLanguage;
        }
        
        protected override void OnExecute()
        {
            this.GetSystem<MultilingualManager>().ChangeLanguage(_languageType);
        }
    }

}