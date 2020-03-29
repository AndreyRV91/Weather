using Caliburn.Micro;
using NLog.Fluent;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeatherApp.Messages;
using WeatherLibrary;
using WeatherLibrary.Models;

namespace WeatherApp.ViewModels
{
    public class MainWindowViewModel : Screen, IHandle<ChangeTheme>
    {
        private readonly IDataAccess DataAccess;
        private readonly IEventAggregator _eventAggregator;
        private int _theme;

        #region Properties

        public string TownName { get { return _townName; } set { Set(ref _townName, value); } }
        private string _townName;
        public int Temperature { get { return _temperature; } set { Set(ref _temperature, value); } }
        private int _temperature;

        public WeatherBase SelectedTown { get { return _selectedTown; } set { Set(ref _selectedTown, value); } }
        private WeatherBase _selectedTown;

        public ObservableCollection<WeatherBase> WeatherList { get { return _weatherList; } set { Set(ref _weatherList, value); } }
        private ObservableCollection<WeatherBase> _weatherList;

        public WeatherParameters CurrentWeather { get { return _currentWeather; } set { Set(ref _currentWeather, value); } }
        private WeatherParameters _currentWeather;

        public WeatherParameters WeatherToday { get { return _weatherToday; } set { Set(ref _weatherToday, value); } }
        private WeatherParameters _weatherToday;

        public bool IsStretchedMenu { get { return _isStretchedMenu; } set { Set(ref _isStretchedMenu, value); } }
        private bool _isStretchedMenu;

        public bool IsBusy { get { return _isBusy; } set { Set(ref _isBusy, value); } }
        private bool _isBusy;

        #endregion

        public MainWindowViewModel(IDataAccess DataAccess, IEventAggregator eventAggregator)
        {
            this.DataAccess = DataAccess;
            _eventAggregator = eventAggregator;

            _theme = 2;//TODO Upload
            SetTheme(_theme);
        }

        protected override async void OnActivate()
        {
            _eventAggregator.Subscribe(this);

            await UpdateTownList();

            base.OnActivate();
        }

        public async Task UpdateTownList()
        {
            WeatherList?.Clear();
            
            WeatherList = new ObservableCollection<WeatherBase>();

            IsBusy = true;
            WeatherList = await Task.Run(() => DataAccess.GetCurrentWeather());
            IsBusy = false;

            if (WeatherList.Any() && SelectedTown==null)
            {
                SelectedTown = WeatherList.FirstOrDefault();
            }
            UpdateWeather(SelectedTown);
        }

        public void UpdateWeather(WeatherBase selectedTown)
        {
            if (WeatherList == null || SelectedTown == null || WeatherList.Count==0)
            {
                return;
            }
            
            try
            {
                TownName = selectedTown.TownName;

                SelectedTown = selectedTown;
                CurrentWeather = SelectedTown.CurrentWeather;
                WeatherToday = selectedTown.WeatherToday;

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public async Task Search()
        {
            if (WeatherList == null || SelectedTown == null)
            {
                return;
            }
            
            var searchResult = await Task.Run(() => DataAccess.GetCurrentWeather(TownName));

            if (searchResult != null)
            {
                if (WeatherList.FirstOrDefault(n => n.TownName == TownName) != null) //если уже есть в списке
                {
                    SelectedTown = SelectedTown = WeatherList.FirstOrDefault(n => n.TownName == searchResult.TownName);
                }
                else //Если нет, то добавляем, но при условии, что города точно нет в нашем списке, т.к. внешний поиск может вестись на русском
                {
                    if (WeatherList.FirstOrDefault(n => n.TownName == searchResult.TownName) == null)
                    {
                        WeatherList.Add(searchResult);
                    }
                    SelectedTown = WeatherList.FirstOrDefault(n => n.TownName == searchResult.TownName);
                }

            }
            else //Если не нашли
            {
                MessageBox.Show("Город не найден");
            }
        }

        public void SlideMenu()
        {
            IsStretchedMenu = !IsStretchedMenu;
        }

        public void ChangeTheme()
        {
            if (_theme == 1) _theme = 2;
            else _theme = 1;

            _eventAggregator.PublishOnUIThread(new ChangeTheme() { Theme = _theme });
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        

        private void SetTheme(int theme)
        {
            var ThemeResourceDictionary = new ResourceDictionary();
            switch (_theme)
            {
                case 1:
                    ThemeResourceDictionary.Source = new Uri("pack://application:,,,/Weather.Resources;component/Styles/Themes/LightTheme.xaml");
                    break;
                case 2:
                    ThemeResourceDictionary.Source = new Uri("pack://application:,,,/Weather.Resources;component/Styles/Themes/DarkTheme.xaml");
                    break;
            }
            
            //TODO Remove previous key
            Application.Current.Resources.MergedDictionaries.Add(ThemeResourceDictionary);
        }

        public void Handle(ChangeTheme message)
        {
            SetTheme(message.Theme);
            NotifyOfPropertyChange(() => SettingsIco);
            NotifyOfPropertyChange(() => MenuIco);
            NotifyOfPropertyChange(() => HomeIco);
            NotifyOfPropertyChange(() => ListIco);
            NotifyOfPropertyChange(() => SunMoonIco);
        }

        #region Images
        public string RefreshIco=> "/Weather.Resources;component/Images/Buttons/refresh.png";
        public string LupeIco=> "/Weather.Resources;component/Images/Buttons/lupe.png";
        
        public string SettingsIco
        {
            get
            {
                switch (_theme)
                {
                    case 1:
                        return "/Weather.Resources;component/Images/Buttons/settings_light.png";
                    case 2:
                        return "/Weather.Resources;component/Images/Buttons/settings_dark.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/settings_light.png";
                }
            }
        }

        public string MenuIco
        {
            get
            {
                switch (_theme)
                {
                    case 1:
                        return "/Weather.Resources;component/Images/Buttons/menu_light.png";
                    case 2:
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
                switch (_theme)
                {
                    case 1:
                        return "/Weather.Resources;component/Images/Buttons/home_light.png";
                    case 2:
                        return "/Weather.Resources;component/Images/Buttons/home_dark.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/home_light.png";
                }
            }
        }

        public string ListIco
        {
            get
            {
                switch (_theme)
                {
                    case 1:
                        return "/Weather.Resources;component/Images/Buttons/list_light.png";
                    case 2:
                        return "/Weather.Resources;component/Images/Buttons/list_dark.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/list_light.png";
                }
            }
        }

        public string SunMoonIco
        {
            get
            {
                switch (_theme)
                {
                    case 1:
                        return "/Weather.Resources;component/Images/Buttons/sun.png";
                    case 2:
                        return "/Weather.Resources;component/Images/Buttons/moon.png";
                    default:
                        return "/Weather.Resources;component/Images/Buttons/sun.png";
                }
            }
        }

        #endregion

    }
}
