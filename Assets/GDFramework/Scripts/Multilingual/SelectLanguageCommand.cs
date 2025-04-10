﻿using GDFramework.Models.Enums;
using GDFrameworkCore;

namespace GDFramework.Multilingual
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