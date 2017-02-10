using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Navigation;
using Windows.UI.Xaml;

namespace Crystal3.Model
{
    public abstract class ViewModelFragment: UIViewModelBase, IDisposable
    {
        #region ViewModelBase
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
        protected internal override sealed Task OnSuspendingAsync()
        {
            return base.OnSuspendingAsync();
        }
        protected internal override sealed void OnNavigatedFrom(object sender, CrystalNavigationEventArgs e)
        {
            base.OnNavigatedFrom(sender, e);
        }
        protected internal override sealed void OnNavigatedTo(object sender, CrystalNavigationEventArgs e)
        {
            base.OnNavigatedTo(sender, e);
        }
        protected internal override sealed Task OnPreservingAsync(IDictionary<string, object> data)
        {
            return base.OnPreservingAsync(data);
        }
        protected internal override sealed Task OnRestoringAsync(IDictionary<string, object> data)
        {
            return base.OnRestoringAsync(data);
        }
        protected internal override sealed Task OnRefreshingAsync()
        {
            return base.OnRefreshingAsync();
        }

        #endregion

        public virtual void Invoke(ViewModelBase viewModel, object data)
        {
            return;
        }
        public virtual void OnVisibilityChanged(Visibility visibility, object data)  { }
        public virtual void Dispose()
        {
            return;
        }
    }
}
