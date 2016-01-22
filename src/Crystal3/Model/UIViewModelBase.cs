using Crystal3.Navigation;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    /// <summary>
    /// A view model base class with UI sugar added on top.
    /// </summary>
    public abstract class UIViewModelBase: ViewModelBase
    {
        /// <summary>
        /// Creates a new UIViewModelBase
        /// </summary>
        public UIViewModelBase()
        {
           Status = WindowManager.GetStatusManagerForCurrentWindow();
        }

        /// <summary>
        /// Returns whether the View Model is busy.
        /// </summary>
        public bool IsBusy { get { return GetPropertyValue<bool>(); } protected set { SetPropertyValue<bool>(value: value); } }

        /// <summary>
        /// Returns the StatusManager instance for this View Model.
        /// </summary>
        public StatusManager Status { get; private set; }

        protected internal override void OnNavigatedFrom(object sender, CrystalNavigationEventArgs e)
        {
            //Clean up!
            Status.RemoveAllControllersForCallingViewModel(this);

            base.OnNavigatedFrom(sender, e);
        }
    }
}
