﻿using Baal.Views;
using IgrisLib;
using IgrisLib.NET;
using IgrisLib.Mvvm;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.Threading;

namespace Baal.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool backgroundFunctionsEnabled = true, backgroundFunctionsRefreshEnabled;
        private readonly IDialogCoordinator dialogCoordinator;

        public PS3API PS3 { get; }

        public PS3RPC PS3RPC { get; }

        public Thread BackgroundFunctionsThread { get; }

        public ModulesView ModulesView { get => GetValue(() => ModulesView); set => SetValue(() => ModulesView, value); }

        public SprxView SprxView { get => GetValue(() => SprxView); set => SetValue(() => SprxView, value); }

        public EbootsView EbootsView { get => GetValue(() => EbootsView); set => SetValue(() => EbootsView, value); }

        public string Status { get => GetValue(() => Status); set => SetValue(() => Status, value); }

        public bool IsConnected { get => GetValue(() => IsConnected); set => SetValue(() => IsConnected, value); }

        public bool IsAttached { get => GetValue(() => IsAttached); set => SetValue(() => IsAttached, value); }

        public string CurrentGame { get => GetValue(() => CurrentGame); set => SetValue(() => CurrentGame, value); }

        public string APIName { get => GetValue(() => APIName); set => SetValue(() => APIName, value); }

        public DelegateCommand<bool> ApiCommand { get; }

        public DelegateCommand ConnectCommand { get; }

        public MainViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
            PS3 = new PS3API(new TMAPI());
            PS3RPC = new PS3RPC(PS3.TMAPI);
            ModulesView = new ModulesView(this, PS3);
            SprxView = new SprxView(this);
            EbootsView = new EbootsView(this, PS3);
            ApiCommand = new DelegateCommand<bool>(p => ChangeApi(p));
            ConnectCommand = new DelegateCommand(Connect);
            CreateDirectory();
            APIName = "TMAPI";
            Status = "Connected to any ps3!";
            BackgroundFunctionsThread = new Thread(() => BackgroundFunctions()) { IsBackground = true };
            BackgroundFunctionsThread?.Start();
        }

        private MainViewModel()
        {

        }

        ~MainViewModel()
        {
            backgroundFunctionsEnabled = false;
            BackgroundFunctionsThread?.Abort();
            Dispose();
        }

        private void ChangeApi(bool isChecked)
        {
            APIName = isChecked ? "PS3MAPI" : "TMAPI";
            PS3.ChangeAPI(isChecked ? new PS3MAPI() : (IApi)new TMAPI());
        }

        private async void Connect()
        {
            backgroundFunctionsRefreshEnabled = false;
            if (PS3.ConnectTarget())
            {
                if (PS3.AttachProcess())
                {
                    CurrentGame = PS3.GetCurrentGame();
                    backgroundFunctionsRefreshEnabled = true;
                    Status = $"Attached to {CurrentGame}!";
                }
                else
                {
                    await dialogCoordinator.ShowMessageAsync(this, "Attach failed..", "Unable to attach process.");
                    Status = "Attached to any process!";
                }
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, "Connect Failed..", "Unable to connect to PS3.");
                Status = "Connected to any ps3!";
            }
        }

        public static void CreateDirectory()
        {
			if (!Directory.Exists("Files"))
            {
                Directory.CreateDirectory(@"Files\SPRX\Advanced Warfare");
                Directory.CreateDirectory(@"Files\SPRX\Battlefield 3");
                Directory.CreateDirectory(@"Files\SPRX\Battlefield 4");
                Directory.CreateDirectory(@"Files\SPRX\Battlefield Hardline");
                Directory.CreateDirectory(@"Files\SPRX\Black Ops 1");
                Directory.CreateDirectory(@"Files\SPRX\Black Ops 2");
                Directory.CreateDirectory(@"Files\SPRX\Black Ops 3");
                Directory.CreateDirectory(@"Files\SPRX\Ghosts");
                Directory.CreateDirectory(@"Files\SPRX\GTA IV");
                Directory.CreateDirectory(@"Files\SPRX\GTA V");
                Directory.CreateDirectory(@"Files\SPRX\Minecraft");
                Directory.CreateDirectory(@"Files\SPRX\Modern Warfare 1");
                Directory.CreateDirectory(@"Files\SPRX\Modern Warfare 2");
                Directory.CreateDirectory(@"Files\SPRX\Modern Warfare 3");
                Directory.CreateDirectory(@"Files\SPRX\World at War");
                Directory.CreateDirectory(@"Files\EBOOTS\Advanced Warfare\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Advanced Warfare\ZM");
                Directory.CreateDirectory(@"Files\EBOOTS\Battlefield 3\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Battlefield 4\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Battlefield Hardline\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Black Ops 1\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Black Ops 1\ZM");
                Directory.CreateDirectory(@"Files\EBOOTS\Black Ops 2\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Black Ops 2\ZM");
                Directory.CreateDirectory(@"Files\EBOOTS\Black Ops 3\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Black Ops 3\ZM");
                Directory.CreateDirectory(@"Files\EBOOTS\Ghosts\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Ghosts\SP");
                Directory.CreateDirectory(@"Files\EBOOTS\GTA IV\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\GTA V\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Minecraft\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Modern Warfare 1\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Modern Warfare 2\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Modern Warfare 2\SP");
                Directory.CreateDirectory(@"Files\EBOOTS\Modern Warfare 3\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\Modern Warfare 3\SP");
                Directory.CreateDirectory(@"Files\EBOOTS\World at War\MP");
                Directory.CreateDirectory(@"Files\EBOOTS\World at War\ZM");
            }
        }

        private void BackgroundFunctions()
        {
            PS3.ConnectTarget();
            Thread.Sleep(100);
            while (backgroundFunctionsEnabled)
            {
                if (backgroundFunctionsRefreshEnabled)
                {
                    if (PS3.GetConnected())
                    {
                        IsConnected = true;
                        if (PS3.GetAttached())
                        {
                            Status = $"Attached to {CurrentGame}!";
                            IsAttached = true;
                        }
                        else
                        {
                            Status = "Attached to any process!";
                            IsAttached = false;
                        }
                    }
                    else
                    {
                        Status = "Connected to any ps3!";
                        IsConnected = false;
                        IsAttached = false;
                    }
                }
            }
        }
    }
}
