using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium_Practice
{
    [TestFixture]
    public class AccuWeatherTests
    {
        private IWebDriver driver;
        private const string AccuWeatherUrl = "https://www.accuweather.com/en/il/tel-aviv/215854/weather-forecast/215854";
        private const string WeatherApiUrl = "https://api.openweathermap.org/data/2.5/weather";
        private const string ApiKey = "0580fde115a169a62e4ee9cc36585ab8";
        private const string City_ID = "293396";//"Tel Aviv";

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            //options.AddArgument("--headless");
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Window.Maximize();
        }

        [Test]
        public async Task CompareWeatherDataInTelAviv()
        {
            var apiWeatherData = await FetchWeatherDataFromApi(City_ID);

            driver.Navigate().GoToUrl(AccuWeatherUrl);
            var websiteWeatherData = FetchWeatherDataFromWebsite();

            double temperatureDifference = Math.Abs(apiWeatherData.Temperature - websiteWeatherData.Temperature);

            Assert.LessOrEqual(temperatureDifference, 10, "Temperature difference exceeds the acceptable range.");
        }

        private async Task<WeatherData> FetchWeatherDataFromApi(string city_id)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{WeatherApiUrl}?id={city_id}&appid={ApiKey}&units=metric";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject weatherJson = JObject.Parse(responseBody);

                return new WeatherData
                {
                    Temperature = (double)weatherJson["main"]["temp"],
                };
            }
        }

        private WeatherData FetchWeatherDataFromWebsite()
        {
            string degree = driver.FindElement(By.CssSelector(".temp")).Text.Replace("°", "").Replace("C","").Trim();
            double temperature = Convert.ToDouble(degree);

            return new WeatherData
            {
                Temperature = temperature,
            };
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        public class WeatherData
        {
            public double Temperature { get; set; }
        }
    }
}
