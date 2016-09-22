using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Crystal3.Navigation
{
    public static class FragmentManager
    {
        private static Dictionary<Type, Type> fragmentViewList = new Dictionary<Type, Type>();

        public static void RegisterFragmentView<T, V>() where V : FrameworkElement where T : ViewModelFragment
        {
            if (!fragmentViewList.ContainsKey(typeof(T)))
                fragmentViewList.Add(typeof(T), typeof(V));
        }

        public static void UnregisterFragmentView<T, V>() where V : FrameworkElement where T : ViewModelFragment
        {
            if (fragmentViewList.ContainsKey(typeof(T)))
                fragmentViewList.Remove(typeof(T));
        }

        public static Type ResolveFragmentView<T>() where T : ViewModelFragment
        {
            return ResolveFragmentView(typeof(T));
        }
        public static Type ResolveFragmentView(Type fragmentType)
        {
            if (fragmentViewList.ContainsKey(fragmentType))
                return fragmentViewList[fragmentType] as Type;
            else
                return null;
        }
    }
}
