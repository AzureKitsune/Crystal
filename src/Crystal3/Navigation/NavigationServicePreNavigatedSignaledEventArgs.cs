using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Model;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    public class NavigationServicePreNavigatedSignaledEventArgs: EventArgs
    {
        public NavigationEventArgs InnerEventArgs { get; private set; }
        public ViewModelBase ViewModel { get; private set; }

        internal NavigationServicePreNavigatedSignaledEventArgs(ViewModelBase viewModel, NavigationEventArgs e)
        {
            ViewModel = viewModel;
            InnerEventArgs = e;
        }

    }
}
