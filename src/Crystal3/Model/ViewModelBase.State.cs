using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    public partial class ViewModelBase
    {
        /// <summary>
        /// This is called when your application is getting sent to the background and give it a chance to save its state (in case it is terminated).
        /// </summary>
        /// <returns></returns>
        protected internal virtual Task OnPreservingAsync(IDictionary<string, object> data)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// This is called when your application returns to the foreground to give your viewmodel a chance to refresh its state.
        /// </summary>
        /// <returns></returns>
        protected internal virtual Task OnRefreshingAsync()
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// This is called when your application is getting sent to the background. You should use OnPreservingAsync.
        /// </summary>
        /// <returns></returns>
        protected internal virtual Task OnSuspendingAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// This is called when your application is activated and needs to restore its state from termination.
        /// </summary>
        /// <returns></returns>
        protected internal virtual Task OnRestoringAsync(IDictionary<string, object> data)
        {
            data?.Clear();

            return Task.CompletedTask;
        }

        /// <summary>
        /// This is called when your application returns to the foreground to give your viewmodel a chance to refresh its state. You should use OnRefreshingAsync instead.
        /// </summary>
        /// <returns></returns>
        protected internal virtual Task OnResumingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
