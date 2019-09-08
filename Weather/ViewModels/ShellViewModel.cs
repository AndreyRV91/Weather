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

namespace Weather.ViewModels
{
    public class ShellViewModel : Screen
    {
        DataAccess DataAccess;
        WeatherLibrary.Models.Weather Weather;

        #region Properties
        ObservableCollection<WeatherLibrary.Models.Weather> _WeatherList;
        string _CurrentWeather;
        WeatherLibrary.Models.Weather _SelectedTown;
        string _TownName;

        public string TownName
        {
            get
            {
                return _TownName;
            }
            set
            {
                _TownName = value;
                NotifyOfPropertyChange("TownName");
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
                NotifyOfPropertyChange("SelectedTown");
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
                NotifyOfPropertyChange("WeatherList");
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
                NotifyOfPropertyChange("CurrentWeather");
            }
        }

#endregion


        public ShellViewModel()
        {

        }


        public void UpdateTownList()
        {
            if (WeatherList != null) { WeatherList = null; }

            DataAccess = new DataAccess();
            WeatherList = new ObservableCollection<WeatherLibrary.Models.Weather>();

           WeatherList = DataAccess.GetCurrentWeather();
           SelectedTown = WeatherList?.FirstOrDefault();

        }

        public void UpdateWeather()
        {
            if (WeatherList == null)
            {
                return;
            }

            Weather = SelectedTown;

            try
            {
                CurrentWeather = String.Format("Давление {0:f0} мм рт.ст. \n" +
               "Ветер: {1}, {2:f1} м/с ({3:f1} км/ч) \n" +
               "Влажность: {4:0}% \n" +
               "Восход: {5:t} Заход: {6:t}", Weather.Pressure, Weather.WindDirection, Weather.WindVelocity, Weather.WindVelocity * 1000 / 3600, Weather.Humidity, Weather.Sunrise, Weather.Sunset);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
           

        }

        public void Search()
        {
            if (WeatherList == null)
            {
                return;
            }

            var Searchresult = DataAccess.GetCurrentWeather(TownName);
            

            if (Searchresult != null) // Если поиск дал результаты
            {
                if (WeatherList.FirstOrDefault(n => n.TownName == Searchresult.TownName) != null) //Если уже есть такой город, то не добавляем в список городов, а просто переводим выделение
                {
                    SelectedTown = WeatherList.FirstOrDefault(n => n.TownName == Searchresult.TownName);
                }
                else
                {
                    WeatherList.Add(Searchresult);
                    SelectedTown = SelectedTown = WeatherList.FirstOrDefault(n => n.TownName == Searchresult.TownName);
                }

            }
            else
            {
                MessageBox.Show("Город не найден");
            }

        }


    }
}
