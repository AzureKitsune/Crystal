using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Crystal2.Actions
{
    public class UIEventTrigger : DependencyObject, IUITrigger
    {
        public UIEventTrigger()
        {

        }

        private EventInfo eventInfo = null;
        private Delegate eventDelegateHandler = null;

        private UIElement ParentControl { get; set; }


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(UIEventTrigger), new PropertyMetadata(null));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(UIEventTrigger), new PropertyMetadata("Loaded", new PropertyChangedCallback((obj, args) =>
        {
            UIEventTrigger trigger = (UIEventTrigger)obj;

            if (trigger.eventInfo != null && args.OldValue != null)
            {
                trigger.eventInfo.RemoveEventHandler(trigger.ParentControl, trigger.eventDelegateHandler);
            }

            if (trigger.ParentControl != null)
                trigger.DoAddEvent((string)args.NewValue);
        })));

        private void DoAddEvent(string eventName)
        {
            if (!string.IsNullOrWhiteSpace(eventName))
            {

                //new Windows.UI.Xaml.Controls.ListView().SelectionChanged


                eventInfo = ParentControl.GetType().GetTypeInfo().GetDeclaredEvent(eventName);
                if (eventInfo == null)
                    eventInfo = ParentControl.GetType().GetTypeInfo().GetDeclaredEvent(eventName + "Event");
                if (eventInfo == null)
                    eventInfo = ParentControl.GetType().GetRuntimeEvent(eventName);


                var delegateType = eventInfo.EventHandlerType;

                eventDelegateHandler = new Action<object, EventArgs>((o, e) =>
                {
                    if (Command != null)
                        Command.Execute(null);
                });

                if (delegateType == typeof(RoutedEventHandler) || delegateType.Namespace.StartsWith("Windows.UI.Xaml.Controls"))
                {

                    //http://www.camronbute.com/2012/06/binding-to-event-at-runtime-in-windows.html
                    Func<RoutedEventHandler, EventRegistrationToken> add = (a) =>
                    {
                        return (EventRegistrationToken)eventInfo.AddMethod.Invoke(ParentControl, new object[] { a });
                    };
                    Action<EventRegistrationToken> remove = (a) => { eventInfo.RemoveMethod.Invoke(eventInfo, new object[] { a }); };
                    //RoutedEventHandler handler = (a, b) => Command.Execute(b);

                    //WindowsRuntimeMarshal.AddEventHandler<RoutedEventHandler>(add, remove, (dynamic)eventDelegateHandler);
                }
                else
                {
                    eventInfo.AddEventHandler(ParentControl, eventDelegateHandler);
                }
            }
        }

        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        public void OnAttach(object control)
        {
            ParentControl = control as UIElement;
            DoAddEvent(EventName);
        }


        public void OnDetach(object control)
        {

        }
    }
}
