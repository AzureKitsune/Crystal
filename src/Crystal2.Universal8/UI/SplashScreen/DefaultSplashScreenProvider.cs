using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
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
        private Task<Task> callbackTask = null;
        private TaskCompletionSource<object> completionTaskBackend = null;
        private Task completionTask = null;
        private Frame rootFrame = null;
        public virtual void Setup(string splashBackgroundColor, string splashScreenImagePath)
        {
            backgroundColor = splashBackgroundColor;
            imagePath = splashScreenImagePath;
            splashScreen = new DefaultWinRTSplashScreen(backgroundColor, imagePath);

            completionTaskBackend = new TaskCompletionSource<object>();
            completionTask = completionTaskBackend.Task;
        }

        public async void PreActivationHook(Windows.ApplicationModel.Activation.IActivatedEventArgs args, Task<Task> workTask)
        {
            if (workTask.Status == TaskStatus.RanToCompletion)
            {
                completionTaskBackend.TrySetResult(null);
            }
            else
            {
                try
                {
                    args.SplashScreen.Dismissed += SplashScreen_Dismissed;

                    if (!CrystalWinRTApplication.IsPhone())
                        Window.Current.Activate();

                    splashScreen.HandleSplashActivation(args.SplashScreen);
                    rootFrame = CrystalWinRTApplication.Current.RootFrame;
                    callbackTask = workTask;

                    System.Diagnostics.Debug.WriteLine("Activating Splash Screen...");
                    ActivateAsync();

                    System.Diagnostics.Debug.WriteLine("Wait for image load...");
                    await splashScreen.ImageLoadingTask;

                    System.Diagnostics.Debug.WriteLine("Activating window...");
                    if (!Window.Current.Visible)
                        Window.Current.Activate();
                }
                catch (Exception) { }
            }
        }

        [DebuggerNonUserCode]
        async void SplashScreen_Dismissed(Windows.ApplicationModel.Activation.SplashScreen sender, object args)
        {
            sender.Dismissed -= SplashScreen_Dismissed;
            //await ActivateAsync();
            if (callbackTask != null)
            {
                try
                {
                    callbackTask.Start();
                }
                catch (Exception) { }
                await (await callbackTask);
            }
            await DeactivateAsync();
            completionTaskBackend.TrySetResult(null);
        }

        public async Task ActivateAsync()
        {
            isVisible = true;

            //((Frame)Window.Current.Content).Content = splashScreen;

            await IOC.IoCManager.Resolve<Crystal2.Core.IUIDispatcher>().RunAsync(Core.IUIDispatcherPriority.High, () =>
            {
                //rootFrame.Content = splashScreen;

                if (!Crystal2.CrystalWinRTApplication.IsPhone())
                {
                    rootFrame.Background = new SolidColorBrush(Crystal2.Utilities.ColorHelper.ParseHex(backgroundColor));
                }
            });
        }

        public Task DeactivateAsync()
        {
            if (!isVisible) return Task.FromResult<object>(null);

            isVisible = false;

            if (Window.Current != null)
                Window.Current.Content = new Grid();

            return Task.FromResult<object>(null);
        }


        public bool IsSplashScreenVisible
        {
            get { return isVisible; }
        }


        public Task CompletionTask
        {
            get { return completionTask; }
        }


        public void SetActivationArguments(Windows.ApplicationModel.Activation.IActivatedEventArgs e)
        {
            ActivatedEventArgs = e;
        }

        public IActivatedEventArgs ActivatedEventArgs { get; private set; }


        public void Preload()
        {
            CrystalWinRTApplication.Current.RootFrame.Content = splashScreen;

            Window.Current.Content = CrystalWinRTApplication.Current.RootFrame;
        }
    }
}
