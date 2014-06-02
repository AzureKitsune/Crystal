using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Navigation
{
    internal interface INavigatingViewModel
    {
        void OnNavigatingTo();
        void OnNavigatedTo(object paramater);

        bool OnNavigatingFrom();
        void OnNavigatedFrom();

        void OnRefresh();
    }
}
