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

            return RunAsync(IUIDispatcherPriority.Normal, callback);
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


        public Task<T> RunAsync<T>(Func<T> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            return RunAsync<T>(IUIDispatcherPriority.Normal, callback);
        }

        public Task<T> RunAsync<T>(IUIDispatcherPriority priority, Func<T> callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            TaskCompletionSource<T> objectTask = new TaskCompletionSource<T>();

            dispatcherObject.Value.RunAsync(ConvertToCoreDispatcherPriority(priority), new DispatchedHandler(() =>
                {
                    objectTask.TrySetResult(callback());
                }));

            return objectTask.Task;
        }
    }
}
