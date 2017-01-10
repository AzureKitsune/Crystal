using Crystal3.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Crystal3.Core
{
    public interface IMessageDialogService: IIoCObject
    {
        object Show(string message = "", string title = "Title");
        Task<object> ShowAsync(string message, string title = "Title");
        Task<IUICommand> AskYesOrNoAsync(string message, string title, UICommand yesCommand, UICommand noCommand);
        Task<bool> AskYesOrNoAsync(string message, string title);
    }
}
