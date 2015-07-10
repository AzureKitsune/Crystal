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
            Parameter = args.Parameter;
        }

        public CrystalNavigationEventArgs(NavigationEventArgs args)
        {
            Parameter = args.Parameter;
        }

        public object Parameter { get; private set; }
    }
}
