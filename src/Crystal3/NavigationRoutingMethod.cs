using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3
{
    public enum NavigationRoutingMethod
    {
        /// <summary>
        /// Dynamic finds views and view models and pairs them using reflection.
        /// </summary>
        Dynamic = 0,
        /// <summary>
        /// Relies on the CrystalApplication sub-class to provide view types for view models.
        /// </summary>
        StaticResolution = 1,
    }
}
