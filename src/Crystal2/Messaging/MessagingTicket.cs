using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Messaging
{
    public class MessagingTicket
    {
        internal Action<object, Action<object>> Callback { get; set; }

        public string Name { get; private set; }

        internal MessagingTicket(string name, Action<object, Action<object>> callback)
        {
            Name = name;
            Callback = callback;
        }
    }
}
