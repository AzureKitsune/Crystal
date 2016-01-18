using System;

namespace Crystal3.Navigation
{
    public class NavigationManagerPreBackRequestedEventArgs: EventArgs
    {
        public bool Handled { get; set; }
    }
}