using IgrisLib.MessageBox;
using IgrisLib.ViewModels;
using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;

namespace IgrisLib.Views
{
    /// <summary>
    /// Interaction logic for CCAPIWindow.xaml
    /// </summary>
    public partial class CCAPIWindow : MetroWindow
    {
        public bool Result { get; internal set; }

        public IConnectAPI Api { get; private set; }

        public CCAPIWindow(IConnectAPI api, ResourceDictionary resources)
        {
            Api = api;
            ViewModel = new CCAPIViewModel(this, api, resources);
            InitializeComponent();
            this.Resources = resources;
            listView.Focus();
        }

        private CCAPIWindow()
        {
            InitializeComponent();
        }

        internal CCAPIViewModel ViewModel
        {
            get => DataContext as CCAPIViewModel;
            set => DataContext = value;
        }

        internal new bool Show()
        {
            if (ViewModel.Consoles.Count() > 0)
            {
                this.ShowDialog();
            }
            else
            {

                this.Result = false;
                base.Close();
                IgrisMessageBox.Show(this.Resources["noConsole"].ToString(), this.Resources["noConsoleTitle"].ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            return Result;
        }
    }
}
