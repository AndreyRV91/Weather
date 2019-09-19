using System.Collections.ObjectModel;
using WeatherLibrary.Models;

namespace WeatherLibrary
{
    public interface IDataAccess
    {
        ObservableCollection<Weather> GetCurrentWeather();
        Weather GetCurrentWeather(string townName);
    }
}