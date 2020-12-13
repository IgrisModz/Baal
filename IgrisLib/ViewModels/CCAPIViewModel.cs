
using IgrisLib.MessageBox;
using IgrisLib.Mvvm;
using IgrisLib.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IgrisLib.ViewModels
{
    public class CCAPIViewModel : ViewModelBase
    {
        private ResourceDictionary Resources;
        private CCAPIWindow Win;
        private Consoles selectedConsole;
        private List<Consoles> consoles;
        private bool connectEnabled;

        public IConnectAPI Api { get; set; }

        public Consoles SelectedConsole
        {
            get => selectedConsole;
            set
            {
                SetProperty(ref selectedConsole, value);
                ConnectEnabled = value != null;
            }
        }

        public List<Consoles> Consoles { get => consoles; set => SetProperty(ref consoles, value); }

        public bool ConnectEnabled { get => connectEnabled; set => SetProperty(ref connectEnabled, value); }

        public DelegateCommand ConnectCommand { get; }

        public DelegateCommand RefreshCommand { get; }

        public CCAPIViewModel(CCAPIWindow win, IConnectAPI api, ResourceDictionary resources)
        {
            Win = win;
            Api = api;
            Resources = resources;
            ConnectCommand = new DelegateCommand(Connect);
            RefreshCommand = new DelegateCommand(Refresh);
            Refresh();
        }

        private CCAPIViewModel()
        {

        }

        private List<Consoles> GetConsoles()
        {

            List<Consoles> list = new List<Consoles>();
            foreach (CCAPI.ConsoleInfo consoleInfo in Api.GetConsoleList())
            {
                list.Add(new Consoles() { Text = $"{consoleInfo.name} : {consoleInfo.ip}", Name = consoleInfo.name, Ip = consoleInfo.ip });
            }
            return list;
        }

        private void Connect()
        {
            if (SelectedConsole != null)
            {
                if (Api.ConnectTarget(SelectedConsole.Ip))
                {
                    Win.Result = true;
                }
                else
                    Win.Result = false;
                Win.Close();
                return;
            }
            else
                IgrisMessageBox.Show(this.Resources["errorSelect"].ToString(), this.Resources["errorSelectTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Refresh()
        {
            Consoles = GetConsoles();
            SelectedConsole = Consoles.FirstOrDefault();
        }
    }
}
