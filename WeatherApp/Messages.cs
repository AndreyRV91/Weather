using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Messages
{
    public class ChangeLanguage:PropertyChangedBase
    { }

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
