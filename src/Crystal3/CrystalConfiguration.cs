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

        }

        public bool HandleSystemBackNavigation { get; set; }
        public bool HandleBackButtonForTopLevelNavigation { get; set; }
    }
}
