using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Context;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Crystal3.Navigation
{
    public static class NavigationManager
    {
        #region fields
        /// <summary>
        /// <ViewModelType, PageType>
        /// </summary>
        private static Dictionary<Type, Type> viewModelViewMappings = new Dictionary<Type, Type>();
        #endregion

        #region properties
        public static NavigationService RootNavigationService { get; internal set; }

        #endregion

        internal static void ProbeForViewViewModelPairs()
        {
            viewModelViewMappings.Clear();

            foreach(TypeInfo type in typeof(CrystalApplication).GetTypeInfo().Assembly.DefinedTypes.Where(x =>
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

        internal static Type GetView(Type viewModelType)
        {
            return (Type)viewModelViewMappings[viewModelType];
        }
    }
}
