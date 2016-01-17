using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Navigation;

namespace Crystal3.Model
{
    /// <summary>
    /// A ViewModel that pretends to be a Model
    /// </summary>
    public class ModelViewModelBase: UIViewModelBase
    {
        protected internal override sealed bool OnNavigatingFrom(object sender, CrystalNavigationEventArgs e)
        {
            return base.OnNavigatingFrom(sender, e);
        }
        protected internal override sealed bool OnNavigatingTo(object sender, CrystalNavigationEventArgs e)
        {
            return base.OnNavigatingTo(sender, e);
        }
        protected internal override sealed Task OnResumingAsync()
        {
            return base.OnResumingAsync();
        }
        protected internal override Task OnSuspendingAsync(object data)
        {
            return base.OnSuspendingAsync(data);
        }
    }
}
