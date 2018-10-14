using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Crystal3.Utilities
{
    /* Code from: http://blog.wpdev.fr/dispatcher-yield-when-and-how-to-use-it-on-winrt/ */

    public static class DispatcherExtensions
    {
        public static DispatcherPriorityAwaitable Yield(this CoreDispatcher dispatcher)
        {
            return Yield(dispatcher, CoreDispatcherPriority.Low);
        }

        public static DispatcherPriorityAwaitable Yield(this CoreDispatcher dispatcher, CoreDispatcherPriority priority)
        {
            return new DispatcherPriorityAwaitable(dispatcher, priority);
        }
    }

    public struct DispatcherPriorityAwaitable
    {
        private readonly CoreDispatcher dispatcher;

        private readonly CoreDispatcherPriority priority;

        internal DispatcherPriorityAwaitable(CoreDispatcher dispatcher, CoreDispatcherPriority priority)
        {
            this.dispatcher = dispatcher;
            this.priority = priority;
        }

        public DispatcherPriorityAwaiter GetAwaiter()
        {
            return new DispatcherPriorityAwaiter(this.dispatcher, this.priority);
        }
    }



    public struct DispatcherPriorityAwaiter : INotifyCompletion
    {
        private readonly CoreDispatcher dispatcher;

        private readonly CoreDispatcherPriority priority;

        public bool IsCompleted
        {
            get
            {
                return false;
            }
        }



        internal DispatcherPriorityAwaiter(CoreDispatcher dispatcher, CoreDispatcherPriority priority)
        {
            this.dispatcher = dispatcher;
            this.priority = priority;
        }


        public void GetResult()
        {

        }

        public void OnCompleted(Action continuation)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            dispatcher.RunAsync(priority, new DispatchedHandler(continuation));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

    }
}
