using System;

namespace WeatherApp.Core.Models
{
    [Serializable]
    public class WeatherBase
    {
        public string TownName { get; set; }
        public WeatherParameters CurrentWeather { get; set; }
        public WeatherParameters WeatherToday { get; set; }

        public WeatherBase()
        {
            CurrentWeather = new WeatherParameters();
            WeatherToday = new WeatherParameters();
        }
    }
}
