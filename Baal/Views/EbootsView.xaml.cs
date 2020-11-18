using Baal.ViewModels;
using IgrisLib;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace Baal.Views
{
    /// <summary>
    /// Logique d'interaction pour EbootsView.xaml
    /// </summary>
    public partial class EbootsView : UserControl
    {
        private string[] file;

        public EbootsView(MainViewModel mainViewModel, TMAPI ps3)
        {
            ViewModel = new EbootsViewModel(DialogCoordinator.Instance, mainViewModel, ps3);
            InitializeComponent();
        }

        private EbootsView()
        {
            InitializeComponent();
        }

        public EbootsViewModel ViewModel { get => DataContext as EbootsViewModel; set => DataContext = value; }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            file = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (file[0].Substring(file[0].Length - 4) == ".bin")
                e.Handled = true;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            (sender as TextBox).Text = file[0];
        }
    }
}
