using Crystal2;
using Crystal2.Navigation;
using CrystalSpeedLaunchTest.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CrystalSpeedLaunchTest
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : CrystalWinRTApplication
    {
        Stopwatch stopwatch = new Stopwatch();
        public App()
            : base()
        {
            stopwatch.Start();
        }
        protected override void OnNormalLaunchNavigationReady(IActivatedEventArgs args)
        {
            NavigationManager.NavigateTo<MainPageViewModel>(stopwatch);
        }
    }
}