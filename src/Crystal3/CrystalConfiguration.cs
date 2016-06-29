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

        public bool OverridePlatformDetection { get; set; }
        public Core.Platform OverridePlatformValue { get; set; }
    }
}
