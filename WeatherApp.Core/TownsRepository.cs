using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WeatherApp.Core.Infrastructure;

namespace WeatherApp.Core
{
    public class TownsRepository : ITownsRepository
    {
        private List<string> _defaultTowns = new List<string>() { "Moscow", "Dubai", "London", "Washington" };

        private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void SaveTownsList(IEnumerable<string> towns)
        {
            var formatter = new XmlSerializer(typeof(List<string>));

            List<string> townsList = new List<string>();
            foreach (var item in towns)
            {
                townsList.Add(item);
            }

            try
            {
                using (FileStream fs = new FileStream("TownList.xml", FileMode.Create))
                {
                    formatter.Serialize(fs, townsList);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        public List<string> LoadTownsList()
        {
            if (!File.Exists("TownList.xml"))
            {
                return _defaultTowns;
            }
            else
            {
                var formatter = new XmlSerializer(typeof(List<string>));

                try
                {
                    using (FileStream fs = new FileStream("TownList.xml", FileMode.OpenOrCreate))
                    {
                        var towns = (List<string>)formatter.Deserialize(fs);
                        if (towns.Any())
                        {
                            return towns;
                        }
                        else
                        {
                            return _defaultTowns;
                        }
                    }
                }
                catch
                {
                    File.Delete("TownList.xml");
                    throw;
                }
            }
        }
    }
}
