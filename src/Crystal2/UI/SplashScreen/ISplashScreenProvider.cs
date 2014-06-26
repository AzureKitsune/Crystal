using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.UI.SplashScreen
{
    public interface ISplashScreenProvider: IIoCObject
    {
        Task ActivateAsync();
        Task DeactivateAsync();
        bool IsSplashScreenVisible { get; }
    }
}
