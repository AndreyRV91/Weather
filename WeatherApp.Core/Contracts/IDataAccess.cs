using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Core.Models;

namespace WeatherApp.Core
{
    public interface IDataAccess
    {
        Task<List<WeatherBase>> GetWeatherList(IEnumerable<string> countriesList);
        Task<WeatherBase>SearchTownWeather(string townName);
    }
}