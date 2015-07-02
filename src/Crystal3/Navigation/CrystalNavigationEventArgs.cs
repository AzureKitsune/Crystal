using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    public class CrystalNavigationEventArgs: EventArgs
    {
        public CrystalNavigationEventArgs()
        {
        }

        public CrystalNavigationEventArgs(NavigatingCancelEventArgs args)
        {

        }

        public CrystalNavigationEventArgs(NavigationEventArgs args)
        {

        }
    }
}
