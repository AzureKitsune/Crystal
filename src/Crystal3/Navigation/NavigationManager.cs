using Crystal3.Model;
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
    public class NavigationManager
    {
        #region fields
        /// <summary>
        /// <ViewModelType, PageType>
        /// </summary>
        private Dictionary<Type, Type> viewModelViewMappings = new Dictionary<Type, Type>();
        private List<NavigationService> navigationServices = new List<NavigationService>();
        #endregion

        #region properties
        public NavigationService RootNavigationService { get; internal set; }

        #endregion

        internal NavigationManager() { }

        internal void ProbeForViewViewModelPairs()
        {
            viewModelViewMappings.Clear();

            foreach (TypeInfo type in CrystalApplication.Current.GetType().GetTypeInfo().Assembly.DefinedTypes.Where(x =>
                                 x.IsSubclassOf(typeof(Page)) &&
                                 x.CustomAttributes.Any(y => y.AttributeType == typeof(NavigationViewModelAttribute))))
            {

                if (type.IsSubclassOf(typeof(Page)))
                {
                    var linkAttribute = type.CustomAttributes.First(y => y.AttributeType == typeof(NavigationViewModelAttribute));

                    var viewModelType = (Type)linkAttribute.ConstructorArguments.FirstOrDefault(x => ((Type)x.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).Value;
                    if (viewModelType == null)
                        viewModelType = (Type)linkAttribute.NamedArguments.First(x => ((Type)x.TypedValue.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).TypedValue.Value;

                    //var isHomePageInfo = linkAttribute.NamedArguments.FirstOrDefault(x => x.MemberName == "IsHome");
                    //bool isHomePage = false;

                    //if (isHomePageInfo.TypedValue.Value != null)
                    //    isHomePage = (bool)isHomePageInfo.TypedValue.Value;

                    //if (navigablePages.Any(x => ((Tuple<Type, Uri, bool>)x.Value).Item3 == true) && isHomePage)
                    //    throw new Exception("Only one home page is allowed.");

                    viewModelViewMappings.Add(viewModelType, type.AsType());

                }
            }
        }

        internal Type GetViewModel(Type viewType)
        {
            return (Type)viewModelViewMappings.First(x => x.Value == viewType).Key;
        }

        internal Type GetView(Type viewModelType)
        {
            return (Type)viewModelViewMappings[viewModelType];
        }

        internal void RegisterNavigationService(NavigationService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            if (RootNavigationService != null && service.NavigationLevel == FrameLevel.One)
            {
                throw new Exception("There can only be one level-one navigation service.");
            }

            if (!navigationServices.Contains(service))
                navigationServices.Add(service);
        }

        public void RegisterFrameAsNavigationService(Frame frame, FrameLevel frameLevel = FrameLevel.Two)
        {
            if (RootNavigationService != null && frameLevel == FrameLevel.One)
            {
                throw new Exception("There can only be one level-one navigation service.");
            }

            if (navigationServices.Any(x => x.NavigationLevel == frameLevel))
                throw new Exception();

            navigationServices.Add(new NavigationService(frame, this, frameLevel));
        }

        public NavigationService GetNavigationServiceFromFrameLevel(FrameLevel level = FrameLevel.One)
        {
            var service = navigationServices.First<NavigationService>(x => x.NavigationLevel == level);
            return service;
        }

        internal IEnumerable<NavigationService> GetAllServices()
        {
            List<NavigationService> services = new List<NavigationService>();

            //services.Add(RootNavigationService); - no longer needed if every navigationservice registers itself in the constructor

            services.AddRange(navigationServices);

            return services;
        }
    }
}
