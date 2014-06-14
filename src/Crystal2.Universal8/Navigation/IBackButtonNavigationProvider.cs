using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Navigation
{
    public interface IBackButtonNavigationProvider: IIoCObject
    {
        void Attach(CrystalWinRTApplication app);
    }
}
