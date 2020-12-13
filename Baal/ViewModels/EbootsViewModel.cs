using Baal.Models;
using IgrisLib;
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
using System.Net;
using System.Windows.Data;

namespace Baal.ViewModels
{
    public class EbootsViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator dialogCoordinator;

        public MainViewModel MainViewModel { get; }

        public PS3API PS3 { get; }

        public string SearchText
        {
            get => GetValue(() => SearchText);
            set
            {
                SetValue(() => SearchText, value);

                ObservableCollection<EBOOT> eboots = new ObservableCollection<EBOOT>();
                string search = value;

                foreach (EBOOT eboot in EbootsCollectionSave)
                {
                    var props = eboot.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        if (Convert.ToString(prop.GetValue(eboot, null)).ToLower().Contains(search.ToLower()))
                        {
                            eboots.Add(eboot);
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(search))
                {
                    EbootsCollection = EbootsCollectionSave;
                    return;
                }
                ICollectionView view = CollectionViewSource.GetDefaultView(eboots);
                view.GroupDescriptions.Add(new PropertyGroupDescription("Game"));
                view.SortDescriptions.Add(new SortDescription("Game", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Mode", ListSortDirection.Ascending));
                EbootsCollection = eboots;
            }
        }

        public ObservableCollection<EBOOT> EbootsCollectionSave { get; set; }

        public ObservableCollection<EBOOT> EbootsCollection { get => GetValue(() => EbootsCollection); set => SetValue(() => EbootsCollection, value); }

        public EBOOT SelectedEboot { get => GetValue(() => SelectedEboot); set => SetValue(() => SelectedEboot, value); }

        public string EBOOTPath { get => GetValue(() => EBOOTPath); set => SetValue(() => EBOOTPath, value); }

        public List<string> EBOOTGameList
        {
            get
            {
                List<string> gameList = new List<string>();
                foreach (string gl in Directory.GetDirectories(@"Files\EBOOTS"))
                {
                    gameList.Add(gl.Replace(@"Files\EBOOTS\", ""));
                }
                return gameList;
            }
        }

        public string SelectedGame { get => GetValue(() => SelectedGame); set => SetValue(() => SelectedGame, value); }

        public List<string> EBOOTModeList
        {
            get
            {
                List<string> modeList = new List<string>
                {
                    "MP",
                    "SP",
                    "ZM"
                };
                return modeList;
            }
        }

        public string SelectedMode { get => GetValue(() => SelectedMode); set => SetValue(() => SelectedMode, value); }

        public string EBOOTDestinationPath { get => GetValue(() => EBOOTDestinationPath); set => SetValue(() => EBOOTDestinationPath, value); }

        public DelegateCommand GetEBOOTSCommand { get; }

        public DelegateCommand BrowseEBOOTCommand { get; }

        public DelegateCommand AddEBOOTCommand { get; }

        public DelegateCommand DeleteEBOOTCommand { get; }

        public DelegateCommand UploadEBOOTCommand { get; }

        public EbootsViewModel(IDialogCoordinator instance, MainViewModel mainViewModel, PS3API ps3)
        {
            dialogCoordinator = instance;
            MainViewModel = mainViewModel;
            PS3 = ps3;
            GetEBOOTSCommand = new DelegateCommand(GetEBOOTS);
            BrowseEBOOTCommand = new DelegateCommand(BrowseEBOOT);
            AddEBOOTCommand = new DelegateCommand(AddEBOOT, CanExecuteAddEBOOT);
            DeleteEBOOTCommand = new DelegateCommand(DeleteEBOOT, CanExecuteDeleteEBOOT);
            UploadEBOOTCommand = new DelegateCommand(UploadEBOOT, CanExecuteUploadEBOOT);
            GetEBOOTS();
            SelectedGame = EBOOTGameList.FirstOrDefault();
            SelectedMode = EBOOTModeList.FirstOrDefault();
            EBOOTDestinationPath = "dev_hdd0/game/{Your Game Region}/USRDIR/";
        }

        private EbootsViewModel()
        {

        }

        ~EbootsViewModel()
        {
            Dispose();
        }

        private bool CanExecuteAddEBOOT()
        {
            return !string.IsNullOrEmpty(SelectedGame) && !string.IsNullOrEmpty(SelectedMode) && !string.IsNullOrEmpty(EBOOTPath);
        }

        private bool CanExecuteDeleteEBOOT()
        {
            return SelectedEboot != null;
        }

        private bool CanExecuteUploadEBOOT()
        {
            return SelectedEboot != null && EbootsCollection.Where(x => x.IsSelected).Count() == 1 && MainViewModel.IsConnected;
        }

        private void GetEBOOTS()
        {
            ObservableCollection<EBOOT> filesCollection = new ObservableCollection<EBOOT>();
            string path = $@"{AppDomain.CurrentDomain.BaseDirectory}Files\EBOOTS";
            var folder = new DirectoryInfo(path);
            var files = folder.GetFiles("*bin", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var icon = Icon.ExtractAssociatedIcon(file.FullName);
                var bmp = icon.ToBitmap();
                var name = file.Name.Length > 13 ? $"{file.Name.Substring(0, 13)}..." : file.Name;
                var mode = Path.GetFileNameWithoutExtension(Directory.GetParent(file.FullName).ToString());
                var game = Path.GetFileNameWithoutExtension(Directory.GetParent(Directory.GetParent(file.FullName).ToString()).ToString());
                filesCollection.Add(new EBOOT() { Game = game, Mode = mode, Name = name, Path = file.FullName, FileThumbnail = BitmapConversion.BitmapToBitmapSource(bmp) });
            }
            ICollectionView view = CollectionViewSource.GetDefaultView(filesCollection);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Game"));
            view.SortDescriptions.Add(new SortDescription("Game", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Mode", ListSortDirection.Ascending));
            EbootsCollectionSave = filesCollection;
            EbootsCollection = filesCollection;
        }

        private void BrowseEBOOT()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "BINARY Files|*.bin",
                Title = "Select a File"
            };

            if (openFileDialog.ShowDialog() == true)
                EBOOTPath = openFileDialog.FileName;
        }

        private async void AddEBOOT()
        {
            if (!string.IsNullOrEmpty(EBOOTPath))
            {
                string sourcePath = EBOOTPath;
                string targetPath = $@"Files\EBOOTS\{SelectedGame}\{SelectedMode}\";
                string fileName = Path.GetFileName(sourcePath);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                if (Directory.Exists(sourcePath.Replace(fileName, "")))
                    File.Copy(sourcePath, targetPath + fileName);
                else
                    await dialogCoordinator.ShowMessageAsync(this, "Failed...", "This file doesn't exist");
            }
            else
                await dialogCoordinator.ShowMessageAsync(this, "Error", "File Path is null");
            GetEBOOTS();
        }

        private async void DeleteEBOOT()
        {
            foreach (var eboot in EbootsCollection.Where(x => x.IsSelected))
            {
                string sourcePath = eboot.Path;
                string fileName = Path.GetFileName(sourcePath);
                if (Directory.Exists(sourcePath.Replace(fileName, "")))
                    File.Delete(sourcePath);
                else
                    await dialogCoordinator.ShowMessageAsync(this, "Failed...", "This file doesn't exist");
            }
            GetEBOOTS();
        }

        private async void UploadEBOOT()
        {
            string sourcePath = SelectedEboot.Path;
            string fileName = Path.GetFileName(sourcePath);
            if (Directory.Exists(sourcePath.Replace(fileName, "")))
            {
                IgrisLib.NET.PS3TMAPI.TCPIPConnectProperties connectProperties = PS3.TMAPI.GetConnectionInfo();
                //Create FTP request
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{connectProperties.IPAddress}{(EBOOTDestinationPath.Substring(1, 1) == "/" ? "" : "/")}{EBOOTDestinationPath}{(EBOOTDestinationPath.Substring(EBOOTDestinationPath.Length - 1) == "/" ? "" : "/")}{fileName}");

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential("", "");
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                //Load the file
                FileStream stream = File.OpenRead(sourcePath);
                byte[] buffer = new byte[stream.Length];

                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                //Upload file
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(buffer, 0, buffer.Length);
                reqStream.Close();
                await dialogCoordinator.ShowMessageAsync(this, "Success...", "This file has been uploaded");
            }
            else
                await dialogCoordinator.ShowMessageAsync(this, "Failed...", "This file doesn't exist");
        }
    }
}
