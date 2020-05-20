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

        public HomePageViewModel(IDataAccess DataAccess, IEventAggregator eventAggregator, IProgramSettings programSettings)
        {
            this.DataAccess = DataAccess;
            _eventAggregator = eventAggregator;
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

            if (WeatherList.Any() && SelectedTown == null)
            {
                SelectedTown = WeatherList.FirstOrDefault();
            }
            UpdateWeather(SelectedTown);
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

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

    }
}
