using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Messaging
{
    public static class Messenger
    {
        private static List<IMessagingTarget> targetList = new List<IMessagingTarget>();

        [DebuggerNonUserCode]
        public static Task<object> SendMessageAsync(Type target, string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            return SendMessageAsync(target, new Message(name, value));
        }

        public static Task<object> SendMessageAsync(Type target, Message message)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (message == null) throw new ArgumentNullException("message");

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            Task.Run(() =>
                {
                    foreach (var item in targetList)
                    {
                        if (item is IMessagingTarget)
                        {
                            if (item.GetSubscriptions().Contains(message.Name))
                            {
                                ((IMessagingTarget)item).OnReceivedMessage(message, (result) =>
                                {
                                    taskCompletionSource.TrySetResult(result);
                                });
                            }
                        }
                    }
                });

            return taskCompletionSource.Task;
        }

        public static void AddTarget(IMessagingTarget target)
        {
            if (target == null) throw new ArgumentNullException("target");

            targetList.Add(target);
        }

        public static bool IsTarget(IMessagingTarget target)
        {
            if (target == null) throw new ArgumentNullException("target");

            return targetList.Any(x => x == target);
        }

        public static void RemoveTarget(IMessagingTarget target)
        {
            if (target == null) throw new ArgumentNullException("target");

            targetList.Remove(target);
        }
    }
}
