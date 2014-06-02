using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Model
{
    /// <summary>
    /// A ViewModelBase that implements IsBusy and IsBusyStatusText for WinRT platforms.
    /// </summary>
    public abstract class WinRTBusyViewModelBase : ViewModelBase
    {
        public WinRTBusyViewModelBase()
        {
            //Sets the property keys for IsBusy and IsBusyStatusText
            IsBusyPropertyKey = GetProperty("IsBusy");
            IsBusyStatusTextKey = GetProperty("IsBusyStatusText");
        }

        /// <summary>
        /// Keys for IsBusy and IsBusyStatusText
        /// </summary>
        protected ViewModelPropertyKey IsBusyPropertyKey = null;
        protected ViewModelPropertyKey IsBusyStatusTextKey = null;

        public bool IsBusy
        {
            get { return GetPropertyValue<bool>(IsBusyPropertyKey); }
            protected set
            {
                SetPropertyValue<bool>(IsBusyPropertyKey, value);

                //if (CrystalWinRTApplication.IsPhone())
                //{
                //    var statusBar = Type.GetType("Windows.UI.ViewManagement.StatusBar")
                //        .GetTypeInfo()
                //        .GetDeclaredMethod("GetForCurrentView")
                //        .Invoke(null, new object[] { null });

                //    var statusBarIndicator = statusBar.GetType()
                //        .GetTypeInfo()
                //        .GetDeclaredField("ProgressIndicator")
                //        .GetValue(statusBar);

                //    if (value)
                //    {
                //        statusBar.GetType()
                //            .GetTypeInfo()
                //            .GetDeclaredMethod("ShowAsync")
                //            .Invoke(statusBar, new object[] { null });
                //    }
                //    else
                //    {
                //        statusBar.GetType()
                //            .GetTypeInfo()
                //            .GetDeclaredMethod("HideAsync")
                //            .Invoke(statusBar, new object[] { null });
                //    }
                //}
            }
        }
        public string IsBusyStatusText
        {
            get { return GetPropertyValue<string>(IsBusyStatusTextKey); }
            protected set { SetPropertyValue<string>(IsBusyStatusTextKey, value); }
        }
    }
}
