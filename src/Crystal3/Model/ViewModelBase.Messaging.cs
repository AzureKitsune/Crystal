using Crystal3.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    public partial class ViewModelBase
    {
        #region Messaging
        private List<MessagingTicket> ticketList = new List<MessagingTicket>();
        protected Messaging.MessagingTicket SubscribeToMessage(string messageName, Action<object, Action<object>> callback)
        {
            if (ticketList.Any(x => x.Name == messageName))
                throw new Exception("You are already subscribed to this message.");

            if (string.IsNullOrWhiteSpace(messageName)) throw new ArgumentNullException("messageName");

            if (!Messenger.IsTarget((IMessagingTarget)this))
                Messenger.AddTarget(this);

            var ticket = new Messaging.MessagingTicket(messageName, callback);

            ticketList.Add(ticket);

            return ticket;
        }
        protected void UnsubscribeToMessage(MessagingTicket ticket)
        {
            ticketList.Remove(ticket);

            if (ticketList.Count == 0)
                if (Messenger.IsTarget((IMessagingTarget)this))
                    Messenger.RemoveTarget(this);
        }

        public void OnReceivedMessage(Message message, Action<object> resultCallback)
        {
            foreach (MessagingTicket ticket in ticketList)
                if (ticket.Name == message.Name)
                    ticket.Callback.Invoke(message, resultCallback);
        }

        public IEnumerable<string> GetSubscriptions()
        {
            foreach (var ticket in ticketList)
                yield return ticket.Name;
        }
        #endregion
    }
}
