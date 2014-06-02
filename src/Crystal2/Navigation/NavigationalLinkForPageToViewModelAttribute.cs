using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Navigation
{
    /// <summary>
    /// Links a page or window to a view model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NavigationalLinkForPageToViewModelAttribute: Attribute
    {
        public NavigationalLinkForPageToViewModelAttribute(Type viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");

            ViewModel = viewModel;
        }
        public Type ViewModel { get; set; }
    }
}
