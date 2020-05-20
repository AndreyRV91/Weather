using Caliburn.Micro;
using WeatherApp.Contracts;
using WeatherApp.Messages;
using WeatherApp.Models.ProgramSettings;

namespace WeatherApp.ViewModels
{
    public class SettingsViewModel: Screen, IMenuPage
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgramSettings _programSettings;

        public SettingsViewModel(IEventAggregator eventAggregator, IProgramSettings programSettings)
        {
            _programSettings = programSettings;
            _eventAggregator = eventAggregator;
        }

        protected override void OnActivate()
        {
            _eventAggregator.Subscribe(this);

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        public void SaveSettings()
        {
            _programSettings.SaveSettings();
        }
    }
}
