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
        public int Pressure { get; set; }


        public string WindDirection { get; set; }//TODO сделать перевод из градусов в слова
        public double WindVelocity { get; set; }
        public int Humidity { get; set; }
        public DateTime Sunset { get; set; }
        public DateTime Sunrise { get; set; }
    }
}
