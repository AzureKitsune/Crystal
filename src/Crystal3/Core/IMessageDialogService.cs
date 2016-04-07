using Crystal3.InversionOfControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Core
{
    public interface IMessageDialogService: IIoCObject
    {
        object Show(string message = "", string title = "Title");
        Task<object> ShowAsync(string message, string title = "Title");
    }
}
