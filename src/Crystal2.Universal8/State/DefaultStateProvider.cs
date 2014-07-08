using Crystal2.IOC;
using Crystal2.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
                await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("_state.json");

                exists = true;
            }
            catch (Exception)
            {
            }

            if (!exists)
                await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("_state.json");
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
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("_state.json");
            try
            {
                
                var fileStr = await file.OpenAsync(FileAccessMode.ReadWrite);

                using (var str = fileStr.AsStreamForRead())
                {
                    var serializer = new DataContractJsonSerializer(typeof(StateObject));

                    _state = (StateObject)serializer.ReadObject(str);
                }
            }
            catch (Exception)
            {
            }

            await file.DeleteAsync();
        }

        public async Task SaveStateAsync()
        {
            if (State == null) return;

            StorageFile file = null;
            try { file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("_state.json"); }
            catch (Exception) { }
            if (file == null) file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("_state.json");

            var fileStr = await file.OpenAsync(FileAccessMode.ReadWrite);

            using (var str = fileStr.AsStreamForWrite())
            {
                var serializer = new DataContractJsonSerializer(typeof(StateObject));

                serializer.WriteObject(str, State);

                await str.FlushAsync();
            }
        }

        public StateObject State
        {
            get { return _state; }
        }
    }
}
