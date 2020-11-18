using Baal.Views;
using IgrisLib;
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

        public TMAPI PS3 { get; }

        public Thread BackgroundFunctionsThread { get; }

        public ModulesView ModulesView { get => GetValue(() => ModulesView); set => SetValue(() => ModulesView, value); }

        public SprxView SprxView { get => GetValue(() => SprxView); set => SetValue(() => SprxView, value); }

        public EbootsView EbootsView { get => GetValue(() => EbootsView); set => SetValue(() => EbootsView, value); }

        public string Status { get => GetValue(() => Status); set => SetValue(() => Status, value); }

        public bool IsConnected { get => GetValue(() => IsConnected); set => SetValue(() => IsConnected, value); }

        public bool IsAttached { get => GetValue(() => IsAttached); set => SetValue(() => IsAttached, value); }

        public string CurrentGame { get => GetValue(() => CurrentGame); set => SetValue(() => CurrentGame, value); }

        public DelegateCommand ConnectCommand { get; }

        public MainViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
            PS3 = new TMAPI();
            ModulesView = new ModulesView(this, PS3);
            SprxView = new SprxView(this);
            EbootsView = new EbootsView(this, PS3);
            ConnectCommand = new DelegateCommand(Connect);
            CreateDirectory();
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
            if (!Directory.Exists("SPRX"))
            {
                Directory.CreateDirectory(@"SPRX\Advanced Warfare");
                Directory.CreateDirectory(@"SPRX\Battlefield 3");
                Directory.CreateDirectory(@"SPRX\Battlefield 4");
                Directory.CreateDirectory(@"SPRX\Battlefield Hardline");
                Directory.CreateDirectory(@"SPRX\Black Ops 1");
                Directory.CreateDirectory(@"SPRX\Black Ops 2");
                Directory.CreateDirectory(@"SPRX\Black Ops 3");
                Directory.CreateDirectory(@"SPRX\Ghosts");
                Directory.CreateDirectory(@"SPRX\GTA IV");
                Directory.CreateDirectory(@"SPRX\GTA V");
                Directory.CreateDirectory(@"SPRX\Minecraft");
                Directory.CreateDirectory(@"SPRX\Modern Warfare 1");
                Directory.CreateDirectory(@"SPRX\Modern Warfare 2");
                Directory.CreateDirectory(@"SPRX\Modern Warfare 3");
                Directory.CreateDirectory(@"SPRX\World at War");
            }
            if (!Directory.Exists("EBOOTS"))
            {
                Directory.CreateDirectory(@"EBOOTS\Advanced Warfare\MP");
                Directory.CreateDirectory(@"EBOOTS\Advanced Warfare\ZM");
                Directory.CreateDirectory(@"EBOOTS\Battlefield 3\MP");
                Directory.CreateDirectory(@"EBOOTS\Battlefield 4\MP");
                Directory.CreateDirectory(@"EBOOTS\Battlefield Hardline\MP");
                Directory.CreateDirectory(@"EBOOTS\Black Ops 1\MP");
                Directory.CreateDirectory(@"EBOOTS\Black Ops 1\ZM");
                Directory.CreateDirectory(@"EBOOTS\Black Ops 2\MP");
                Directory.CreateDirectory(@"EBOOTS\Black Ops 2\ZM");
                Directory.CreateDirectory(@"EBOOTS\Black Ops 3\MP");
                Directory.CreateDirectory(@"EBOOTS\Black Ops 3\ZM");
                Directory.CreateDirectory(@"EBOOTS\Ghosts\MP");
                Directory.CreateDirectory(@"EBOOTS\Ghosts\SP");
                Directory.CreateDirectory(@"EBOOTS\GTA IV\MP");
                Directory.CreateDirectory(@"EBOOTS\GTA V\MP");
                Directory.CreateDirectory(@"EBOOTS\Minecraft\MP");
                Directory.CreateDirectory(@"EBOOTS\Modern Warfare 1\MP");
                Directory.CreateDirectory(@"EBOOTS\Modern Warfare 2\MP");
                Directory.CreateDirectory(@"EBOOTS\Modern Warfare 2\SP");
                Directory.CreateDirectory(@"EBOOTS\Modern Warfare 3\MP");
                Directory.CreateDirectory(@"EBOOTS\Modern Warfare 3\SP");
                Directory.CreateDirectory(@"EBOOTS\World at War\MP");
                Directory.CreateDirectory(@"EBOOTS\World at War\ZM");
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
                    if (PS3.GetStatus() == "Connected")
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
