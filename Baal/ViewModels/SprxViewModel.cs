using Baal.Models;
using IgrisLib.Mvvm;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Baal.ViewModels
{
    public class SprxViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator dialogCoordinator;

        public MainViewModel MainViewModel { get; }

        public string SearchText
        {
            get => GetValue(() => SearchText);
            set
            {
                SetValue(() => SearchText, value);

                ObservableCollection<SPRX> sprxList = new ObservableCollection<SPRX>();
                string search = value;

                foreach (SPRX sprx in SprxCollectionSave)
                {
                    PropertyInfo[] props = sprx.GetType().GetProperties();
                    foreach (PropertyInfo prop in props)
                    {
                        if (Convert.ToString(prop.GetValue(sprx, null)).ToLower().Contains(search.ToLower()))
                        {
                            sprxList.Add(sprx);
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(search))
                {
                    SprxCollection = SprxCollectionSave;
                    return;
                }
                ICollectionView view = CollectionViewSource.GetDefaultView(sprxList);
                view.GroupDescriptions.Add(new PropertyGroupDescription("Game"));
                view.SortDescriptions.Add(new SortDescription("Game", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                SprxCollection = sprxList;
            }
        }

        public ObservableCollection<SPRX> SprxCollectionSave { get; set; }

        public ObservableCollection<SPRX> SprxCollection { get => GetValue(() => SprxCollection); set => SetValue(() => SprxCollection, value); }

        public SPRX SelectedSprx { get => GetValue(() => SelectedSprx); set => SetValue(() => SelectedSprx, value); }

        public string SPRXPath { get => GetValue(() => SPRXPath); set => SetValue(() => SPRXPath, value); }

        public List<string> SPRXGameList
        {
            get
            {
                List<string> gameList = new List<string>();
                foreach (string gl in Directory.GetDirectories(@"Files\SPRX"))
                {
                    gameList.Add(gl.Replace(@"Files\SPRX\", ""));
                }
                return gameList;
            }
        }

        public string SelectedGame { get => GetValue(() => SelectedGame); set => SetValue(() => SelectedGame, value); }

        public DelegateCommand GetSPRXCommand { get; }

        public DelegateCommand BrowseSPRXCommand { get; }

        public DelegateCommand AddSPRXCommand { get; }

        public DelegateCommand DeleteSPRXCommand { get; }

        public DelegateCommand SelectSPRXCommand { get; }

        public SprxViewModel(IDialogCoordinator instance, MainViewModel mainViewModel)
        {
            dialogCoordinator = instance;
            MainViewModel = mainViewModel;
            GetSPRXCommand = new DelegateCommand(GetSPRX);
            BrowseSPRXCommand = new DelegateCommand(BrowseSPRX);
            AddSPRXCommand = new DelegateCommand(AddSPRX, CanExecuteAddSPRX);
            DeleteSPRXCommand = new DelegateCommand(DeleteSPRX, CanExecuteDeleteSPRX);
            SelectSPRXCommand = new DelegateCommand(SelectSPRX, CanExecuteSelectSPRX);
            SelectedGame = SPRXGameList.FirstOrDefault();
            GetSPRX();
        }

        private SprxViewModel()
        {

        }

        ~SprxViewModel()
        {
            Dispose();
        }

        private bool CanExecuteAddSPRX()
        {
            return SelectedGame != null && !string.IsNullOrEmpty(SPRXPath);
        }

        private bool CanExecuteDeleteSPRX()
        {
            return SelectedSprx != null;
        }

        private bool CanExecuteSelectSPRX()
        {
            return SelectedSprx != null && SprxCollection.Count(x => x.IsSelected) == 1;
        }

        private void GetSPRX()
        {
            ObservableCollection<SPRX> filesCollection = new ObservableCollection<SPRX>();
            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}Files\SPRX";
            DirectoryInfo folder = new DirectoryInfo(path);
            FileInfo[] files = folder.GetFiles("*sprx", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                Icon icon = Icon.ExtractAssociatedIcon(file.FullName);
                Bitmap bmp = icon.ToBitmap();
                string name = file.Name.Length > 13 ? $"{file.Name.Substring(0, 13)}..." : file.Name;
                string game = Path.GetFileNameWithoutExtension(Directory.GetParent(file.FullName).ToString());
                filesCollection.Add(new SPRX() { Game = game, Name = name, Path = file.FullName, FileThumbnail = BitmapConversion.BitmapToBitmapSource(bmp) });
            }
            ICollectionView view = CollectionViewSource.GetDefaultView(filesCollection);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Game"));
            view.SortDescriptions.Add(new SortDescription("Game", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            SprxCollectionSave = filesCollection;
            SprxCollection = filesCollection;
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

        private async void AddSPRX()
        {
            if (!string.IsNullOrEmpty(SPRXPath))
            {
                string sourcePath = SPRXPath;
                string targetPath = $@"Files\SPRX\{SelectedGame}\";
                string fileName = Path.GetFileName(sourcePath);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                if (Directory.Exists(sourcePath.Replace(fileName, "")))
                {
                    File.Copy(sourcePath, targetPath + fileName);
                }
                else
                {
                    await dialogCoordinator.ShowMessageAsync(this, "Failed...", "This file doesn't exist");
                }
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, "Error", "File Path is null");
            }

            GetSPRX();
        }

        private async void DeleteSPRX()
        {
            foreach (SPRX eboot in SprxCollection.Where(x => x.IsSelected))
            {
                string sourcePath = SelectedSprx.Path;
                string fileName = Path.GetFileName(sourcePath);
                if (Directory.Exists(sourcePath.Replace(fileName, "")))
                {
                    File.Delete(sourcePath);
                }
                else
                {
                    await dialogCoordinator.ShowMessageAsync(this, "Failed...", "This file doesn't exist");
                }
            }
            GetSPRX();
        }

        private void SelectSPRX()
        {
            MainViewModel.ModulesView.ViewModel.SPRXPath = SelectedSprx.Path;
        }
    }
}
