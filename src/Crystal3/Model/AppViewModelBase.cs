using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Crystal3.Model
{
    public class AppViewModelBase : UIViewModelBase
    {
        internal virtual void OnActivatedAsync(IActivatedEventArgs args)
        {
            
        }
    }
}
