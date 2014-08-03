using Crystal2.Actions;
using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Crystal2.Navigation
{
    public class NavigationCommands
    {
        INavigationProvider navigationProvider = null;
        CrystalRelayCommand backCommand = null;
        CrystalRelayCommand forwardCommand = null;
        CrystalRelayCommand homeCommand = null;

        public NavigationCommands()
        {
            navigationProvider = IoCManager.Resolve<INavigationProvider>();

            backCommand = new CrystalRelayCommand(
                x =>
                    navigationProvider.CanGoBackward,
                x =>
                    navigationProvider.GoBackward());
            forwardCommand = new CrystalRelayCommand(
                x =>
                    navigationProvider.CanGoForward,
                x =>
                    navigationProvider.GoForward());
            homeCommand = new CrystalRelayCommand(
                x =>
                    !navigationProvider.IsHome,
                x =>
                    navigationProvider.GoHome());

            
            navigationProvider.Navigated += navigationProvider_Navigated;
        }

        ~NavigationCommands()
        {
            if (navigationProvider != null)
                navigationProvider.Navigated -= navigationProvider_Navigated;
        }

        void navigationProvider_Navigated(object sender, CrystalNavigationEventArgs e)
        {
            GoBackwardCommand.RaiseCanExecuteChanged();
            GoForwardCommand.RaiseCanExecuteChanged();
            GoHomeCommand.RaiseCanExecuteChanged();
        }

        public CrystalRelayCommand GoBackwardCommand
        {
            get { return backCommand; }
        }

        public CrystalRelayCommand GoForwardCommand
        {
            get { return forwardCommand; }
        }

        public CrystalRelayCommand GoHomeCommand
        {
            get { return homeCommand; }
        }
    }
}
