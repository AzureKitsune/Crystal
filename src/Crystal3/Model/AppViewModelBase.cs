using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Navigation;
using Windows.ApplicationModel.Activation;

namespace Crystal3.Model
{
    public class AppViewModelBase : UIViewModelBase
    {
        protected internal override void OnNavigatedTo(object sender, CrystalNavigationEventArgs e)
        {
            base.OnNavigatedTo(sender, e);
        }

        protected internal virtual void OnAppActivated(IActivatedEventArgs args)
        {
            
        }

        protected internal virtual void OnAppLaunched(ILaunchActivatedEventArgs args)
        {
            
        }

        protected internal virtual void OnAppRestoredLaunched(ILaunchActivatedEventArgs args)
        {
            
        }
    }
}
