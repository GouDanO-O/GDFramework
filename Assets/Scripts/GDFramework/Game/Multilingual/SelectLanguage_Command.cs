using GDFramework.Game.Multilingual;
using GDFramework.Models.Enums;
using QFramework;

namespace GDFramework.Command
{
    public class SelectLanguage_Command : AbstractCommand
    {
        private ELanguageType _languageType;
        
        public SelectLanguage_Command(ELanguageType willChangeLanguage)
        {
            _languageType = willChangeLanguage;
        }
        
        protected override void OnExecute()
        {
            this.GetSystem<MultilingualManager>().ChangeLanguage(_languageType);
        }
    }

}