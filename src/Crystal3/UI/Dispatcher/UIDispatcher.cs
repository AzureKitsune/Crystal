using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crystal3.Core;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace Crystal3.UI.Dispatcher
{
    internal class UIDispatcher : Crystal3.Core.IUIDispatcher
    {
        private CoreDispatcher dispatcher = null;

        public bool HasThreadAccess
        {
            get
            {
                return dispatcher.HasThreadAccess;
            }
        }

        internal UIDispatcher(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task RunAsync(Action action)
        {
            return dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(action)).AsTask();
        }

        public Task RunAsync(IUIDispatcherPriority priority, Action callback)
        {
            CoreDispatcherPriority dispatcherPriority = ConvertIUIDispatcherPriorityToCoreDispatcherPriority(priority);

            return dispatcher.RunAsync(dispatcherPriority, new Windows.UI.Core.DispatchedHandler(callback)).AsTask();
        }

        private CoreDispatcherPriority ConvertIUIDispatcherPriorityToCoreDispatcherPriority(IUIDispatcherPriority priority)
        {
            CoreDispatcherPriority dispatcherPriority = CoreDispatcherPriority.Normal;

            switch (priority)
            {
                case IUIDispatcherPriority.Normal:
                    dispatcherPriority = CoreDispatcherPriority.Normal;
                    break;
                case IUIDispatcherPriority.High:
                    dispatcherPriority = CoreDispatcherPriority.High;
                    break;
                case IUIDispatcherPriority.Low:
                    dispatcherPriority = CoreDispatcherPriority.Low;
                    break;
            }

            return dispatcherPriority;
        }

        public Task<T> RunAsync<T>(Func<T> callback)
        {
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();

            return RunWithTaskCompletionSource(callback, taskCompletionSource);
        }

        private Task<T> RunWithTaskCompletionSource<T>(Func<T> callback, TaskCompletionSource<T> taskCompletionSource)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            {
                taskCompletionSource.SetResult(callback());
            }));

            return taskCompletionSource.Task;
        }

        private Task<T> RunWhenIdleWithTaskCompletionSource<T>(Func<T> callback, TaskCompletionSource<T> taskCompletionSource)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            dispatcher.RunIdleAsync(new Windows.UI.Core.IdleDispatchedHandler(args =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            {
                taskCompletionSource.SetResult(callback());
            }));

            return taskCompletionSource.Task;
        }

        public Task<T> RunAsync<T>(IUIDispatcherPriority priority, Func<T> callback)
        {
            CoreDispatcherPriority dispatcherPriority = ConvertIUIDispatcherPriorityToCoreDispatcherPriority(priority);

            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();

            return RunWithTaskCompletionSource(callback, taskCompletionSource);
        }

        public Task RunWhenIdleAsync(Action callback)
        {
            return dispatcher.RunIdleAsync(new Windows.UI.Core.IdleDispatchedHandler(args =>
            {
                callback();
            })).AsTask();
        }

        public Task<T> RunWhenIdleAsync<T>(Func<T> callback)
        {
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();

            return RunWithTaskCompletionSource(callback, taskCompletionSource);
        }
    }
}
