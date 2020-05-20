using Caliburn.Micro;
using NLog.Fluent;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeatherApp.Contracts;
using WeatherApp.Messages;
using WeatherApp.Models.ProgramSettings;
using WeatherApp.Views;
using WeatherLibrary;
using WeatherLibrary.Models;
using static WeatherApp.Models.Enums;

namespace WeatherApp.ViewModels
{
    public class MainWindowViewModel : Conductor<IMenuPage>,
                                       IHandle<ChangeTheme>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgramSettings _programSettings;

        #region Properties

        public bool IsStretchedMenu { get { return _isStretchedMenu; } set { Set(ref _isStretchedMenu, value); } }
        private bool _isStretchedMenu;

        public bool IsBusy { get { return _isBusy; } set { Set(ref _isBusy, value); } }
        private bool _isBusy;

        public readonly Uri LightTheme = new Uri("pack://application:,,,/Weather.Resources;component/Styles/Themes/LightTheme.xaml");
        public readonly Uri DarkTheme = new Uri("pack://application:,,,/Weather.Resources;component/Styles/Themes/DarkTheme.xaml");

        #endregion

        public MainWindowViewModel(IDataAccess DataAccess, IEventAggregator eventAggregator, IProgramSettings programSettings)
        {
            _programSettings = programSettings;
            _eventAggregator = eventAggregator;

            SetTheme(_programSettings.Theme);
            OpenHomePage();
        }

        protected override void OnActivate()
        {
            _eventAggregator.Subscribe(this);

            base.OnActivate();
        }

        public void SlideMenu()
        {
            IsStretchedMenu = !IsStretchedMenu;
        }

        public void ChangeTheme()
        {
            if (_programSettings.Theme == 1) _programSettings.Theme = 2;
            else _programSettings.Theme = 1;
            _programSettings.SaveSettings();

            _eventAggregator.PublishOnUIThread(new ChangeTheme() { Theme = _programSettings.Theme });
        }

        public void OpenSettings()
        {
            var vm = IoC.Get<SettingsViewModel>();
            ActivateItem(vm);
        }

        public void OpenHomePage()
        {
            var vm = IoC.Get<HomePageViewModel>();
            ActivateItem(vm);
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        public async Task Search()
        {
            var vm = ActiveItem as HomePageViewModel;
            if (vm != null)
            {
                await vm.Search();
            }
        }        
        
        public async Task UpdateTownList()
        {
            var vm = ActiveItem as HomePageViewModel;
            if (vm != null)
            {
                await vm.UpdateTownList();
            }
        }

        private void SetTheme(int theme)
        {
            var ThemeResourceDictionary = new ResourceDictionary();
            var resources = Application.Current.Resources;
            Uri oldTheme = null;

            switch (_programSettings.Theme)
            {
                case (int)ThemeEnum.LightTheme:
                    ThemeResourceDictionary.Source = LightTheme;
                    oldTheme = DarkTheme;
                    break;
                case (int)ThemeEnum.DarkTheme:
                    ThemeResourceDictionary.Source = DarkTheme;
                    oldTheme = LightTheme;
                    break;
            }

            var oldThemeResource = resources.MergedDictionaries.Where(x => x.Source != null).FirstOrDefault(d => d.Source.ToString().ToUpper() == oldTheme.ToString().ToUpper());

            resources.MergedDictionaries.Add(ThemeResourceDictionary);
            if (oldThemeResource != null)
            {
                resources.MergedDictionaries.Remove(oldThemeResource);
            }
        }

        public void Handle(ChangeTheme message)
        {
            SetTheme(message.Theme);
            NotifyOfPropertyChange(() => SettingsIco);
            NotifyOfPropertyChange(() => MenuIco);
            NotifyOfPropertyChange(() => HomeIco);
            NotifyOfPropertyChange(() => SunMoonIco);
            NotifyOfPropertyChange(() => RefreshIco);
        }

        #region Images
        public string LupeIco => "/Weather.Resources;component/Images/Buttons/lupe.png";

        public string SettingsIco
        {
            get
            {
                switch (_programSettings.Theme)
                {
                    case (int)ThemeEnum.LightTheme:
                        return "/Weather.Resources;component/Images/Buttons/settings_light.png";
                    case (int)ThemeEnum.DarkTheme:
                        return "/Weather.Resources;component/Images/Buttons/settings_dark.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/settings_light.png";
                }
            }
        }

        public string RefreshIco
        {
            get
            {
                switch (_programSettings.Theme)
                {
                    case (int)ThemeEnum.LightTheme:
                        return "/Weather.Resources;component/Images/Buttons/refresh_light.png";
                    case (int)ThemeEnum.DarkTheme:
                        return "/Weather.Resources;component/Images/Buttons/refresh_dark.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/refresh_light.png";
                }
            }
        }

        public string MenuIco
        {
            get
            {
                switch (_programSettings.Theme)
                {
                    case (int)ThemeEnum.LightTheme:
                        return "/Weather.Resources;component/Images/Buttons/menu_light.png";
                    case (int)ThemeEnum.DarkTheme:
                        return "/Weather.Resources;component/Images/Buttons/menu_dark.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/menu_light.png";
                }
            }
        }

        public string HomeIco
        {
            get
            {
                switch (_programSettings.Theme)
                {
                    case (int)ThemeEnum.LightTheme:
                        return "/Weather.Resources;component/Images/Buttons/home_light.png";
                    case (int)ThemeEnum.DarkTheme:
                        return "/Weather.Resources;component/Images/Buttons/home_dark.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/home_light.png";
                }
            }
        }

        public string SunMoonIco
        {
            get
            {
                switch (_programSettings.Theme)
                {
                    case (int)ThemeEnum.LightTheme:
                        return "/Weather.Resources;component/Images/Buttons/sun.png";
                    case (int)ThemeEnum.DarkTheme:
                        return "/Weather.Resources;component/Images/Buttons/moon.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/sun.png";
                }
            }
        }

        #endregion

    }
}
