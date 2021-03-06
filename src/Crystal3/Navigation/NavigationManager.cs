﻿using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Context;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Crystal3.Navigation
{
    /// <summary>
    /// A class that manages all navigation services in the current Window.
    /// </summary>
    public class NavigationManager
    {
        #region fields
        /// <summary>
        /// <ViewModelType, PageType>
        /// </summary>
        private static List<NavigationManagerViewMapping> viewModelViewMappings = new List<NavigationManagerViewMapping>();
        private List<NavigationServiceBase> navigationServices = new List<NavigationServiceBase>();
        #endregion

        #region properties
        /// <summary>
        /// Returns the NavigationService for this Window that has a FrameLevel of One.
        /// </summary>
        public NavigationServiceBase RootNavigationService { get; internal set; }
        /// <summary>
        /// Represent the instance of Application (CrystalApplication) that was initialized when the Application started.
        /// </summary>
        internal CrystalApplication AppInstance { get; set; }

        #endregion

        public class NavigationManagerViewMapping
        {
            public Type ViewType { get; internal set; }
            public bool UseDataContextInsteadOfCreating { get; internal set; }
            public Type ViewModelType { get; internal set; }
        }

        internal NavigationManager(CrystalApplication appInstance)
        {
            if (appInstance == null) throw new ArgumentNullException("appInstance");

            AppInstance = appInstance;
        }

        /// <summary>
        /// Probes for Views that have the NavigationViewModelAttribute and links them to their corresponding View Model.
        /// </summary>
        internal void ProbeForViewViewModelPairs()
        {
            viewModelViewMappings.Clear();

            foreach (TypeInfo type in AppInstance.GetType().GetTypeInfo().Assembly.DefinedTypes.Where(x =>
                        x.IsSubclassOf(typeof(Page)) &&
                        x.CustomAttributes.Any(y =>
                        y.AttributeType == typeof(NavigationViewModelAttribute))))
            {

                if (type.IsSubclassOf(typeof(Page)))
                {
                    var linkAttribute = type.CustomAttributes.First(y => y.AttributeType == typeof(NavigationViewModelAttribute));

                    var viewModelType = (Type)linkAttribute.ConstructorArguments.FirstOrDefault(x => ((Type)x.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).Value;
                    if (viewModelType == null)
                        viewModelType = (Type)linkAttribute.NamedArguments.First(x => ((Type)x.TypedValue.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).TypedValue.Value;

                    var platformType = (NavigationViewSupportedPlatform)linkAttribute.ConstructorArguments[1].Value;
                    var useDataContextInsteadOfCreating = (bool)linkAttribute.ConstructorArguments[2].Value;

                    //var isHomePageInfo = linkAttribute.NamedArguments.FirstOrDefault(x => x.MemberName == "IsHome");
                    //bool isHomePage = false;

                    //if (isHomePageInfo.TypedValue.Value != null)
                    //    isHomePage = (bool)isHomePageInfo.TypedValue.Value;

                    //if (navigablePages.Any(x => ((Tuple<Type, Uri, bool>)x.Value).Item3 == true) && isHomePage)
                    //    throw new Exception("Only one home page is allowed.");


                    if (CurrentPlatformSupportsView(platformType))
                    {
                        viewModelViewMappings.Add(new NavigationManagerViewMapping()
                        {
                            ViewModelType = viewModelType,
                            ViewType = type.AsType(),
                            UseDataContextInsteadOfCreating = useDataContextInsteadOfCreating
                        });
                    }

                }
            }
        }

        /// <summary>
        /// Returns an object containing information relating a view model to its corresponding view.
        /// </summary>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <returns></returns>
        public NavigationManagerViewMapping GetViewModelInfo(Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));

            return viewModelViewMappings.FirstOrDefault(x => x.ViewModelType == viewModelType);
        }

        /// <summary>
        /// Returns the type of View Model associated with a View.
        /// </summary>
        /// <param name="viewType">The type of View.</param>
        /// <returns></returns>
        internal Type GetViewModelType(Type viewType)
        {
            return (Type)viewModelViewMappings.First(x => x.ViewType == viewType).ViewModelType;
        }
        /// <summary>
        /// Returns the type of View associated with a View Model.
        /// </summary>
        /// <param name="viewModelType">The type of View Model.</param>
        /// <returns></returns>
        internal Type GetViewType(Type viewModelType)
        {
            //Depending on what the user chose, handle resolving the view type accordingly.

            if (AppInstance.Options.NavigationRoutingMethod == NavigationRoutingMethod.Dynamic || viewModelViewMappings.Any(x=> x.ViewModelType == viewModelType))
            {
                //Return the type that we got from probing.
                return (Type)viewModelViewMappings.FirstOrDefault(x => x.ViewModelType == viewModelType)?.ViewType;
            }
            else
            {
                //Call a virtual void implemented by the user and have them return the type of view to show.
                //Override this process if you want to show a different view based on the platform.
                var pageType = AppInstance.ResolveStaticPageType(viewModelType);

                if (!viewModelViewMappings.Any(x=> x.ViewModelType == viewModelType))
                {
                    viewModelViewMappings.Add(new NavigationManagerViewMapping()
                    {
                        ViewModelType = viewModelType,
                        ViewType = pageType
                    });
                }

                return pageType;
            }
        }

        /// <summary>
        /// Registers the NavigationService with the NavigationManager.
        /// </summary>
        /// <param name="service">The NavigationService to be registered.</param>
        internal void RegisterNavigationService(NavigationServiceBase service)
        {
            //Null check.
            if (service == null) throw new ArgumentNullException("service");

            //Check if the user is trying to register a top-level Navigation Service.
            if (RootNavigationService != null && service.NavigationLevel == FrameLevel.One)
            {
                throw new Exception("There can only be one level-one navigation service.");
            }

            //If it isn't already in the list, add it.
            if (!navigationServices.Contains(service))
                navigationServices.Add(service);
        }

        public void RegisterCustomNavigationService(NavigationServiceBase service, FrameLevel frameLevel = FrameLevel.Two)
        {
            //Check if the user is trying to register a top-level Navigation Service.
            if (RootNavigationService != null && frameLevel == FrameLevel.One)
            {
                throw new Exception("There can only be one level-one navigation service.");
            }

            //If it isn't already in the list, add it.
            if (navigationServices.Any(x => x.NavigationLevel == frameLevel))
                throw new Exception();

            if (service == null)
                throw new ArgumentNullException(nameof(service));

            service.NavigationLevel = frameLevel;
            service.NavigationManager = this;

            navigationServices.Add(service);
        }

        /// <summary>
        /// Registers a Frame object as a NavigationService and returns the instance of the new service.
        /// </summary>
        /// <param name="frame">The frame to be used to create the service.</param>
        /// <param name="frameLevel">The frame level of the new service.</param>
        /// <returns></returns>
        public FrameNavigationService RegisterFrameAsNavigationService(Frame frame, FrameLevel frameLevel = FrameLevel.Two)
        {
            //Check if the user is trying to register a top-level Navigation Service.
            if (RootNavigationService != null && frameLevel == FrameLevel.One)
            {
                throw new Exception("There can only be one level-one navigation service.");
            }

            //If it isn't already in the list, add it.
            if (navigationServices.Any(x => x.NavigationLevel == frameLevel))
                throw new Exception();

            //Creates the new service.
            var service = new FrameNavigationService(frame, this, frameLevel);

            //navigationServices.Add(service);

            //Returns the new service for the user to use.
            return service;
        }

        /// <summary>
        /// Removes all NavigationServices with the specified FrameLevel.
        /// </summary>
        /// <param name="frameLevel">The FrameLevel used to remove the services.</param>
        public void UnregisterNavigationServiceByFrameLevel(FrameLevel frameLevel)
        {
            navigationServices.RemoveAll(x => x.NavigationLevel == frameLevel);
        }

        /// <summary>
        /// Returns the corresponding NavigationService based on the FrameLevel provided.
        /// </summary>
        /// <param name="level">The FrameLevel of the service to return.</param>
        /// <returns></returns>
        public NavigationServiceBase GetNavigationServiceFromFrameLevel(FrameLevel level = FrameLevel.One)
        {
            var service = navigationServices.FirstOrDefault<NavigationServiceBase>(x => x.NavigationLevel == level);
            return service;
        }

        internal IEnumerable<NavigationServiceBase> GetAllServices()
        {
            return navigationServices;
        }

        internal IEnumerable<ViewModelBase> GetAllNavigatedViewModels()
        {
            List<ViewModelBase> viewModelsInWindow = new List<ViewModelBase>();

            viewModelsInWindow.AddRange(GetAllServices().Select(x => x.GetNavigatedViewModel()).Distinct());

            return viewModelsInWindow;
        }


        private bool CurrentPlatformSupportsView(NavigationViewSupportedPlatform platformType)
        {
            var platform = DeviceInformation.GetDevicePlatform();

            NavigationViewSupportedPlatform convertedNavPlatform = NavigationViewSupportedPlatform.All;
            switch(platform)
            {
                case Core.Platform.Desktop:
                    convertedNavPlatform = NavigationViewSupportedPlatform.Desktop;
                    break;
                case Core.Platform.Holographic:
                    convertedNavPlatform = NavigationViewSupportedPlatform.Holographic;
                    break;
                case Core.Platform.Mobile:
                    convertedNavPlatform = NavigationViewSupportedPlatform.Mobile;
                    break;
                case Core.Platform.IoT:
                    convertedNavPlatform = NavigationViewSupportedPlatform.IoT;
                    break;
                case Core.Platform.Xbox:
                    convertedNavPlatform = NavigationViewSupportedPlatform.Xbox;
                    break;
                case Core.Platform.Team:
                    convertedNavPlatform = NavigationViewSupportedPlatform.Team;
                    break;
                default:
                    throw new Exception();
            }

            //cool bitwise method http://stackoverflow.com/a/18001375

            return (platformType & convertedNavPlatform) > 0;
        }
    }
}
