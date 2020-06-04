using Caliburn.Micro;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using WeatherApp.Core.Models.ProgramSettings;
using WPFLocalizeExtension.Engine;

namespace WeatherApp.Core
{
    public class LocalizationManager: PropertyChangedBase
    {
        private readonly IProgramSettings _programSettings;

        public List<LocCulture> CultureList
        {
            get
            {
                if(_cultureList == null)
                {
                    _cultureList = new List<LocCulture>()
                    {
                        new LocCulture {Culture = new CultureInfo("ru")},
                        new LocCulture {Culture = new CultureInfo("en")}
                    };
                }
                return _cultureList;
            }
        }
        private List<LocCulture> _cultureList;

        public LocCulture CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                if (Set(ref _currentCulture, value))
                    SetCulture();
            }
        }
        LocCulture _currentCulture;

        public static string CultureName { get => Instance.CurrentCulture.Culture.Name; }

        public void SetCultureAtStartUp()
        {
            var culFromS = _programSettings.Culture ?? CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            var cul = Instance.CultureList.FirstOrDefault(c => c.Culture.Name == culFromS);
            if (cul != null)
            {
                Instance._currentCulture = cul;
                _programSettings.Culture = cul.CultureString;
            }
            else
            {
                var enCul = Instance.CultureList.FirstOrDefault(c => c.Culture.TwoLetterISOLanguageName == "en");
                if (enCul == null) enCul = Instance.CultureList.FirstOrDefault();
                Instance._currentCulture = enCul;
                _programSettings.Culture = enCul.CultureString;
            }

            SetCurrentThreadCulture();
        }

        public static void SetCurrentThreadCulture(CultureInfo culture = null)
        {
            if (culture == null) culture = Instance.CurrentCulture.Culture;

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture.TwoLetterISOLanguageName);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = culture;
        }

        void SetCulture()
        {
                if (Instance.CurrentCulture == null)
                {
                    SetCultureAtStartUp();
                    return;
                }

                var culture = Instance.CurrentCulture.Culture;
                _programSettings.Culture = culture.Name;
                SetCurrentThreadCulture(culture);
        }

        #region Singleton
        public static volatile LocalizationManager _instance;
        private static readonly object threadLock = new object();
        private LocalizationManager() { _programSettings = IoC.Get<IProgramSettings>(); }
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                    lock (threadLock)
                    {
                        _instance = new LocalizationManager();
                    }
                return _instance;
            }
        }
        #endregion
    }

    public class LocCulture : PropertyChangedBase
    {
        public string Name { get { return Culture.NativeName.Substring(0, 1).ToUpper(Culture) + Culture.NativeName.Substring(1, Culture.NativeName.Length - 1).ToLower(Culture); } }
        public CultureInfo Culture { get; set; }
        public string CultureString { get { return Culture.TwoLetterISOLanguageName; } }

        public override string ToString()
        {
            return Name;
        }

        #region Object overrides

        public override bool Equals(object obj)
        {
            var cul = obj as LocCulture;
            if (cul == null)
                return false;
            return Culture.Equals(cul.Culture);
        }

        public override int GetHashCode()
        {
            return Culture.GetHashCode();
        }

        #endregion Object overrides
    }

}
