using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Crystal2.UI.SplashScreen
{
    public interface IWinRTSplashScreenProvider: ISplashScreenProvider
    {
        void Setup(string splashBackgroundColor, string splashScreenImagePath);
        void PreActivationHook(Windows.ApplicationModel.Activation.IActivatedEventArgs args, Frame rootFrame, Task workTask);
        Task CompletionTask { get; }
    }
}
