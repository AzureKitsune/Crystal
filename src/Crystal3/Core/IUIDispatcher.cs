using Crystal3.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Core
{
    public interface IUIDispatcher : IIoCObject
    {
        Task RunAsync(Action callback);
        Task RunAsync(IUIDispatcherPriority priority, Action callback);

        Task<T> RunAsync<T>(Func<T> callback);
        Task<T> RunAsync<T>(IUIDispatcherPriority priority, Func<T> callback);

        Task RunWhenIdleAsync(Action callback);
        Task<T> RunWhenIdleAsync<T>(Func<T> callback);

        bool HasThreadAccess { get; }

        Task YieldAsync();
        Task YieldAsync(IUIDispatcherPriority priority);
    }
    public enum IUIDispatcherPriority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }
}
