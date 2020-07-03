using System.Collections.Generic;

namespace WeatherApp.Core.Infrastructure
{
    public interface ITownsRepository
    {
        List<string> LoadTownsList();
        void SaveTownsList(IEnumerable<string> townsList);
    }
}