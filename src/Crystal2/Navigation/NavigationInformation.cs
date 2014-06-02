using Crystal2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal2.Navigation
{
    /// <summary>
    /// A class that represents the information to be used to navigate to a view.
    /// </summary>
    public class NavigationInformation
    {
        internal NavigationInformation() { }

        public ViewModelBase TargetViewModel { get; internal set; }
        public Uri TargetUri { get; internal set; }
        public object Parameter { get; internal set; }
    }
}
