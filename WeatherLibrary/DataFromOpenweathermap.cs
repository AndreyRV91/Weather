using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.Core;
using WeatherApp.Core.Infrastructure;
using WeatherApp.Core.Models;

namespace WeatherLibrary
{
    public class DataFromOpenweathermap : IDataAccess
    {
        const string BASE_ADDRESS = "https://api.openweathermap.org/data/2.5/";
        const string APPID = "8afa526a19c42cc1d31ad0688de135f7";

        const double MMHG = 1.333;

        string jsonResultText;
        JToken townNameToken;
        JToken temperatureToken;
        JToken pressureToken;
        JToken windDirectionToken;
        JToken windVelocityToken;
        JToken humidityToken;
        JToken sunriseToken;
        JToken sunsetToken;

        private enum CNT { tomorrowWeather=2 }

        private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<List<WeatherBase>> GetWeatherList(IEnumerable<string> townsList) //Using for update weather
        {
            var weatherList = new List<WeatherBase>();
            WeatherBase weather;

            try
            {
                foreach (var town in townsList)
                {
                    weather = await SearchTownWeather(town).ConfigureAwait(false); //there is a restriction of api.openweathermap service for request for several cities at once

                    if(weather == null)
                    {
                        return new List<WeatherBase>();
                    }
                    else
                    {
                        weatherList.Add(weather);
                    }
                }
            }
            catch
            {
                return new List<WeatherBase>();
            }

            return weatherList;
        }

        public async Task<WeatherBase> SearchTownWeather(string townName)//Using for town name search
        {
            WeatherBase weather;

            try
            {
                weather = await GetInformationFromWeb($"?q={townName}").ConfigureAwait(false);
            }
            catch
            {
                return null;
            }

            return weather;
        }

        private async Task<WeatherBase> GetInformationFromWeb(string path) //request for today's weather forecast
        {
            WeatherBase weather = new WeatherBase();
            string lng = LocalizationManager.CultureName;
            HttpResponseMessage response = new HttpResponseMessage();
            JObject jsonResult = new JObject();

            try
            {
                response = await Http.GetAsync($"{BASE_ADDRESS}weather{path}&appid={APPID}&units=metric&lang={lng}").ConfigureAwait(false);

                jsonResultText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                jsonResult = JObject.Parse(jsonResultText);
                townNameToken = jsonResult["name"];
                temperatureToken = jsonResult["main"]["temp"];
                pressureToken = jsonResult["main"]["pressure"];
                windDirectionToken = jsonResult["wind"]["deg"];
                windVelocityToken = jsonResult["wind"]["speed"];
                humidityToken = jsonResult["main"]["humidity"];
                sunriseToken = jsonResult["sys"]["sunrise"];
                sunsetToken = jsonResult["sys"]["sunset"];

                weather.TownName = townNameToken.ToString();
                weather.CurrentWeather.Pressure = (int)((double)(pressureToken) / MMHG);
                weather.CurrentWeather.Temperature = (int)(temperatureToken);
                weather.CurrentWeather.WindDirection = windDirectionToken != null? (double)(windDirectionToken): 0 ;
                weather.CurrentWeather.WindVelocity = (double)(windVelocityToken);
                weather.CurrentWeather.Humidity = (int)(humidityToken);
                weather.CurrentWeather.Sunrise = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunriseToken.ToString())).ToLocalTime();
                weather.CurrentWeather.Sunset = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunsetToken.ToString())).ToLocalTime();


                response = await Http.GetAsync($"{BASE_ADDRESS}forecast{path}&appid={APPID}&cnt={(int)CNT.tomorrowWeather}&units=metric&lang={lng}").ConfigureAwait(false);

                jsonResultText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                jsonResult = JObject.Parse(jsonResultText);

                IList<JToken> results = jsonResult["list"].Children().ToList();

                pressureToken = results[1]["main"]["pressure"];
                temperatureToken = results[1]["main"]["temp"];
                windDirectionToken = results[1]["wind"]["deg"];
                windVelocityToken = results[1]["wind"]["speed"];
                humidityToken = results[1]["main"]["humidity"];
                sunriseToken = jsonResult["city"]["sunrise"];
                sunsetToken = jsonResult["city"]["sunset"];

                weather.TownName = townNameToken.ToString();
                weather.WeatherToday.Pressure = (int)((double)(pressureToken) / MMHG);
                weather.WeatherToday.Temperature = (int)(temperatureToken);
                weather.WeatherToday.WindDirection = windDirectionToken != null ? (double)(windDirectionToken) : 0;
                weather.WeatherToday.WindVelocity = (double)(windVelocityToken);
                weather.WeatherToday.Humidity = (int)(humidityToken);
                weather.WeatherToday.Sunrise = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunriseToken.ToString())).ToLocalTime();
                weather.WeatherToday.Sunset = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(Convert.ToDouble(sunsetToken.ToString())).ToLocalTime();

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), "Error while converting data ");
                throw;
            }

            return weather;

        }
    }
}
