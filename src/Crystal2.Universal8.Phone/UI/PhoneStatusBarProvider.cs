using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Crystal2.UI
{
    public class PhoneStatusBarProvider: IStatusBarProvider
    {
        public double? Progress
        {
            get
            {
                return StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue;
            }
            set
            {
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = value;
            }
        }

        public string Message
        {
            get
            {
                return StatusBar.GetForCurrentView().ProgressIndicator.Text;
            }
            set
            {
                StatusBar.GetForCurrentView().ProgressIndicator.Text = value;
            }
        }

        public Task ShowAsync()
        {
            return StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync().AsTask();
        }

        public Task HideAsync()
        {
            return StatusBar.GetForCurrentView().ProgressIndicator.HideAsync().AsTask();
        }
    }
}
