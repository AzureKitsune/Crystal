using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.UI.SplashScreen
{
    public interface IWinRTSplashScreenProvider: ISplashScreenProvider
    {
        void Setup(Windows.ApplicationModel.Activation.IActivatedEventArgs args, string splashBackgroundColor, string splashScreenImagePath);
    }
}
