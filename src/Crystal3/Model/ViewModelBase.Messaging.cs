using Crystal3.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Model
{
    public partial class ViewModelBase: IMessagingTarget
    {
        #region Messaging
        //A list of subscriptions.
        private List<MessagingTicket> ticketList = new List<MessagingTicket>();
        /// <summary>
        /// Subscribes to the messager for a certain message.
        /// </summary>
        /// <param name="messageName">The name of the message to subscribe for.</param>
        /// <param name="callback">The callback that should be fired when the message is received.</param>
        /// <returns></returns>
        protected Messaging.MessagingTicket SubscribeToMessage(string messageName, Action<object, Action<object>> callback)
        {
            //Checks if the message name is valid.
            if (string.IsNullOrWhiteSpace(messageName)) throw new ArgumentNullException("messageName");

            //Check if this view model already has a subscription for this message.
            if (ticketList.Any(x => x.Name == messageName))
                throw new Exception("You are already subscribed to this message.");

            //Lets the Messenger know we exist if we haven't already.
            if (!Messenger.IsTarget((IMessagingTarget)this))
                Messenger.AddTarget(this);

            //Create the ticke (representing our subscription) with the message name/id and the callback.
            var ticket = new Messaging.MessagingTicket(messageName, callback);

            //Add the ticket/subscription to our own list.
            ticketList.Add(ticket);

            //Return the ticket/subscription.
            return ticket;
        }
        /// <summary>
        /// Unsubscribes from the Messenger using a ticket (subscription)
        /// </summary>
        /// <param name="ticket">The ticket (subscription) to use to unsubscribe.</param>
        protected void UnsubscribeToMessage(MessagingTicket ticket)
        {
            //Removes the ticket from our list.
            ticketList.Remove(ticket);

            //If we no longer have any subscriptions, remove ourself from the Messenger.
            if (ticketList.Count == 0)
            {
                if (Messenger.IsTarget((IMessagingTarget)this))
                    Messenger.RemoveTarget(this);
            }
        }

        /// <summary>
        /// Not to be called from your code.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="resultCallback"></param>
        public void OnReceivedMessage(Message message, Action<object> resultCallback)
        {
            foreach (MessagingTicket ticket in ticketList)
                if (ticket.Name == message.Name)
                    ticket.Callback.Invoke(message, resultCallback);
        }

        /// <summary>
        /// Returns our subscriptions' names.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetSubscriptions()
        {
            return ticketList.Select(x => x.Name);
        }
        #endregion
    }
}
