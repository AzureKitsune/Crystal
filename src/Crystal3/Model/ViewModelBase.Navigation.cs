using Crystal3.Core;
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


        protected internal virtual bool OnNavigatingTo(object sender, CrystalNavigationEventArgs e)
        {
            return false;
        }

        protected internal virtual void OnNavigatedTo(object sender, CrystalNavigationEventArgs e)
        {
            
        }

        protected internal virtual bool OnNavigatingFrom(object sender, CrystalNavigationEventArgs e)
        {
            return false;
        }

        protected internal virtual void OnNavigatedFrom(object sender, CrystalNavigationEventArgs e)
        {
            
        }
    }
}
