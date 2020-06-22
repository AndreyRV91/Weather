using Caliburn.Micro;
using NLog.Fluent;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Weather.Resources.Localizations;
using WeatherApp.Contracts;
using WeatherApp.Core.Models.ProgramSettings;
using WeatherApp.Properties;
using WeatherLibrary;
using WeatherLibrary.Models;

namespace WeatherApp.ViewModels
{
    public class HomePageViewModel : Screen, IMenuPage
    {
        private readonly IDataAccess DataAccess;
        private readonly IEventAggregator _eventAggregator;

        #region Properties

        public string TownName { get { return _townName; } set { Set(ref _townName, value); } }
        private string _townName;

        public int Temperature { get { return _temperature; } set { Set(ref _temperature, value); } }
        private int _temperature;

        public WeatherBase SelectedTown { get { return _selectedTown; } set { Set(ref _selectedTown, value); } }
        private WeatherBase _selectedTown;

        public BindableCollection<WeatherBase> WeatherList { get { return _weatherList; } set { Set(ref _weatherList, value); } }
        private BindableCollection<WeatherBase> _weatherList;

        public WeatherParameters CurrentWeather { get { return _currentWeather; } set { Set(ref _currentWeather, value); } }
        private WeatherParameters _currentWeather;

        public WeatherParameters WeatherToday { get { return _weatherToday; } set { Set(ref _weatherToday, value); } }
        private WeatherParameters _weatherToday;

        public bool IsStretchedMenu { get { return _isStretchedMenu; } set { Set(ref _isStretchedMenu, value); } }
        private bool _isStretchedMenu;

        public bool IsBusy { get { return _isBusy; } set { Set(ref _isBusy, value); } }
        private bool _isBusy;

        #endregion

        public HomePageViewModel(IDataAccess DataAccess, IEventAggregator eventAggregator, IProgramSettings programSettings)
        {
            this.DataAccess = DataAccess;
            _eventAggregator = eventAggregator;
        }

        protected override async void OnActivate()
        {
            _eventAggregator.Subscribe(this);

            try
            {
                await UpdateTownList().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            base.OnActivate();
        }

        public async Task UpdateTownList()
        {
            WeatherList?.Clear();

            WeatherList = new BindableCollection<WeatherBase>();

            IsBusy = true;
            WeatherList = await DataAccess.GetCurrentWeather().ConfigureAwait(false);
            IsBusy = false;

            if (WeatherList != null && SelectedTown == null)
            {
                SelectedTown = WeatherList.FirstOrDefault();
            }

            if(WeatherList != null)
            {
                UpdateWeather(SelectedTown);
            }
            else
            {
                MessageBox.Show(SettingsRes.text_ErrorWhileRetrievingData);
            }
        }

        public void UpdateWeather(WeatherBase selectedTown)
        {
            if (WeatherList == null || SelectedTown == null || WeatherList.Count == 0)
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

        public async Task Search(string searchTownName)
        {
            if (WeatherList == null || SelectedTown == null)
            {
                return;
            }

            var searchResult = await DataAccess.GetCurrentWeather(searchTownName).ConfigureAwait(false);

            if (searchResult != null)
            {
                var townFromList = WeatherList.FirstOrDefault(n => n.TownName == searchResult.TownName);

                if (townFromList == null)
                {
                    WeatherList.Add(searchResult);
                    townFromList = WeatherList.FirstOrDefault(n => n.TownName == searchResult.TownName);
                }

                SelectedTown = townFromList;
                TownName = townFromList.TownName;
                CurrentWeather = townFromList.CurrentWeather;
                WeatherToday = townFromList.WeatherToday;
            }
            else
            {
                MessageBox.Show(SettingsRes.text_TownNotFound);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

    }
}
