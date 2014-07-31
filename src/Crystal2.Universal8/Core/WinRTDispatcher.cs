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
        private Lazy<CoreDispatcher> dispatcherObject = null;
        internal WinRTDispatcher()
        {
            dispatcherObject = new Lazy<CoreDispatcher>(() =>
            {
                try
                {
                    return Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                }
                catch (InvalidOperationException) { return null; }
            });
        }

        public Task RunAsync(Action callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            return dispatcherObject.Value.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() => callback())).AsTask();
        }


        public Task RunAsync(IUIDispatcherPriority priority, Action callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            return dispatcherObject.Value.RunAsync(ConvertToCoreDispatcherPriority(priority), new DispatchedHandler(() => callback())).AsTask();
        }

        private CoreDispatcherPriority ConvertToCoreDispatcherPriority(IUIDispatcherPriority priority)
        {
            var name = Enum.GetName(typeof(IUIDispatcherPriority), priority);
            return (CoreDispatcherPriority)Enum.Parse(typeof(CoreDispatcherPriority), name);
        }
    }
}
