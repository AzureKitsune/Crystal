using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalSpeedLaunchTest.ViewModel
{
    public class MainPageViewModel: Crystal2.Model.WinRTBusyViewModelBase
    {
        public override void OnNavigatedTo(object parameter, Crystal2.Navigation.CrystalNavigationEventArgs args)
        {
            Stopwatch stopWatch = parameter as Stopwatch;
            stopWatch.Stop();

            LaunchTime = stopWatch.Elapsed;
            RaisePropertyChanged("LaunchTime");
        }

        public TimeSpan LaunchTime { get; set; }
    }
}
