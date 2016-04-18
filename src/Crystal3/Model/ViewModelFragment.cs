using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Navigation;

namespace Crystal3.Model
{
    public abstract class ViewModelFragment: UIViewModelBase, IDisposable
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
        protected internal override sealed Task OnSuspendingAsync(IDictionary<string, object> data)
        {
            return base.OnSuspendingAsync(data);
        }

        public abstract void Invoke(ViewModelBase viewModel, object data);

        public abstract void Dispose();
    }
}
