using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Crystal2.Actions
{
    public static class WinRTInteractionMonitor
    {
        //private object Parent = null;

        public static readonly DependencyProperty HasBeenAttachedProperty = DependencyProperty.RegisterAttached("HasBeenAttached", typeof(bool), 
            typeof(WinRTInteractionMonitor), new PropertyMetadata(false));
        public static bool GetHasBeenAttached(UIElement sender)
        {
            return (bool)sender.GetValue(HasBeenAttachedProperty);
        }
        public static void SetHasBeenAttached(UIElement sender, bool value)
        {
            sender.SetValue(HasBeenAttachedProperty, value);
        }

        public static readonly DependencyProperty TriggersProperty = DependencyProperty.RegisterAttached("Triggers", typeof(UITriggerCollection), 
            typeof(WinRTInteractionMonitor), new PropertyMetadata(new UITriggerCollection(),
            new PropertyChangedCallback(HandleTriggersChanged)));

        private static void HandleTriggersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UnhookOldTriggers((UIElement)d, (UITriggerCollection)e.OldValue);

            HookNewTriggers((UIElement)d, (UITriggerCollection)e.NewValue);
        }

        public static UITriggerCollection GetTriggers(UIElement sender)
        {
            //if (GetHasBeenAttached(sender))
            //{
            //    UnhookOldTriggers(sender);
            //    SetHasBeenAttached(sender, false);
            //}
            //if (!GetHasBeenAttached(sender))
            //{
            //    HookNewTriggers(sender, (UITriggerCollection)sender.GetValue(TriggersProperty));
            //}

            return (UITriggerCollection)sender.GetValue(TriggersProperty);
        }

        public static void SetTriggers(UIElement sender, UITriggerCollection value)
        {
            sender.SetValue(TriggersProperty, value);
        }

        private static void HookNewTriggers(UIElement sender, UITriggerCollection value)
        {
            foreach (IUITrigger trigger in value)
            {
                trigger.OnAttach(sender);
            }
        }

        private static void UnhookOldTriggers(UIElement sender, UITriggerCollection triggers = null)
        {
            var triggersToUnhook = triggers ?? (UITriggerCollection)GetTriggers(sender);

            if (GetTriggers(sender) != null)
            {
                foreach (IUITrigger trigger in triggersToUnhook)
                {
                    trigger.OnDetach(sender);
                }
            }
        }
    }
}
