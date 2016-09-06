using Crystal3.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace Crystal3.Core
{
    internal static class PreservationManager
    {
        internal const string SuspensionStateFileName = "CrystalSuspensionState.xml";

        internal static async Task PreserveAsync(Task suspendingOp)
        {
            XDocument document = new XDocument();
            XElement windows = new XElement("Windows");

            foreach (var window in WindowManager.GetAllWindowServices())
            {
                XElement windowElement = new XElement("Window");
                try
                {
                    foreach (var navsrv in window.NavigationManager.GetAllServices())
                    {
                        var navsrvElement = new XElement("NavigationService");

                        var navLvlAttr = new XAttribute("NavigationLevel", navsrv.NavigationLevel);
                        navsrvElement.Add(navLvlAttr);

                        var naviState = new XElement("NavigationState", navsrv.NavigationFrame.GetNavigationState());
                        navsrvElement.Add(naviState);

                        var rootViewModel = navsrv.GetNavigatedViewModel();
                        if (rootViewModel != null)
                        {
                            var attr = new XAttribute("ViewModelType", rootViewModel.GetType().FullName);
                            navsrvElement.Add(attr);

                            XElement viewModelData = new XElement("ViewModelData");

                            Dictionary<string, object> dataDic = new Dictionary<string, object>();

                            await rootViewModel.OnPreservingAsync(dataDic);

                            foreach (var data in dataDic)
                            {
                                var keyValueElement = new XElement(data.Key, data.Value);
                                viewModelData.Add(keyValueElement);
                            }

                            navsrvElement.Add(viewModelData);
                        }

                        windowElement.Add(navsrvElement);
                    }
                    
                }
                catch (Exception)
                {

                }

                windows.Add(windowElement);
            }

            document.Add(windows);

            StorageFile file = await CrystalApplication.CrystalDataFolder.CreateFileAsync(SuspensionStateFileName, CreationCollisionOption.ReplaceExisting);
            var stream = (await file.OpenAsync(FileAccessMode.ReadWrite)).AsStream();
            document.Save(stream, SaveOptions.None);
            stream.Dispose();

            await suspendingOp;

        }
        internal static async Task<bool> RestoreAsync()
        {
            StorageFile file = await CrystalApplication.CrystalDataFolder.CreateFileAsync(SuspensionStateFileName, CreationCollisionOption.OpenIfExists);

            var instance = new CrystalRestoredApplicationInstance();

            await (CrystalApplication.Current as CrystalApplication).OnRestoringAsync();

            XDocument suspensionDoc = XDocument.Parse(await FileIO.ReadTextAsync(file));

            try
            {
                var windowsElement = suspensionDoc.Element("Windows");
                var windows = WindowManager.GetAllWindowServices();
                foreach (var windowElement in windowsElement.Elements("Window"))
                {
                    var window = windows.First();

                    foreach(var navisrvElement in windowElement.Elements("NavigationService")) //todo verify that these are in framelevel order
                    {
                        var navisrv = window.NavigationManager.GetNavigationServiceFromFrameLevel((FrameLevel)Enum.Parse(typeof(FrameLevel), navisrvElement.Attribute("NavigationLevel").Value));

                        navisrv.NavigationFrame.SetNavigationState(navisrvElement.Element("NavigationState").Value);
                        navisrv.HandleTerminationReload();
                        var viewModel = navisrv.GetNavigatedViewModel();
                        if (viewModel != null)
                        {
                            //todo pass restored data

                            var viewModelDataEle = navisrvElement.Element("ViewModelData");

                            Dictionary<string, object> viewModelData = new Dictionary<string, object>();

                            foreach(var ele in viewModelDataEle.Elements())
                            {
                                viewModelData.Add(ele.Name.LocalName, ele.Value);
                            }

                            await viewModel.OnRestoringAsync(viewModelData);
                        }
                    }

                    break; //todo handle multiple windows
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
