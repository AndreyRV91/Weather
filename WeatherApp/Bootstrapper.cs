using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WeatherApp.Contracts;
using WeatherApp.Core;
using WeatherApp.Core.Infrastructure;
using WeatherApp.Core.Models.ProgramSettings;
using WeatherApp.ViewModels;
using WeatherLibrary;

namespace WeatherApp
{
    public class Bootstrapper: BootstrapperBase
    {
        SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<ITownsRepository, TownsRepository>();
            _container.Singleton<IMenuPage, SettingsViewModel>();
            _container.Singleton<IMenuPage, HomePageViewModel>();

            _container
                .PerRequest<IDataAccess, DataFromOpenweathermap>();

            var settings = new ProgramSettings();

            _container.Instance<IProgramSettings>(settings);

            GetType().Assembly.GetTypes().
                Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var settings = (ProgramSettings)_container.GetInstance<IProgramSettings>();
            if(!settings.LoadSettings())
            {
                settings.SetDefault();
                settings.SaveSettings();
            }
            LocalizationManager.Instance.SetCultureAtStartUp(settings);

            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
