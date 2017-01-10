using Crystal3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Crystal3.UI.MessageDialog
{
    public class DefaultMessageDialogService : IMessageDialogService
    {
        public object Show(string message = "", string title = "Title")
        {
            throw new NotImplementedException();
        }

        public async Task<object> ShowAsync(string message, string title = "Title")
        {
            await CrystalApplication.Dispatcher.RunAsync(() =>
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(message, title);
                md.Options = Windows.UI.Popups.MessageDialogOptions.None;

                return md.ShowAsync();
            });

            return null;
        }

        public async Task<IUICommand> AskYesOrNoAsync(string message, string title, UICommand yesCommand, UICommand noCommand)
        {
            return await await CrystalApplication.Dispatcher.RunAsync(() =>
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(message, title);
                md.Commands.Add(yesCommand);
                md.Commands.Add(noCommand);

                md.CancelCommandIndex = 1;

                return md.ShowAsync();
            });
        }

        public async Task<bool> AskYesOrNoAsync(string message, string title)
        {
            Windows.UI.Popups.UICommand yesCommand = new Windows.UI.Popups.UICommand();
            yesCommand.Label = "Yes";
            Windows.UI.Popups.UICommand noCommand = new Windows.UI.Popups.UICommand();
            noCommand.Label = "No";

            return await AskYesOrNoAsync(message, title, yesCommand, noCommand) == yesCommand;
        }
    }
}
