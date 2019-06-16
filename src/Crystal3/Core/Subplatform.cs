using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Core
{
    public enum Subplatform
    {
        None,
        /// <summary>
        /// Desktop and Holographic only (as of 6/16/2019)
        /// </summary>
        MixedReality,
        /// <summary>
        /// Desktop mode only
        /// </summary>
        TabletMode,
    }
}
