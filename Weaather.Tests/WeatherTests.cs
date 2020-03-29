using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherLibrary;
using Weather;
using Weather.ViewModels;
using System.Windows;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace Weather.Tests
{
    [TestFixture]
    public class WeatherTests
    {

        //Тесты для методов в DataFromOpenweathermap
        [Test]
        public void GetCurrentWeather_ShouldGiveInformation()
        {
            //Arrange
            IDataAccess DataFromOpenweathermap = new DataFromOpenweathermap();

            //Act
            var data = DataFromOpenweathermap.GetCurrentWeather();


            //Assert
            foreach (var item in data)
            {
                Assert.NotNull(item.TownName);
                Assert.That(item.CurrentWeather.Humidity, Is.InRange(0, 100));
                Assert.That(item.CurrentWeather.Pressure, Is.InRange(500, 1500));
                Assert.NotNull(item.CurrentWeather.WindDirection);
                Assert.That(item.CurrentWeather.WindVelocity, Is.InRange(0, 50));
                Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunrise);
                Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunset);

                Assert.That(item.WeatherToday.Humidity, Is.InRange(0, 100));
                Assert.That(item.WeatherToday.Pressure, Is.InRange(500, 1500));
                Assert.NotNull(item.WeatherToday.WindDirection);
                Assert.That(item.WeatherToday.WindVelocity, Is.InRange(0, 50));
                Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunrise);
                Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunset);
            }

        }

        [Test]
        public void Search_ShouldReturnWeatherInTown()
        {

            //Arrange
            IDataAccess DataFromOpenweathermap = new DataFromOpenweathermap();

            //Act
            var item = DataFromOpenweathermap.GetCurrentWeather("Moscow");


            //Assert

            Assert.NotNull(item.TownName);
            Assert.That(item.CurrentWeather.Humidity, Is.InRange(0, 100));
            Assert.That(item.CurrentWeather.Pressure, Is.InRange(500, 1500));
            Assert.NotNull(item.CurrentWeather.WindDirection);
            Assert.That(item.CurrentWeather.WindVelocity, Is.InRange(0, 50));
            Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunrise);
            Assert.AreNotEqual(DateTime.MinValue, item.CurrentWeather.Sunset);

            Assert.That(item.WeatherToday.Humidity, Is.InRange(0, 100));
            Assert.That(item.WeatherToday.Pressure, Is.InRange(500, 1500));
            Assert.NotNull(item.WeatherToday.WindDirection);
            Assert.That(item.WeatherToday.WindVelocity, Is.InRange(0, 50));
            Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunrise);
            Assert.AreNotEqual(DateTime.MinValue, item.WeatherToday.Sunset);

        }


        //Тесты для проверки заполнения DataGrid, TextBox и нажатий на кнопки
        [Test]
        public void WeatherListIsNotEmpty()
        {

            //Arrange
            MockWeather mockWeather = new MockWeather();
            MainWindowViewModel viewModel = new MainWindowViewModel(mockWeather);

            //Act
            viewModel.UpdateTownList().Wait();
            viewModel.UpdateWeather();

            //Assert
            //Проверяем получили ли данные из MockWeather
            Assert.IsNotEmpty(viewModel.WeatherList);


        }

        [Test]
        public void WeatherParametersShouldBeInTownDataGrid()
        {
            //Arrange
            MockWeather mockWeather = new MockWeather();
            MainWindowViewModel viewModel = new MainWindowViewModel(mockWeather);

            //Act
            viewModel.UpdateTownList().Wait();
            viewModel.UpdateWeather();


            //Assert
            //Проверяем названия городов в DataGrid
            for (int i = 0; i < viewModel.WeatherList.Count; i++)
            {
                Assert.AreEqual(viewModel.WeatherList[i].TownName, viewModel.WeatherList[i].TownName);
            }

        }


        [Test]
        public void SelectedTown_ShouldBeMoscow()
        {
            //Arrange
            MockWeather mockWeather = new MockWeather();
            MainWindowViewModel viewModel = new MainWindowViewModel(mockWeather);

            //Act
            viewModel.UpdateTownList().Wait();
            viewModel.UpdateWeather();

            //Проверяем, что выбор города по умолчанию  в DataGrid - это первый город из коллекции
            Assert.AreEqual(viewModel.WeatherList.FirstOrDefault().TownName, viewModel.SelectedTown.TownName);

        }

        [Test]
        public void Search_ShouldBeTula()
        {
            //Arrange
            MockWeather mockWeather = new MockWeather();
            MainWindowViewModel viewModel = new MainWindowViewModel(mockWeather);

            //Act
            viewModel.UpdateTownList().Wait();
            viewModel.UpdateWeather();
            viewModel.TownName = "Tula";
            viewModel.Search().Wait();

            //Проверяем, что найден город "Тула" и выделение в DataGrid перемещено в SelectedTown
            Assert.AreEqual("Tula", viewModel.SelectedTown.TownName);
        }


    }
}
