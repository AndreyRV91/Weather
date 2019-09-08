using Newtonsoft.Json.Linq;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherLibrary.Models;

namespace WeatherLibrary
{


    public class DataAccess
    {
        const string BASE_ADDRESS = "https://api.openweathermap.org/data/2.5/";
        const string APPID = "&appid=8afa526a19c42cc1d31ad0688de135f7";

        const double MMHG = 1.333;

        string[] windDirections = { "Северный", "Северо-северо-восточный", "Северо-восточный", "Восток-северо-восточный", "Восточный",
            "Восток-юго-восточный", "Юго-восточный", "Юго-юго-восточный", "Южный", "Юго-юго-западный", "Юго-западный", "Запад-юго-западный",
            "Западный", "Запад-северо-западный", "Северо-западный", "Северо-северо-западный" };

        string[] CountryIdList = { "4740157", "4310411", "564723", "564487", "6446507", "3201984" };

        string jsonResultText;
        JToken townNameToken;
        JToken pressureToken;
        JToken windDirectionToken;
        JToken windVelocityToken;
        JToken humidityToken;
        JToken sunriseToken;
        JToken sunsetToken;


        public ObservableCollection<Weather> GetCurrentWeather()
        {
            ObservableCollection<Weather> weatherList = new ObservableCollection<Weather>();

            for (int i = 1; i < CountryIdList.Length; i++)
            {
                Weather weather = GetInformationFromWeb("weather?id=" + CountryIdList[i]);
                weatherList.Add(weather);
            }

            return weatherList;
        }

        public Weather GetCurrentWeather(string counrtyName)
        {
            Weather weather = GetInformationFromWeb("weather?q=" + counrtyName);
            return weather;
        }


        public Weather GetInformationFromWeb(string path)
        {
            Weather weather = new Weather();

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), BASE_ADDRESS + path + APPID))
                {

                    using (HttpResponseMessage response = httpClient.SendAsync(request).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {

                            try
                            {
                                jsonResultText = response.Content.ReadAsStringAsync().Result;
                                JObject jsonResult = JObject.Parse(jsonResultText);
                                townNameToken = jsonResult["name"];
                                pressureToken = jsonResult["main"]["pressure"];
                                windDirectionToken = jsonResult["wind"]["deg"];
                                windVelocityToken = jsonResult["wind"]["speed"];
                                humidityToken = jsonResult["main"]["humidity"];
                                sunriseToken = jsonResult["sys"]["sunrise"];
                                sunsetToken = jsonResult["sys"]["sunset"];

                            }
                            catch (Exception ex)
                            {
                                Log.Error("Не удалось извлечь данные " + ex.ToString());
                                return null;
                            }

                            weather.TownName = townNameToken.ToString();
                            weather.Pressure = (int)((double)(pressureToken) / MMHG);
                            weather.WindDirection = ConvertWindDirection(windDirectionToken);
                            weather.WindVelocity = (double)(windVelocityToken);
                            weather.Humidity = (int)(humidityToken);
                            weather.Sunrise = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunriseToken.ToString()));
                            weather.Sunset = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunsetToken.ToString()));

                        }

                        else
                        {
                            Log.Error("Не удалось подключиться");
                            return null;
                        }

                    }
                }
            }
            return weather;
        }


        private string ConvertWindDirection(JToken windDirectionToken)
        {
            if (windDirectionToken == null)
            {
                return "Не определено";
            }
            double windDirection = (double)(windDirectionToken);
            int windQuater = (int)((windDirection / 22.5) + 0.5);
            string result = windDirections[windQuater % 16];
            return (windDirections[windQuater % 16]);
        }
    }
}
