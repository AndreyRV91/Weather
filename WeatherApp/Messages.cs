using Caliburn.Micro;

namespace WeatherApp.Messages
{
    public class ChangeTheme:PropertyChangedBase
    {
        private int _theme;

        public int Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
