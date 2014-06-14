using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;

namespace Crystal2.Navigation
{
    public class PhoneBackButtonNavigationProvider: IBackButtonNavigationProvider
    {
        public void Attach(CrystalWinRTApplication app)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (NavigationManager.CanGoBack)
            {
                NavigationManager.GoBackward();

                e.Handled = true;
            }
        }
    }
}
