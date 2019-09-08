using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherLibrary.Models
{
    public class WeatherParameters
    {
        public int Pressure { get; set; }
        public string WindDirection { get; set; }
        public double WindVelocity { get; set; }
        public int Humidity { get; set; }
        public DateTime Sunset { get; set; }
        public DateTime Sunrise { get; set; }
    }
}
