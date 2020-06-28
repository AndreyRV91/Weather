using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherLibrary;
using WeatherLibrary.Models;

namespace Weather.Test
{

    public class MockWeather : IDataAccess
    {
        BindableCollection<WeatherBase> mockWeatherList;
        WeatherBase MockWeatherInMoscow;
        WeatherBase MockWeatherInTula;

        public MockWeather()
        {
            MockWeatherInMoscow = new WeatherBase();
            MockWeatherInTula = new WeatherBase();

            mockWeatherList = new BindableCollection<WeatherBase>();

            MockWeatherInMoscow.TownName = "Moscow";
            MockWeatherInMoscow.CurrentWeather.Humidity = 50;
            MockWeatherInMoscow.CurrentWeather.Pressure = 764;
            MockWeatherInMoscow.CurrentWeather.WindDirection = 123;
            MockWeatherInMoscow.CurrentWeather.WindVelocity = 1;
            MockWeatherInMoscow.CurrentWeather.Sunrise = Convert.ToDateTime("15.12.2019 06:00:53");
            MockWeatherInMoscow.CurrentWeather.Sunset = Convert.ToDateTime("15.11.2019 22:00:55");
            MockWeatherInMoscow.WeatherToday.Humidity = 44;
            MockWeatherInMoscow.WeatherToday.Pressure = 800;
            MockWeatherInMoscow.WeatherToday.WindDirection = 90;
            MockWeatherInMoscow.WeatherToday.WindVelocity = 3;
            MockWeatherInMoscow.WeatherToday.Sunrise = Convert.ToDateTime("15.12.2019 06:01:46");
            MockWeatherInMoscow.WeatherToday.Sunset = Convert.ToDateTime("15.11.2019 22:01:44");

            MockWeatherInTula.TownName = "Tula";
            MockWeatherInTula.CurrentWeather.Humidity = 44;
            MockWeatherInTula.CurrentWeather.Pressure = 777;
            MockWeatherInTula.CurrentWeather.WindDirection = 124;
            MockWeatherInTula.CurrentWeather.WindVelocity = 2;
            MockWeatherInTula.CurrentWeather.Sunrise = Convert.ToDateTime("15.12.2019 06:00:53");
            MockWeatherInTula.CurrentWeather.Sunset = Convert.ToDateTime("15.11.2019 22:00:55");
            MockWeatherInTula.WeatherToday.Humidity = 33;
            MockWeatherInTula.WeatherToday.Pressure = 766;
            MockWeatherInTula.WeatherToday.WindDirection = 90;
            MockWeatherInTula.WeatherToday.WindVelocity = 3;
            MockWeatherInTula.WeatherToday.Sunrise = Convert.ToDateTime("15.12.2019 06:01:46");
            MockWeatherInTula.WeatherToday.Sunset = Convert.ToDateTime("15.11.2019 22:01:44");

            mockWeatherList.Add(MockWeatherInMoscow);
            mockWeatherList.Add(MockWeatherInTula);
        }

        public async Task<BindableCollection<WeatherBase>> GetCurrentWeather()
        {
            return mockWeatherList;
        }

        public async Task<WeatherBase> GetCurrentWeather(string townName)
        {
            return mockWeatherList.FirstOrDefault(n => n.TownName == townName);
        }
    }
}
