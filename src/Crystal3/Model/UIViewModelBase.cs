using Crystal3.Navigation;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    public abstract class UIViewModelBase: ViewModelBase
    {
        public UIViewModelBase()
        {
           Status = WindowManager.GetStatusManagerForCurrentWindow();
        }

        public bool IsBusy { get { return GetPropertyValue<bool>(); } protected set { SetPropertyValue<bool>(value: value); } }

        public StatusManager Status { get; private set; }

        protected internal override void OnNavigatedFrom(object sender, CrystalNavigationEventArgs e)
        {
            Status.RemoveAllControllersForCallingViewModel(this);

            base.OnNavigatedFrom(sender, e);
        }
    }
}
