using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Crystal2.UI.SplashScreen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class DefaultWinRTSplashScreen : Page
    {
        public DefaultWinRTSplashScreen(string splashBackgroundColor, string splashScreenImagePath)
        {
            this.InitializeComponent();

            this.Background = new SolidColorBrush(Crystal2.Utilities.ColorHelper.ParseHex(splashBackgroundColor));

            var bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:///" + splashScreenImagePath.Replace("\\","/"));
            appSplashImage.Source = bitmapImage;

            this.Loaded += DefaultWinRTSplashScreen_Loaded;
            this.Unloaded += DefaultWinRTSplashScreen_Unloaded;
        }

        internal void HandleSplashActivation(Windows.ApplicationModel.Activation.SplashScreen splash)
        {
            if (CrystalWinRTApplication.IsPhone())
            {
                imageCanvas.Margin = new Thickness(0, -25, 0, 0); //account for the statusbar.
                imageCanvas.Width = Window.Current.Bounds.Width;
                imageCanvas.Height = Window.Current.Bounds.Height;

                appSplashImage.Width = imageCanvas.Width;
                appSplashImage.Height = imageCanvas.Height;
                //appSplashImage.Stretch = Stretch.Fill;

                appSplashImage.SetValue(Canvas.LeftProperty, 0);
                appSplashImage.SetValue(Canvas.TopProperty, 0);
            }
            else
            {
                appSplashImage.Height = splash.ImageLocation.Height;
                appSplashImage.Width = splash.ImageLocation.Width;

                appSplashImage.SetValue(Canvas.LeftProperty, splash.ImageLocation.Left);
                appSplashImage.SetValue(Canvas.TopProperty, splash.ImageLocation.Top);
            }
        }

        private DisplayOrientations oldOrientation = DisplayOrientations.None;
        void DefaultWinRTSplashScreen_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= DefaultWinRTSplashScreen_Loaded;
            this.Unloaded -= DefaultWinRTSplashScreen_Unloaded;

            DisplayInformation.AutoRotationPreferences = oldOrientation;
        }

        void DefaultWinRTSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            oldOrientation = DisplayInformation.AutoRotationPreferences;

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }
    }
}
