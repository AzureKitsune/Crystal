using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.UI.MessageDialog
{
    internal class DefaultMessageDialogProvider : IMessageDialogProvider
    {
        public async Task<CrystalDialogYesNoMessageResult> ShowYesNoMessageDialogAsync(string message, string title = "", string yesString = "Yes", string noString = "No")
        {
            var md = new Windows.UI.Popups.MessageDialog("This operation will clear any data fields (including ones you have edited) and change them to what was detected. Are you should you want to do this?",
                    "Are you sure?");
            md.Commands.Add(new Windows.UI.Popups.UICommand(yesString, new Windows.UI.Popups.UICommandInvokedHandler(x => { }), "Yes"));
            md.Commands.Add(new Windows.UI.Popups.UICommand(noString, new Windows.UI.Popups.UICommandInvokedHandler(x => { }), "No"));

            var result = await md.ShowAsync();
            return (CrystalDialogYesNoMessageResult)Enum.Parse(typeof(CrystalDialogYesNoMessageResult), ((string)result.Id));
        }

        public Task ShowOkayMessageDialogAsync(string message, string title = "")
        {
            return new Windows.UI.Popups.MessageDialog(message, title).ShowAsync().AsTask();
        }
    }
}
