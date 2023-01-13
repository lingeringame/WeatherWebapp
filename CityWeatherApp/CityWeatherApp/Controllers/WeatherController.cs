using Microsoft.AspNetCore.Mvc;
using CityWeatherApp.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace CityWeatherApp.Controllers
{
    public class WeatherController : Controller
    {
        public IHttpClientFactory _httpClientFactory { get; }
        private string _apiKey = "54f815b14012b086d47900711b5953ab";

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //use searchTerm viewmodel for validation
        public async Task<IActionResult> GetCities(string searchTerm)
        {
            var httpClient = _httpClientFactory.CreateClient("geolocation");
            int limit = 10;
            var URL = $"?q={searchTerm}&limit={limit}&appid={_apiKey}";

            var response = await httpClient.GetAsync(URL);
            var results = await response.Content.ReadAsStringAsync();
            List<GeoLocation> locations = JsonConvert.DeserializeObject<List<GeoLocation>>(results);
            return View(locations);
        }

        public async Task<IActionResult> GetWeather(string lat, string lon)
        {
            var httpClient = _httpClientFactory.CreateClient("currWeather");
            var URL = $"?lat={lat}&lon={lon}&appid={_apiKey}";
            
            var response = await httpClient.GetAsync(URL);
            var results = await response.Content.ReadAsStringAsync();
            CurrentWeather currentWeather = JsonConvert.DeserializeObject<CurrentWeather>(results);

            currentWeather.main.temp = (float)Math.Round(((currentWeather.main.temp - 273.15) * 9 / 5 + 32), 2);
            currentWeather.weather[0].iconUrl = $"http://openweathermap.org/img/wn/{currentWeather.weather[0].icon}.png";
            return View(currentWeather);
        }
    }
}
