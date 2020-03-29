using System.Collections.ObjectModel;
using WeatherLibrary.Models;

namespace WeatherLibrary
{
    public interface IDataAccess
    {
        ObservableCollection<WeatherBase> GetCurrentWeather();
        WeatherBase GetCurrentWeather(string townName);
    }
}