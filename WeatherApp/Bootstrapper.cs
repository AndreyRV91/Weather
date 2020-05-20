using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeatherApp.Contracts;
using WeatherApp.Models.ProgramSettings;
using WeatherApp.ViewModels;
using WeatherLibrary;

namespace WeatherApp
{
    public class Bootstrapper: BootstrapperBase
    {
        SimpleContainer container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container.Instance(container);

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.Singleton<IMenuPage, SettingsViewModel>();
            container.Singleton<IMenuPage, HomePageViewModel>();

            container
                .PerRequest<IDataAccess, DataFromOpenweathermap>();

            var settings = new ProgramSettings();

            container.Instance<IProgramSettings>(settings);

            GetType().Assembly.GetTypes().
                Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var settings = (ProgramSettings)container.GetInstance<IProgramSettings>();
            if(!settings.LoadSettings())
            {
                settings.SetDefault();
            }
            LocalizationManager.Instance.SetCultureAtStartUp();

            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}
