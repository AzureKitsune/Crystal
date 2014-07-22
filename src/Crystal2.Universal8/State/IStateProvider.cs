using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.State
{
    public interface IStateProvider: IIoCObject
    {
        Task LoadStateAsync();
        Task SaveStateAsync();
        bool CanStateBeStored { get; }

        StateObject State { get; }
    }
}
