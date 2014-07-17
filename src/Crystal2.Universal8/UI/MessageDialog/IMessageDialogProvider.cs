using Crystal2.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.UI.MessageDialog
{
    public interface IMessageDialogProvider : IIoCObject
    {
        Task<CrystalDialogYesNoMessageResult> ShowYesNoMessageDialogAsync(string message, string title = "", string yesString = "Yes", string noString = "No");

        Task ShowOkayMessageDialogAsync(string message, string title = "");
    }
}
