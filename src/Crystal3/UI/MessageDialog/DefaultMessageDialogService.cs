using Crystal3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.UI.MessageDialog
{
    public class DefaultMessageDialogService : IMessageDialogService
    {
        public object Show(string message = "", string title = "Message")
        {
            throw new NotImplementedException();
        }

        public async Task<object> ShowAsync(string message, string title = "Message")
        {
            await CrystalApplication.Dispatcher.RunAsync(() =>
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(message, title);
                md.Options = Windows.UI.Popups.MessageDialogOptions.None;

                return md.ShowAsync();
            });

            return null;
        }
    }
}
