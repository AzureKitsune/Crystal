using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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


            this.Background = new SolidColorBrush(ParseHex(splashBackgroundColor));

            var bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri("ms-appx:///" + splashScreenImagePath);
            layoutRoot.Background = new ImageBrush()
            {
                ImageSource = bitmapImage
            };
        }
        private Color ParseHex(string hexCode)
        {
            //http://stackoverflow.com/a/16815300/2263199
            //modified by Amrykid

            int index = hexCode.StartsWith("#") ? -2 : -3;

            var color = new Color();
            if (hexCode.Length >= 9)
            {
                color.A = byte.Parse(hexCode.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                index = hexCode.StartsWith("#") ? -1 : 0;
            }
            else
                color.A = 255;

            color.R = byte.Parse(hexCode.Substring(index + 3, 2), NumberStyles.AllowHexSpecifier);
            color.G = byte.Parse(hexCode.Substring(index + 5, 2), NumberStyles.AllowHexSpecifier);
            color.B = byte.Parse(hexCode.Substring(index + 7, 2), NumberStyles.AllowHexSpecifier);

            return color;
        }
    }
}
