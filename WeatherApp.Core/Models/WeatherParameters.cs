using System;

namespace WeatherApp.Core.Models
{
    [Serializable]
    public class WeatherParameters
    {
        public int Pressure { get; set; }
        public int Temperature { get; set; }
        public double WindDirection { get; set; }
        public double WindVelocity { get; set; }
        public int Humidity { get; set; }
        public DateTime Sunset { get; set; }
        public DateTime Sunrise { get; set; }
    }
}
