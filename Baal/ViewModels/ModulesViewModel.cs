using Baal.Models;
using IgrisLib;
using IgrisLib.Mvvm;
using IgrisLib.NET;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;

namespace Baal.ViewModels
{
    public class ModulesViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator dialogCoordinator;

        public MainViewModel MainViewModel { get; }

        public PS3API PS3 { get; }

        public ObservableCollection<PS3Module> Modules { get => GetValue(() => Modules); set => SetValue(() => Modules, value); }

        public PS3Module SelectedModule { get => GetValue(() => SelectedModule); set => SetValue(() => SelectedModule, value); }

        public string SPRXPath { get => GetValue(() => SPRXPath); set => SetValue(() => SPRXPath, value); }

        public DelegateCommand BrowseSPRXCommand { get; }

        public DelegateCommand LoadSPRXCommand { get; }

        public DelegateCommand RefreshModulesCommand { get; }

        public DelegateCommand UnloadModuleCommand { get; }

        public ModulesViewModel(IDialogCoordinator instance, MainViewModel mainViewModel, PS3API ps3)
        {
            dialogCoordinator = instance;
            MainViewModel = mainViewModel;
            PS3 = ps3;
            BrowseSPRXCommand = new DelegateCommand(BrowseSPRX);
            LoadSPRXCommand = new DelegateCommand(LoadSPRX, CanExecuteLoadSPRX);
            RefreshModulesCommand = new DelegateCommand(RefreshModules, CanExecuteRefreshModules);
            UnloadModuleCommand = new DelegateCommand(UnloadModule, CanExecuteUnloadModule);
            SPRXPath = "/dev_hdd0/tmp/module.sprx";
        }

        private ModulesViewModel()
        {

        }

        private bool CanExecuteLoadSPRX()
        {
            return !string.IsNullOrEmpty(SPRXPath) && MainViewModel.IsAttached;
        }

        private bool CanExecuteRefreshModules()
        {
            return MainViewModel.IsAttached;
        }

        private bool CanExecuteUnloadModule()
        {
            return MainViewModel.IsAttached && SelectedModule != null;
        }

        private void BrowseSPRX()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "SPRX Files|*.sprx",
                Title = "Select a File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SPRXPath = openFileDialog.FileName;
            }
        }

        private async void LoadSPRX()
        {
            string modulePath = SPRXPath;
            if (!modulePath.Contains("hdd0"))
            {
                modulePath = "/host_root/" + SPRXPath;
            }

            modulePath.Replace("\\", "/");
            ulong error;
            if (PS3.GetCurrentAPI().GetType() == typeof(TMAPI))
                error = PS3RPC.LoadModule(modulePath);
            else error = PS3.PS3MAPI.LoadModule(modulePath);
            Thread.Sleep(150);
            RefreshModules();
            if (error != 0x0)
            {
                await dialogCoordinator.ShowMessageAsync(this, "Error...", $"Load Module Error: 0x{error:X}");
            }
        }

        private void RefreshModules()
        {
            ObservableCollection<PS3Module> modules = new ObservableCollection<PS3Module>();
            if (PS3.GetCurrentAPI().GetType() == typeof(TMAPI))
            {
                foreach (uint module in PS3RPC.GetModules())
                {
                    if (module != 0x0)
                    {
                        modules.Add(new PS3Module
                        {
                            Name = PS3.TMAPI.GetModuleName(module),
                            ID = $"0x{module:X}",
                            Path = "N/A",
                            Start = $"0x{PS3.TMAPI.GetModuleStartAddress(module):X} ",
                            Stop = $"0x{PS3.TMAPI.GetModuleStopAddress(module):X} ",
                            Size = $"0x{PS3.TMAPI.GetModuleSize(module):X}",
                        });
                    }
                }
            }
            else
            {
                foreach(int module in PS3.PS3MAPI.GetModules())
                {
                    if (module != 0x0)
                    {
                        modules.Add(new PS3Module
                        {
                            Name = PS3.PS3MAPI.GetModuleName(module),
                            ID = $"0x{module:X}",
                            Path = PS3.PS3MAPI.GetModuleFilename(module),
                            Start = "N/A",
                            Stop = "N/A",
                            Size = "N/A"
                        });
                    }
                }
            }
            Modules = modules;
        }

        private async void UnloadModule()
        {
            uint.TryParse(SelectedModule.ID.Substring(2), NumberStyles.HexNumber,
                    CultureInfo.CurrentCulture,
                    out uint result);
            ulong error;
            if (PS3.GetCurrentAPI().GetType() == typeof(TMAPI))
                error = PS3RPC.UnloadModule(result);
            else error = PS3.PS3MAPI.UnloadModule((int)result);
            Thread.Sleep(150);
            RefreshModules();
            if (error != 0x0)
            {
                await dialogCoordinator.ShowMessageAsync(this, "Error...", $"Unload Module Error: 0x{error:X}");
            }
        }
    }
}
