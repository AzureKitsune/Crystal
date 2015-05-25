using Crystal3.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Model
{
    public partial class ViewModelBase
    {
        protected internal NavigationService NavigationService { get; internal set; }

        private FrameLevel GetNavigationFrameLevel()
        {
            return NavigationService.NavigationLevel;
        }


        protected internal virtual bool OnNavigatingTo(object sender, NavigatingCancelEventArgs e)
        {
            return false;
        }

        protected internal virtual void OnNavigatedTo(object sender, NavigationEventArgs e)
        {
            
        }

        protected internal virtual bool OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            return false;
        }

        protected internal virtual void OnNavigatedFrom(object sender, NavigationEventArgs e)
        {
            
        }
    }
}
