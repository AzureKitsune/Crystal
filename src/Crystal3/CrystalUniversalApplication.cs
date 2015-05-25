using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Crystal3
{
    public abstract class CrystalUniversalApplication: Application
    {
        public CrystalConfiguration Options { get; private set; }


        public CrystalUniversalApplication() : base()
        {
            Options = new CrystalConfiguration();


        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
        }
    }
}
