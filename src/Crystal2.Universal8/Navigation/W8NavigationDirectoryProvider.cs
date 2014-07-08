﻿using Crystal2.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Crystal2.Navigation
{
    public class W8NavigationDirectoryProvider : INavigationDirectoryProvider
    {
        private Assembly executingAssembly = null;
        private Lazy<ReadOnlyDictionary<Type, object>> lazyMap = null;
        private bool autoProbe = false;
        private Dictionary<Type, object> navigablePages = new Dictionary<Type, object>();

        internal W8NavigationDirectoryProvider(Assembly ExecutingAssembly, bool shouldProbe)
        {
            executingAssembly = ExecutingAssembly;
            autoProbe = shouldProbe;
        }
        public ReadOnlyDictionary<Type, object> ProvideMap()
        {
            if (lazyMap == null)
            {
                lazyMap = new Lazy<ReadOnlyDictionary<Type, object>>(() =>
                {
                    if (autoProbe)
                    {
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

                            var uri = GetUriFromPageType(page);

                            navigablePages.Add((Type)(object)viewModelType, (object)new Tuple<Type, Uri>(page.AsType(), uri));
                        }
                    }

                    return new ReadOnlyDictionary<Type, object>(navigablePages);
                });
            }

            return lazyMap.Value;
        }

        private static Uri GetUriFromPageType(TypeInfo page)
        {
            var url = Uri.EscapeDataString(page.FullName);
            var uri = new Uri("/" + url.Substring(url.IndexOf(".") + 1).Replace(".", "/") + ".xaml", UriKind.Relative); //just try to guess the uri;
            return uri;
        }

        public void AddViewModelViewPair(Type viewModel, Type pageType)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            if (pageType == null) throw new ArgumentNullException("pageType");

            var uri = GetUriFromPageType(pageType.GetTypeInfo());

            navigablePages.Add(viewModel, new Tuple<Type, Uri>(pageType, uri));
        }
        //internal void AddViewModelUriPair(ViewModelBase viewModel, Uri uri)
        //{
        //    if (viewModel == null) throw new ArgumentNullException("viewModel");
        //    if (uri == null) throw new ArgumentNullException("uri");

        //    AddViewModelUriPair(viewModel.GetType(), uri);
        //}
    }
}
