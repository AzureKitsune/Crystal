### What is Crystal?
Crystal is a UWP application development framework I use for my own projects. It essentially a way for me to get started quickly and not have to get my hands dirty. Batteries are not included. Use at your own risk!

### Why Crystal?
Don't use Crystal! Seriously, don't do it!

### Seriously. Why? (Backstory)
I first wrote Crystal back during my days of WPF (Windows Presentation Foundation) as a way to jump into MVVM. I was a little too impatient to tutorials for the major libraries at the time so I decided to write my own. It would be fast and easy to, at least, for myself. Eventually, a [friend](https://github.com/lethalbit) of mine decided to use it in his application. From that point on, I began to use Crystal in all of my applications. You can view the beginning of this library if you look at the very beginning of the commit log from 2012.

Eventually I stumbled upon WP7 and Silverlight. From that point on, I rewrote Crystal as Crystal 2 and added support for both of those platforms. Unfortunately at the time, I didn't do very much with those platforms. I rewrote an old application named Hanasu to use it but thats it. Crystal 2 eventually supported WP8, then W8/W8.1 (WinRT) and by extension, WP8.1 (WinPRT). I created a few apps with this new support but never released them to the public.

Finally, with the invention of UWP, I decided to once again, rewrite the library to support the platform from the ground up.


### Features
Crystal provides support for MVVM, Navigation, Suspension/Resurrection (as per the Application Lifecycle), IoC, and Messaging among other miscellaneous goodies. 

- MVVM - Model View ViewModel
    - ViewModels are bound to a view based on platform (Desktop, Mobile, Xbox, Holographic, IoT and Surface) meaning you can easily customize your view based on platform. 
- Supports suspension and resurrection of your application out of the box. ViewModels are provided with lifecycle events accordingly.
- Contains a basic IoC (Inversion of Control) implementation. Theres one global IoC container and an additional one per viewmodel in your application.
    - Support for utilizing the application dispatcher is powered by IoC.
- Supports the Mobile statusbar.
- Basic RelayCommand implementation.
- Handles the Desktop Window Back Button for you.
- (Anniversary Update/Redstone 1) Supports single-process background activation with prelaunch support coming soon.
- Messaging between ViewModels
- Prelimenary Multi-Window support
- Various miscellaneous converters such as:
    - BooleanToVisbilityConverter
    - InverseBooleanToVisibilityConverter
    - BooleanToInverseBooleanConverter
    - RelativeTimeConverter
    - DateTimeToTimeStringConverter
- Other miscellaneous extension methods

### Tested Platforms
- Desktop - Works!
    - Mixed Reality headsets as well!
- Mobile - Works!
- Xbox - Works!
- Hololens (Holographic) - Not Tested
- Surface Hub - Not Tested
- IoT - Not Tested
