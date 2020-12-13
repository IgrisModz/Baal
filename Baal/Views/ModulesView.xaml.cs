using Baal.ViewModels;
using IgrisLib;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace Baal.Views
{
    /// <summary>
    /// Logique d'interaction pour ModulesView.xaml
    /// </summary>
    public partial class ModulesView : UserControl
    {
        private string[] file;

        public ModulesView(MainViewModel mainViewModel, PS3API ps3)
        {
            ViewModel = new ModulesViewModel(DialogCoordinator.Instance, mainViewModel, ps3);
            InitializeComponent();
        }

        private ModulesView()
        {
            InitializeComponent();
        }

        public ModulesViewModel ViewModel { get => DataContext as ModulesViewModel; set => DataContext = value; }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            file = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (file[0].Substring(file[0].Length - 5) == ".sprx")
                e.Handled = true;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            (sender as TextBox).Text = file[0];
        }
    }
}
