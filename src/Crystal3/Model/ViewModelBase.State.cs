using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    public partial class ViewModelBase
    {
        protected internal virtual Task OnSuspendingAsync(object data)
        {
            return Task.FromResult<object>(null);
        }

        protected internal virtual Task OnResumingAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
