using Caliburn.Micro;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeatherLibrary;
using WeatherLibrary.Models;

namespace Weather.ViewModels
{
    public class ShellViewModel : Screen
    {
        IDataAccess DataAccess;
        WeatherLibrary.Models.Weather Weather;

        #region Свойства
        ObservableCollection<WeatherLibrary.Models.Weather> _WeatherList;
        string _CurrentWeather;
        WeatherLibrary.Models.Weather _SelectedTown;
        string _TownName;
        string _WeatherToday;

        public string WeatherToday
        {
            get
            {
                return _WeatherToday;
            }
            set
            {
                _WeatherToday = value;
                NotifyOfPropertyChange(() => WeatherToday);
            }
        }

        public string TownName
        {
            get
            {
                return _TownName;
            }
            set
            {
                _TownName = value;
            }
        }

        public WeatherLibrary.Models.Weather SelectedTown
        {
            get
            {
                return _SelectedTown;
            }
            set
            {
                _SelectedTown = value;
                NotifyOfPropertyChange(() => SelectedTown);
            }
        }

        public ObservableCollection<WeatherLibrary.Models.Weather> WeatherList
        {
            get
            {
                return _WeatherList;
            }
            set
            {
                _WeatherList = value;
                NotifyOfPropertyChange(() => WeatherList);
            }
        }

        public string CurrentWeather
        {
            get
            {
                return _CurrentWeather;
            }

            set
            {
                _CurrentWeather = value;
                NotifyOfPropertyChange(() => CurrentWeather);
            }
        }

#endregion


        public ShellViewModel(IDataAccess DataAccess)
        {
            this.DataAccess = DataAccess;
        }


        public async Task UpdateTownList()
        {
            if (WeatherList != null) { WeatherList = null; }


           WeatherList = new ObservableCollection<WeatherLibrary.Models.Weather>();

           WeatherList = await Task.Run(() => DataAccess.GetCurrentWeather());

            if(WeatherList.Any())
            {
                SelectedTown = WeatherList.FirstOrDefault();
            }


        }

        public void UpdateWeather()
        {
            if (WeatherList == null || SelectedTown == null)
            {
                return;
            }

            Weather = SelectedTown;

            try
            {
                CurrentWeather = String.Format("Давление {0:f0} мм рт.ст. \n" +
               "Ветер: {1}, {2:f1} м/с ({3:f1} км/ч) \n" +
               "Влажность: {4:0}% \n" +
               "Восход: {5:t} Заход: {6:t}", 
               Weather.CurrentWeather.Pressure, Weather.CurrentWeather.WindDirection,
               Weather.CurrentWeather.WindVelocity, Weather.CurrentWeather.WindVelocity * 1000 / 3600, 
               Weather.CurrentWeather.Humidity, Weather.CurrentWeather.Sunrise, Weather.CurrentWeather.Sunset);

                WeatherToday = String.Format("Давление {0:f0} мм рт.ст. \n" +
               "Ветер: {1}, {2:f1} м/с ({3:f1} км/ч) \n" +
               "Влажность: {4:0}% \n" +
               "Восход: {5:t} Заход: {6:t}", Weather.WeatherToday.Pressure, Weather.WeatherToday.WindDirection, 
               Weather.WeatherToday.WindVelocity, Weather.WeatherToday.WindVelocity * 1000 / 3600, 
               Weather.WeatherToday.Humidity, Weather.WeatherToday.Sunrise, Weather.WeatherToday.Sunset);

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


            var searchResult = await Task.Run(()=>DataAccess.GetCurrentWeather(TownName));

            if (searchResult != null)
            {
                if (WeatherList.FirstOrDefault(n=>n.TownName == TownName) != null) //если уже есть в списке
                {
                    SelectedTown = WeatherList.FirstOrDefault(n => n.TownName == searchResult.TownName);
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

    }
}
