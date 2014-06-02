using Crystal2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Crystal2.Navigation
{
    class W8NavigationDirectoryProvider : INavigationDirectoryProvider
    {
        private Assembly executingAssembly = null;
        private Lazy<Dictionary<Type, object>> lazyMap = null;
        public W8NavigationDirectoryProvider(Assembly ExecutingAssembly)
        {
            executingAssembly = ExecutingAssembly;
        }
        public Dictionary<Type, object> ProvideMap()
        {
            if (lazyMap == null)
            {
                lazyMap = new Lazy<Dictionary<Type, object>>(() =>
                {
                    Dictionary<Type, object> navigablePages = new Dictionary<Type, object>();

                    var navigablePageTypes = executingAssembly.DefinedTypes
                        .Where(x =>
                            x.IsSubclassOf(typeof(Page)) &&
                            x.CustomAttributes.Any(y => y.AttributeType == typeof(NavigationalLinkForPageToViewModelAttribute)));

                    foreach (var page in navigablePageTypes)
                    {
                        var linkAttribute = page.CustomAttributes.First(y => y.AttributeType == typeof(NavigationalLinkForPageToViewModelAttribute));

                        var viewModelType = (Type)linkAttribute.ConstructorArguments.FirstOrDefault(x => ((Type)x.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).Value;
                        if (viewModelType == null)
                            viewModelType = (Type)linkAttribute.NamedArguments.First(x => ((Type)x.TypedValue.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).TypedValue.Value;

                        var url = Uri.EscapeDataString(page.FullName);
                        var uri = new Uri("/" + url.Substring(url.IndexOf(".") + 1).Replace(".", "/") + ".xaml", UriKind.Relative); //just try to guess the uri;

                        navigablePages.Add((Type)(object)viewModelType, (object)new Tuple<Type, Uri>(page.AsType(), uri));
                    }

                    return navigablePages;
                });
            }

            return lazyMap.Value;
        }
    }
}
