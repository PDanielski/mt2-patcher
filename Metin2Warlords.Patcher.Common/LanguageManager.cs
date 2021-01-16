using System.Collections.Generic;

namespace MTRPatcher
{
    public delegate void ChangedLangEventHandler(object sender, Language lang);
    public class LanguageManager
    {
        List<Language> allLangs;
        Language currentLang;

        public event ChangedLangEventHandler changedLangEvent;
        public LanguageManager(List<Language> langs)
        {
            allLangs = langs;
        }
        public void changeLang(Language lang)
        {
            currentLang = lang;
            onChangedLang(lang);
        }
        public Language getCurrentLang()
        {
            return this.currentLang;
        }
        public void updateLangs(List<Language> ls)
        {
            this.allLangs = ls;
        }
        public Language getLangByCode(string code)
        {
            foreach(Language ls in allLangs)
            {
                if(ls.code == code)
                {
                    return ls;
                }
            }
            return new Language();
        }
        public void raiseChangedLang(Language lang)
        {
            currentLang = lang;
            onChangedLang(lang);
        }
        protected virtual void onChangedLang(Language lang)
        {
            if(changedLangEvent != null)
            {
                changedLangEvent(this, lang);
            }
        }
    }
}
