using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherApp.Core.Infrastructure
{
    public static class Http
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task<HttpResponseMessage> GetAsync(string request)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = await client.GetAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ee)
            {
                logger.Error(ee, "Response error");
                throw;
            }

            return response;
        }
    }
}
