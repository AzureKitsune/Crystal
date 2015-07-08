using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Crystal3.Navigation
{
    internal class WindowBundle
    {
        internal Window WindowView { get; set; }
        internal NavigationManager NavigationManager { get; set; }
        internal StatusManager StatusManager { get; set; }
    }
}
