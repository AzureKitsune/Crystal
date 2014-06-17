using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Crystal2.Core
{
    public sealed class WinRTDispatcher : IUIDispatcher
    {
        private CoreDispatcher dispatcherObject = null;
        internal WinRTDispatcher()
        {
            SetDispatcher();
        }

        private void SetDispatcher()
        {
            try
            {
                dispatcherObject = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            }
            catch (InvalidOperationException) { }
        }

        public Task RunAsync(Action callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            if (dispatcherObject == null) SetDispatcher();

            return dispatcherObject.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() => callback())).AsTask();
        }
    }
}
