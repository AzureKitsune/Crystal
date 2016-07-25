using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal3
{
    public sealed class CrystalConfiguration
    {
        internal CrystalConfiguration()
        {
            ViewModelRefreshMethod = ViewModelRefreshMethod.OnRefreshingAsync;
            NavigationRoutingMethod = NavigationRoutingMethod.Dynamic;
            HandleSystemBackNavigation = true;

            OverridePlatformDetection = false;
        }

        public bool HandleSystemBackNavigation { get; set; }
        public bool HandleBackButtonForTopLevelNavigation { get; set; }
        public ViewModelRefreshMethod ViewModelRefreshMethod { get; set; }
        public NavigationRoutingMethod NavigationRoutingMethod { get; set; }

        public void OverridePlatform(Core.Platform plat)
        {
            OverridePlatformDetection = true;
            OverridePlatformValue = plat;
        }

        public bool OverridePlatformDetection { get; private set; }
        public Core.Platform OverridePlatformValue { get; private set; }
    }
}
