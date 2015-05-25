using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Crystal3.Navigation
{
    public class NavigationService
    {
        public Frame NavigationFrame { get; private set; }

        public NavigationService(Frame navFrame)
        {
            if (NavigationFrame == null) throw new ArgumentNullException("navFrame");
            NavigationFrame = navFrame;

        }

        public void NavigateTo<T>(object parameter = null) where T: ViewModelBase
        {
            var view = NavigationManager.GetView(typeof(T));

            NavigationFrame.Navigate(view, parameter);
        }
    }
}
