﻿using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Crystal3.Navigation
{
    public class NavigationService
    {
        //TODO pass CrystalNavigationEventArgs instead of the built-in WinRT event args

        public Frame NavigationFrame { get; private set; }
        public FrameLevel NavigationLevel { get; private set; }
        internal NavigationManager NavigationManager { get; set; }

        public bool CanGoBackward { get { return NavigationFrame.CanGoBack; } }

        public void GoBack() { NavigationFrame.GoBack(); }

        internal NavigationService(Frame navFrame, NavigationManager manager)
        {
            if (navFrame == null) throw new ArgumentNullException("navFrame");

            NavigationManager = manager;
            NavigationFrame = navFrame;
            NavigationFrame.DataContext = null;

            NavigationManager.RegisterNavigationService(this);

            NavigationFrame.Navigating += NavigationFrame_Navigating;
            NavigationFrame.Navigated += NavigationFrame_Navigated;
        }

        private void NavigationFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            
        }

        private void NavigationFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back && ((Page)(e.Content)).DataContext is ViewModelBase)
            {
                //so the following line seems to point out a possible bug. when using inline navigation, the inline-page's datacontext reverts to the datacontext of the frame's parent.
                //... mo-code, mo-problems
                var viewModel = ((Page)(e.Content)).DataContext as ViewModelBase;

                try
                {
                    if (NavigationServicePreNavigatedSignaled != null)
                        NavigationServicePreNavigatedSignaled(this, new NavigationServicePreNavigatedSignaledEventArgs(viewModel, e));
                }
                catch (Exception) { }

                viewModel.OnNavigatedFrom(sender, e);
            }
        }

        internal NavigationService(Frame navFrame, NavigationManager manager, FrameLevel navigationLevel) : this(navFrame, manager)
        {
            NavigationLevel = navigationLevel;
        }

        public void NavigateTo<T>(object parameter = null) where T : ViewModelBase
        {
            var view = NavigationManager.GetView(typeof(T));

            if (view == null) throw new Exception("View not found!");

            ViewModelBase viewModel = Activator.CreateInstance(typeof(T)) as ViewModelBase;
            viewModel.NavigationService = this;

            NavigatingCancelEventHandler navigatingHandler = null;
            navigatingHandler = new NavigatingCancelEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e) =>
            {
                NavigationFrame.Navigating -= navigatingHandler;

                if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Forward)
                {
                    e.Cancel = viewModel.OnNavigatingTo(sender, e);
                }
                else if (e.NavigationMode == NavigationMode.Back)
                {
                    e.Cancel = viewModel.OnNavigatingFrom(sender, e);
                }
            });

            NavigatedEventHandler navigatedHandler = null;
            navigatedHandler = new NavigatedEventHandler((object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e) =>
            {
                NavigationFrame.Navigated -= navigatedHandler;

                try
                {
                    if (NavigationServicePreNavigatedSignaled != null)
                        NavigationServicePreNavigatedSignaled(this, new NavigationServicePreNavigatedSignaledEventArgs(viewModel, e));
                }
                catch (Exception) { }

                if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Forward)
                {
                    Page page = e.Content as Page;
                    page.DataContext = viewModel;

                    viewModel.OnNavigatedTo(sender, e);
                }
                else if (e.NavigationMode == NavigationMode.Back)
                {
                    viewModel.OnNavigatedFrom(sender, e);
                }
            });


            NavigationFrame.Navigated += navigatedHandler;
            NavigationFrame.Navigating += navigatingHandler;

            NavigationFrame.Navigate(view, parameter);
        }

        /// <summary>
        /// A workaround for .NET event's first-subscribe, last-fire approach.
        /// </summary>
        public event EventHandler<NavigationServicePreNavigatedSignaledEventArgs> NavigationServicePreNavigatedSignaled;
    }
}
