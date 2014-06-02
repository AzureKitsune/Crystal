using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crystal2;

namespace Crystal2.Navigation
{
    /// <summary>
    /// Event args containing details about a navigation operation.
    /// </summary>
    public class CrystalNavigationEventArgs: EventArgs
    {
        protected CrystalNavigationEventArgs()
        {
            
        }

        public object Parameter { get; protected set; }
    }
}
