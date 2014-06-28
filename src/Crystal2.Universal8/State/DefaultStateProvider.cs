using Crystal2.IOC;
using Crystal2.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;

namespace Crystal2.State
{
    internal class DefaultStateProvider : IStateProvider
    {
        private StateObject _state = null;

        public DefaultStateProvider()
        {
            IoCManager.Resolve<Navigation.INavigationProvider>().Navigated += DefaultStateProvider_Navigated;

            HandleFileState();

            if (_state == null)
                _state = new StateObject();
        }

        private static async void HandleFileState()
        {
            bool exists = false;
            try
            {
                await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("_state.xml");

                exists = true;
            }
            catch (Exception)
            {
            }

            if (!exists)
                await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("_state.xml");
        }

        void DefaultStateProvider_Navigated(object sender, Navigation.CrystalNavigationEventArgs e)
        {
            //TODO find workaround for the exception which is thrown because Frames do not support getting the navigational state string for pages that are navigated to with a parameter.
            //Currently, EVERY page is navigated to with a parameter in Crystal
            try
            {
                _state.NavigationState = IoCManager.Resolve<INavigationProvider>().GetNavigationContext() as string;
            }
            catch (Exception)
            {

            }
        }

        public async Task LoadStateAsync()
        {
            try
            {
                var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("_state.xml");
                var fileStr = await file.OpenAsync(FileAccessMode.ReadWrite);

                using (var str = fileStr.AsStreamForWrite())
                {
                    var serializer = new XmlSerializer(typeof(StateObject));

                    _state = (StateObject)await Task.Run<StateObject>(() =>
                        (StateObject)serializer.Deserialize(str));
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task SaveStateAsync()
        {
            if (State == null) return;

            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("_state.xml");
            var fileStr = await file.OpenAsync(FileAccessMode.ReadWrite);

            using (var str = fileStr.AsStreamForWrite())
            {
                var serializer = new XmlSerializer(typeof(StateObject));

                await Task.Run(() =>
                    serializer.Serialize(str, State));
            }

            await fileStr.FlushAsync();

            fileStr.Dispose();
        }

        public StateObject State
        {
            get { return _state; }
        }
    }
}
