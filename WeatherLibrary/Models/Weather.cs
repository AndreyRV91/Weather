using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherLibrary.Models
{
    public class Weather
    {
        public string TownName { get; set; }
        public WeatherParameters CurrentWeather { get; set; }
        public WeatherParameters WeatherToday { get; set; }

        public Weather()
        {
            CurrentWeather = new WeatherParameters();
            WeatherToday = new WeatherParameters();
        }
    }
}
