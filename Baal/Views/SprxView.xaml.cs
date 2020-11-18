using Baal.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace Baal.Views
{
    /// <summary>
    /// Logique d'interaction pour SprxView.xaml
    /// </summary>
    public partial class SprxView : UserControl
    {
        private string[] file;

        public SprxView(MainViewModel mainViewModel)
        {
            ViewModel = new SprxViewModel(DialogCoordinator.Instance, mainViewModel);
            InitializeComponent();
        }

        private SprxView()
        {
            InitializeComponent();
        }

        public SprxViewModel ViewModel { get => DataContext as SprxViewModel; set => DataContext = value; }

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
