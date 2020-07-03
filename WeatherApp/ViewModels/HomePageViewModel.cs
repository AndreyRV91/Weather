using Caliburn.Micro;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Weather.Resources.Localizations;
using WeatherApp.Contracts;
using WeatherApp.Core;
using WeatherApp.Core.Infrastructure;
using WeatherApp.Core.Models;

namespace WeatherApp.ViewModels
{
    public class HomePageViewModel : Screen, IMenuPage
    {
        private readonly IDataAccess _dataAccess;
        private readonly IEventAggregator _eventAggregator;
        private readonly ITownsRepository _townsRepository;

        private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private bool IsInited;

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

        public IEnumerable<string> townsList 
        { 
            get 
            {
                foreach (var item in WeatherList)
                {
                    yield return item.TownName;
                }
            }
        }

        public bool IsStretchedMenu { get { return _isStretchedMenu; } set { Set(ref _isStretchedMenu, value); } }
        private bool _isStretchedMenu;

        public bool IsBusy { get { return _isBusy; } set { Set(ref _isBusy, value); } }
        private bool _isBusy;

        #endregion

        public HomePageViewModel(IDataAccess dataAccess, IEventAggregator eventAggregator, ITownsRepository townsRepository)
        {
            _dataAccess = dataAccess;
            _eventAggregator = eventAggregator;
            _townsRepository = townsRepository;
        }

        protected override async void OnActivate()
        {
            _eventAggregator.Subscribe(this);

            await Init().ConfigureAwait(false);

            IsInited = true;

            base.OnActivate();
        }

        private async Task Init()
        {
            WeatherList = new BindableCollection<WeatherBase>();

            await UpdateTownList();
        }

        public async Task UpdateTownList()
        {
            var list = new List<WeatherBase>();
            IEnumerable<string> towns;

            if(IsInited)
            {
                towns = townsList;
            }
            else
            {
                towns = _townsRepository.LoadTownsList();
            }

            try
            {
                IsBusy = true;
                list = await _dataAccess.GetWeatherList(towns).ConfigureAwait(false);
                IsBusy = false;
            }
            catch (IOException ex)
            {
                MessageBox.Show(SettingsRes.text_ErrorWhileRetrievingData);
                _logger.Error(ex.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            WeatherList = new BindableCollection<WeatherBase>(list);

            if (WeatherList.Any())
            {
                if(!IsInited)
                {
                    SelectedTown = WeatherList.FirstOrDefault();
                }
                UpdateWeather(SelectedTown);
            }
            else
            {
                MessageBox.Show(SettingsRes.text_ErrorWhileRetrievingData);
            }
        }

        public void UpdateWeather(WeatherBase selectedTown)
        {
            if (WeatherList == null || selectedTown == null || WeatherList.Count == 0)
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
                _logger.Error(ex.ToString());
            }
        }

        public async Task Search(string searchTownName)
        {
            if (WeatherList == null || SelectedTown == null)
            {
                return;
            }

            var searchResult = await _dataAccess.SearchTownWeather(searchTownName).ConfigureAwait(false);

            if (searchResult != null)
            {
                var townFromList = WeatherList.FirstOrDefault(n => n.TownName == searchResult.TownName);

                if (townFromList == null)
                {
                    WeatherList.Add(searchResult);
                    
                    _townsRepository.SaveTownsList(townsList);

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

        public void RemoveTown()
        {
            WeatherList.Remove(SelectedTown);
            SelectedTown = WeatherList.FirstOrDefault();
            UpdateWeather(SelectedTown);

            _townsRepository.SaveTownsList(townsList);
        }

        protected override void OnDeactivate(bool close)
        {
            IsInited = false;
            _townsRepository.SaveTownsList(townsList);
            WeatherList?.Clear();
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

    }
}
