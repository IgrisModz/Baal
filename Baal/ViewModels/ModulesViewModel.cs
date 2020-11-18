﻿using Baal.Models;
using IgrisLib;
using IgrisLib.Mvvm;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Threading;

namespace Baal.ViewModels
{
    public class ModulesViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator dialogCoordinator;

        public MainViewModel MainViewModel { get; }

        public TMAPI PS3 { get; }

        public ObservableCollection<PS3Module> Modules { get => GetValue(() => Modules); set => SetValue(() => Modules, value); }

        public string SPRXPath { get => GetValue(() => SPRXPath); set => SetValue(() => SPRXPath, value); }

        public DelegateCommand BrowseSPRXCommand { get; }

        public DelegateCommand LoadSPRXCommand { get; }

        public DelegateCommand RefreshModulesCommand { get; }

        public ModulesViewModel(IDialogCoordinator instance, MainViewModel mainViewModel, TMAPI ps3)
        {
            dialogCoordinator = instance;
            MainViewModel = mainViewModel;
            PS3 = ps3;
            BrowseSPRXCommand = new DelegateCommand(BrowseSPRX);
            LoadSPRXCommand = new DelegateCommand(LoadSPRX, CanExecuteLoadSPRX);
            RefreshModulesCommand = new DelegateCommand(RefreshModules, CanExecuteRefreshModules);
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

        private void BrowseSPRX()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "SPRX Files|*.sprx",
                Title = "Select a File"
            };

            if (openFileDialog.ShowDialog() == true)
                SPRXPath = openFileDialog.FileName;
        }

        private async void LoadSPRX()
        {
            string modulePath = SPRXPath;
            if (!modulePath.Contains("hdd0"))
                modulePath = "/host_root/" + SPRXPath;
            modulePath.Replace("\\", "/");
            ulong error = PS3RPC.LoadModule(modulePath);
            Thread.Sleep(150);
            RefreshModules();
            if (error != 0x0)
                await dialogCoordinator.ShowMessageAsync(this, "Error...", $"Load Module Error: 0x{error:X}");
        }

        private void RefreshModules()
        {
            ObservableCollection<PS3Module> modules = new ObservableCollection<PS3Module>();
            foreach (var module in PS3RPC.GetModules())
            {
                if (module != 0x0)
                {
                    modules.Add(new PS3Module
                    {
                        Name = PS3.GetModuleName(module),
                        ID = $"0x{module:X}",
                        Start = $"0x{PS3.GetModuleStartAddress(module):X} ",
                        Stop = $"0x{PS3.GetModuleStopAddress(module):X} ",
                        Size = $"0x{PS3.GetModuleSize(module):X}",
                    });
                }
            }
            Modules = modules;
        }
    }
}
