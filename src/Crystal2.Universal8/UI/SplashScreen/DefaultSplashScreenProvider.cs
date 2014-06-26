using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal2.UI.SplashScreen
{
    public class DefaultSplashScreenProvider : IWinRTSplashScreenProvider
    {
        private bool isVisible;
        private DefaultWinRTSplashScreen splashScreen = null;
        public virtual void Setup(Windows.ApplicationModel.Activation.IActivatedEventArgs args, string splashBackgroundColor, string splashScreenImagePath)
        {
            args.SplashScreen.Dismissed += SplashScreen_Dismissed;
            splashScreen = new DefaultWinRTSplashScreen(splashBackgroundColor, splashScreenImagePath);
        }

        void SplashScreen_Dismissed(Windows.ApplicationModel.Activation.SplashScreen sender, object args)
        {
            sender.Dismissed -= SplashScreen_Dismissed;
            DeactivateAsync();
        }

        public Task ActivateAsync()
        {
            isVisible = true;

            ((Frame)Window.Current.Content).Content = splashScreen;

            return Task.FromResult<object>(null);
        }

        public Task DeactivateAsync()
        {
            isVisible = false;

            if (Window.Current != null)
                Window.Current.Content = null;

            return Task.FromResult<object>(null);
        }


        public bool IsSplashScreenVisible
        {
            get { return isVisible; }
        }
    }
}
