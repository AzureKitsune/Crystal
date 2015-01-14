using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Navigation
{
    public class CrystalWinRTNavigationEventArgs: CrystalNavigationEventArgs
    {
        public CrystalWinRTNavigationEventArgs(object parameter): base ()
        {
            Parameter = parameter;
        }

        public Uri TargetUri { get; internal set; }
        public CrystalNavigationDirection Direction { get; internal set; }
    }
}
