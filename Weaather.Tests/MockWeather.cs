using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherLibrary;
using WeatherLibrary.Models;

namespace Weather.Tests
{
    public class MockWeather : IDataAccess
    {
        ObservableCollection<WeatherLibrary.Models.Weather> mockWeatherList;
        WeatherLibrary.Models.Weather MockWeatherInMoscow;
        WeatherLibrary.Models.Weather MockWeatherInTula;

        public MockWeather()
        {
            MockWeatherInMoscow = new WeatherLibrary.Models.Weather();
            MockWeatherInTula = new WeatherLibrary.Models.Weather();

            mockWeatherList = new ObservableCollection<WeatherLibrary.Models.Weather>();

            MockWeatherInMoscow.TownName = "Moscow";
            MockWeatherInMoscow.CurrentWeather.Humidity = 50;
            MockWeatherInMoscow.CurrentWeather.Pressure = 764;
            MockWeatherInMoscow.CurrentWeather.WindDirection = "Северный";
            MockWeatherInMoscow.CurrentWeather.WindVelocity = 1;
            MockWeatherInMoscow.CurrentWeather.Sunrise = Convert.ToDateTime("15.12.2019 06:00:53");
            MockWeatherInMoscow.CurrentWeather.Sunset = Convert.ToDateTime("15.11.2019 22:00:55");
            MockWeatherInMoscow.WeatherToday.Humidity = 44;
            MockWeatherInMoscow.WeatherToday.Pressure = 800;
            MockWeatherInMoscow.WeatherToday.WindDirection = "Южный";
            MockWeatherInMoscow.WeatherToday.WindVelocity = 3;
            MockWeatherInMoscow.WeatherToday.Sunrise = Convert.ToDateTime("15.12.2019 06:01:46");
            MockWeatherInMoscow.WeatherToday.Sunset = Convert.ToDateTime("15.11.2019 22:01:44");

            MockWeatherInTula.TownName = "Tula";
            MockWeatherInTula.CurrentWeather.Humidity = 44;
            MockWeatherInTula.CurrentWeather.Pressure = 777;
            MockWeatherInTula.CurrentWeather.WindDirection = "Северный";
            MockWeatherInTula.CurrentWeather.WindVelocity = 2;
            MockWeatherInTula.CurrentWeather.Sunrise = Convert.ToDateTime("15.12.2019 06:00:53");
            MockWeatherInTula.CurrentWeather.Sunset = Convert.ToDateTime("15.11.2019 22:00:55");
            MockWeatherInTula.WeatherToday.Humidity = 33;
            MockWeatherInTula.WeatherToday.Pressure = 766;
            MockWeatherInTula.WeatherToday.WindDirection = "Южный";
            MockWeatherInTula.WeatherToday.WindVelocity = 3;
            MockWeatherInTula.WeatherToday.Sunrise = Convert.ToDateTime("15.12.2019 06:01:46");
            MockWeatherInTula.WeatherToday.Sunset = Convert.ToDateTime("15.11.2019 22:01:44");

            mockWeatherList.Add(MockWeatherInMoscow);
            mockWeatherList.Add(MockWeatherInTula);
        }

        public ObservableCollection<WeatherLibrary.Models.Weather> GetCurrentWeather()
        {
            return mockWeatherList;
        }

        public WeatherLibrary.Models.Weather GetCurrentWeather(string townName)
        {
            return mockWeatherList.FirstOrDefault(n => n.TownName == townName);
        }
    }
}
