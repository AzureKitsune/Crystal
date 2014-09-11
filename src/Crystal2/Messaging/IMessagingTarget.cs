using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal2.Messaging
{
    public interface IMessagingTarget
    {
        void OnReceivedMessage(Message message, Action<object> resultCallback);

        IEnumerable<string> GetSubscriptions();
    }
}
