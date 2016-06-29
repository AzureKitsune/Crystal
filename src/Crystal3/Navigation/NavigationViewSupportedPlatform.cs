using System;

namespace Crystal3.Navigation
{
    [Flags]
    public enum NavigationViewSupportedPlatform
    {
        All = Desktop | Mobile | Xbox | Holographic | SurfaceHub | IoT,
        Desktop = 1,
        Mobile = 2,
        Xbox = 4,
        Holographic = 8,
        SurfaceHub = 16,
        IoT = 32
    }
}