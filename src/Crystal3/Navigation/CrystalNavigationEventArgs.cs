using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    /// <summary>
    /// EventArgs containing information about the navigation process.
    /// </summary>
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

        public CrystalNavigationDirection Direction { get; set; }
        public object Parameter { get; private set; }
    }
}
