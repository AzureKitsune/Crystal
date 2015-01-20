using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.UI
{
    public interface IStatusBarProvider: IIoCObject
    {
        double? Progress { get; set; }
        string Message { get; set; }
        Task ShowAsync();
        Task HideAsync();
    }
    internal class DummyStatusBarProvider: IStatusBarProvider
    {
        public double? Progress { get; set; }

        public string Message { get; set; }

        public Task ShowAsync()
        {
            return Task.FromResult<object>(null);
        }

        public Task HideAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
