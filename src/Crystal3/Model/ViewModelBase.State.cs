using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    public partial class ViewModelBase
    {
        protected internal virtual Task OnPreservingAsync(IDictionary<string, object> data)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnRefreshingAsync()
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnSuspendingAsync()
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnRestoringAsync(IDictionary<string, object> data)
        {
            data?.Clear();

            return Task.CompletedTask;
        }

        protected internal virtual Task OnResumingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
