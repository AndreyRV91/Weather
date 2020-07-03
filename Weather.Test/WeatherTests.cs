using Caliburn.Micro;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.Core;
using WeatherApp.Core.Infrastructure;
using WeatherApp.Core.Models.ProgramSettings;
using WeatherApp.ViewModels;
using WeatherLibrary;

namespace Weather.Test
{
    [TestFixture]
    public class WeatherTests
    {
        IProgramSettings _settings;
        IEventAggregator _eventAggregator;
        ITownsRepository _townsRepository;

        [SetUp]
        public void SetUp()
        {
            _settings = new ProgramSettings();
            _settings.SetDefault();
            LocalizationManager.Instance.SetCultureAtStartUp(_settings);

            _eventAggregator = new EventAggregator();


        }

        [Test]
        public async Task GetCurrentWeather_ShouldGiveInformation()
        {
            //Arrange
            IDataAccess DataFromOpenweathermap = new DataFromOpenweathermap();

            //Act
            var data = await DataFromOpenweathermap.GetWeatherList(new System.Collections.Generic.List<string>());


            //Assert
            foreach (var item in data)
            {
                Assert.NotNull(item.TownName);
                Assert.That(item.CurrentWeather.Humidity, Is.InRange(0, 100));
                Assert.That(item.CurrentWeather.Pressure, Is.InRange(0, 900));
                Assert.NotNull(item.CurrentWeather.WindDirection);
                Assert.That(item.CurrentWeather.WindVelocity, Is.InRange(0, 231));
                Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunrise);
                Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunset);

                Assert.That(item.WeatherToday.Humidity, Is.InRange(0, 100));
                Assert.That(item.WeatherToday.Pressure, Is.InRange(0, 900));
                Assert.NotNull(item.WeatherToday.WindDirection);
                Assert.That(item.WeatherToday.WindVelocity, Is.InRange(0, 231));
                Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunrise);
                Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunset);
            }

        }

        [Test]
        public async Task Search_ShouldReturnWeatherInTown()
        {
            //Arrange
            IDataAccess DataFromOpenweathermap = new DataFromOpenweathermap();

            //Act
            var item = await DataFromOpenweathermap.SearchTownWeather("Moscow");


            //Assert

            Assert.NotNull(item.TownName);
            Assert.That(item.CurrentWeather.Humidity, Is.InRange(0, 100));
            Assert.That(item.CurrentWeather.Pressure, Is.InRange(0, 900));
            Assert.NotNull(item.CurrentWeather.WindDirection);
            Assert.That(item.CurrentWeather.WindVelocity, Is.InRange(0, 231));
            Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunrise);
            Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunset);

            Assert.That(item.WeatherToday.Humidity, Is.InRange(0, 100));
            Assert.That(item.WeatherToday.Pressure, Is.InRange(0, 900));
            Assert.NotNull(item.WeatherToday.WindDirection);
            Assert.That(item.WeatherToday.WindVelocity, Is.InRange(0, 231));
            Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunrise);
            Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunset);

        }

        [Test]
        public async Task WeatherListIsNotEmpty()
        {
            //Arrange
            MockWeather mockWeather = new MockWeather();
            var homePageVM = new HomePageViewModel(mockWeather, _eventAggregator, _townsRepository);

            //Act
            await homePageVM.UpdateTownList();

            //Assert
            Assert.IsNotEmpty(homePageVM.WeatherList);
        }

        [Test]
        public async Task SelectedTown_ShouldBeMoscow()
        {
            //Arrange
            MockWeather mockWeather = new MockWeather();
            var homePageVM = new HomePageViewModel(mockWeather, _eventAggregator, _townsRepository);

            //Act
            await homePageVM.UpdateTownList();

            //Assert
            var townFromWeatherList = homePageVM.WeatherList.FirstOrDefault();
            Assert.AreEqual(townFromWeatherList.TownName, homePageVM.SelectedTown.TownName);
        }

        [Test]
        public async Task Search_ShouldBeTula()
        {
            //Arrange
            MockWeather mockWeather = new MockWeather();
            var homePageVM = new HomePageViewModel(mockWeather, _eventAggregator, _townsRepository);

            //Act
            await homePageVM.UpdateTownList();
            await homePageVM.Search("Tula");

            //Check wether town "Tula" had been found and automaticly choosen
            Assert.AreEqual("Tula", homePageVM.SelectedTown.TownName);
        }

    }
}
