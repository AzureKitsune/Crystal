using System;

namespace Crystal3.Navigation
{
    /// <summary>
    /// EventArgs for handling just before the back button is pressed.
    /// </summary>
    public class NavigationManagerPreBackRequestedEventArgs: EventArgs
    {
        public bool Handled { get; set; }
    }
}