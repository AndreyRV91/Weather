using Caliburn.Micro;
using System.Threading.Tasks;
using WeatherLibrary.Models;

namespace WeatherLibrary
{
    public interface IDataAccess
    {
        Task<BindableCollection<WeatherBase>> GetCurrentWeather();
        Task<WeatherBase >GetCurrentWeather(string townName);
    }
}