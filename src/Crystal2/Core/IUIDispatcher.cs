using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Core
{
    public interface IUIDispatcher: IIoCObject
    {
        Task RunAsync(Action callback);
    }
}
