using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2
{
    public class CrystalWinRTConfiguration: CrystalConfiguration
    {
        internal CrystalWinRTConfiguration(): base()
        {
            AutomaticallyDiscoverViewModelPairs = true;
            AutomaticallyCallInitializeComponent = true;
        }
        public bool AutoDetectSplashScreenImage { get; set; }

        public bool AutomaticallyShowExtendedSplashScreen { get; set; }

        public bool AutomaticallyHandleSuspendingAndRestoringState { get; set; }

        public bool AutomaticallyCallInitializeComponent { get; set; }

        public bool AutomaticallyDiscoverViewModelPairs { get; set; }
    }
}
