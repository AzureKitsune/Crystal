using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2
{
    /// <summary>
    /// Represents the core of any application using Crystal. Please, do not implement this interface manually.
    /// </summary>
    public interface ICrystalApplication
    {
        bool ShouldHandleSplashScreen { get; }

        void OnInitialize();
    }
}
