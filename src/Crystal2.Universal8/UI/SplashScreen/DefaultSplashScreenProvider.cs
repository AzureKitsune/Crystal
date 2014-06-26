using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Crystal2.UI.SplashScreen
{
    public class DefaultSplashScreenProvider : IWinRTSplashScreenProvider
    {
        private bool isVisible;
        private DefaultWinRTSplashScreen splashScreen = null;
        private string backgroundColor = "";
        private string imagePath = "";
        public virtual void Setup(string splashBackgroundColor, string splashScreenImagePath)
        {
            backgroundColor = splashBackgroundColor;
            imagePath = splashScreenImagePath;
        }

        public void PreActivationHook(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            args.SplashScreen.Dismissed += SplashScreen_Dismissed;
            splashScreen = new DefaultWinRTSplashScreen(args.SplashScreen, backgroundColor, imagePath);
        }

        void SplashScreen_Dismissed(Windows.ApplicationModel.Activation.SplashScreen sender, object args)
        {
            sender.Dismissed -= SplashScreen_Dismissed;
            DeactivateAsync();
        }

        public Task ActivateAsync()
        {
            isVisible = true;

            if (!Crystal2.CrystalWinRTApplication.IsPhone())
                ((Frame)Window.Current.Content).Background = new SolidColorBrush(Crystal2.Utilities.ColorHelper.ParseHex(backgroundColor));

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
