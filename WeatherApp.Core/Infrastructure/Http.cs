using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherApp.Core.Infrastructure
{
    public static class Http
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static async Task<HttpResponseMessage> GetAsync(string request)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = await _client.GetAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ee)
            {
                _logger.Error(ee, "Response error");
                throw;
            }

            return response;
        }
    }
}
