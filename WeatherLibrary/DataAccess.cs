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

        string[] CountryIdList = { "4740157", "4310411", "564723", "564487", "6446507", "3201984", "524901" };// Просто рандомные id городов для начального списка

        string jsonResultText;
        JToken townNameToken;
        JToken pressureToken;
        JToken windDirectionToken;
        JToken windVelocityToken;
        JToken humidityToken;
        JToken sunriseToken;
        JToken sunsetToken;

        HttpClient httpClient;

        public ObservableCollection<Weather> GetCurrentWeather()// Загрузка первоначального списка городов
        {
            ObservableCollection<Weather> weatherList = new ObservableCollection<Weather>();
            httpClient = new HttpClient();

            for (int i = 0; i < CountryIdList.Length; i++)
            {
                Weather weather = GetInformationFromWeb("?id=" + CountryIdList[i]);
                weatherList.Add(weather);
            }

            return weatherList;
        }

        public Weather GetCurrentWeather(string counrtyName)//Используется при поиске по названию страны
        {
            httpClient = new HttpClient();
            Weather weather = GetInformationFromWeb("?q=" + counrtyName);
            return weather;
        }

        public Weather GetInformationFromWeb(string path) //Запрос на получение погоды за сегодня и сейчас
        {
            Weather weather = new Weather();

                using (var request = new HttpRequestMessage(new HttpMethod("POST"), BASE_ADDRESS + "weather" + path + APPID)) //Получение погоды на сейчас
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

                            weather.TownName = townNameToken.ToString();
                            weather.CurrentWeather.Pressure = (int)((double)(pressureToken) / MMHG);
                            weather.CurrentWeather.WindDirection = ConvertWindDirection(windDirectionToken);
                            weather.CurrentWeather.WindVelocity = (double)(windVelocityToken);
                            weather.CurrentWeather.Humidity = (int)(humidityToken);
                            weather.CurrentWeather.Sunrise = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunriseToken.ToString()));
                            weather.CurrentWeather.Sunset = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunsetToken.ToString()));

                        }
                            catch (Exception ex)
                            {
                                Log.Error("Не удалось извлечь данные " + ex.ToString());
                                return null;
                            }


                        }

                        else
                        {
                            Log.Error("Не удалось подключиться");
                            return null;
                        }

                    }
                }

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), BASE_ADDRESS + "forecast" + path + APPID+ "&cnt=1")) //cnt=1 это загрузка погоды на след. день
            {

                using (HttpResponseMessage response = httpClient.SendAsync(request).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {

                        try
                        {
                            jsonResultText = response.Content.ReadAsStringAsync().Result;
                            JObject jsonResult = JObject.Parse(jsonResultText);

                            IList<JToken> results = jsonResult["list"].Children().ToList();

                            pressureToken = results[0]["main"]["pressure"];
                            windDirectionToken = results[0]["wind"]["deg"];
                            windVelocityToken = results[0]["wind"]["speed"];
                            humidityToken = results[0]["main"]["humidity"];
                            sunriseToken = jsonResult["city"]["sunrise"];
                            sunsetToken = jsonResult["city"]["sunset"];

                            weather.TownName = townNameToken.ToString();
                            weather.WeatherToday.Pressure = (int)((double)(pressureToken) / MMHG);
                            weather.WeatherToday.WindDirection = ConvertWindDirection(windDirectionToken);
                            weather.WeatherToday.WindVelocity = (double)(windVelocityToken);
                            weather.WeatherToday.Humidity = (int)(humidityToken);
                            weather.WeatherToday.Sunrise = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunriseToken.ToString()));
                            weather.WeatherToday.Sunset = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunsetToken.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Log.Error("Не удалось извлечь данные " + ex.ToString());
                            return null;
                        }


                    }

                    else
                    {
                        Log.Error("Не удалось подключиться");
                        return null;
                    }

                }
            }

            return weather;
        }


        private string ConvertWindDirection(JToken windDirectionToken) //Конвертация градусов в направление ветра
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
