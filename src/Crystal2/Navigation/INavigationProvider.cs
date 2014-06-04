using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Navigation
{
    /// <summary>
    /// An interface that handles the navigation of pages or the showing of windows depending on what is provided in the NavigationInformation class.
    /// </summary>
    public interface INavigationProvider: IIoCObject
    {
        /// <summary>
        /// Sets up the NavigationProvider, passing an object to be used for navigation.
        /// </summary>
        /// <param name="navigationObject">The navigation object to be used, if applicable. For WinRT, this is a Frame.</param>
        void Setup(object navigationObject);
        /// <summary>
        /// Tells the provider to perform navigation.
        /// </summary>
        /// <param name="information">The information for where we should navigate to.</param>
        /// <param name="provider">The directory provider to give us some possible choices.</param>
        void Navigate(NavigationInformation information, INavigationDirectoryProvider provider);

        object NavigationObject { get; }

        event EventHandler<CrystalNavigationEventArgs> Navigating;
        event EventHandler<CrystalNavigationEventArgs> Navigated;
    }
}
