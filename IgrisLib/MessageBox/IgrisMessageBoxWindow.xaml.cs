using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Media;
using System.Threading;
using System.Windows;

namespace IgrisLib.MessageBox
{
    /// <summary>
    /// Logique d'interaction pour IgrisMessageBoxWindow.xaml
    /// </summary>
    internal partial class IgrisMessageBoxWindow : MetroWindow
    {
        internal string Caption { get => Title; set => Title = value; }
        internal string Message { get =>  txtBMessage.Text; set => txtBMessage.Text = value; }
        internal string OkButtonText { get => lblOk.Text; set => lblOk.Text = value.TryAddKeyboardAccellerator(); }
        internal string CancelButtonText { get => lblCancel.Text; set => lblCancel.Text = value.TryAddKeyboardAccellerator(); }
        internal string YesButtonText { get => lblYes.Text; set => lblYes.Text = value.TryAddKeyboardAccellerator(); }
        internal string NoButtonText { get => lblNo.Text; set => lblNo.Text = value.TryAddKeyboardAccellerator(); }
        public MessageBoxResult Result { get; set; }

        internal IgrisMessageBoxWindow(string message)
        {
            InitializeComponent();
            SetLanguageDictionary();
            Message = message;
            imageMessageBox.Visibility = Visibility.Collapsed;
            imageMessageBoxQ.Visibility = Visibility.Collapsed;
            DisplayButtons(MessageBoxButton.OK);
        }

        internal IgrisMessageBoxWindow(string message, string caption)
        {
            InitializeComponent();
            SetLanguageDictionary();
            Message = message;
            Caption = caption;
            imageMessageBox.Visibility = Visibility.Collapsed;
            imageMessageBoxQ.Visibility = Visibility.Collapsed;
            DisplayButtons(MessageBoxButton.OK);
        }

        internal IgrisMessageBoxWindow(string message, string caption, MessageBoxButton button)
        {
            InitializeComponent();
            SetLanguageDictionary();
            Message = message;
            Caption = caption;
            //btnYes.Content = "Yes";
            //btnNo.Content = "No";
            //btnCancel.Content = "Cancel";
            imageMessageBox.Visibility = Visibility.Collapsed;
            imageMessageBoxQ.Visibility = Visibility.Collapsed;
            DisplayButtons(button);
        }

        internal IgrisMessageBoxWindow(string message, string caption, MessageBoxImage image)
        {
            InitializeComponent();
            SetLanguageDictionary();
            Message = message;
            Caption = caption;
            DisplayImage(image);
            DisplayButtons(MessageBoxButton.OK);
        }

        internal IgrisMessageBoxWindow(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            InitializeComponent();
            SetLanguageDictionary();
            Message = message;
            Caption = caption;
            //btnYes.Content = "Yes";
            //btnNo.Content = "No";
            //btnCancel.Content = "Cancel";
            imageMessageBox.Visibility = Visibility.Collapsed;
            imageMessageBoxQ.Visibility = Visibility.Collapsed;
            DisplayButtons(button);
            DisplayImage(image);
        }

        private void DisplayButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    btnOK.Visibility = Visibility.Visible;
                    btnOK.Focus();
                    btnCancel.Visibility = Visibility.Visible;
                    btnYes.Visibility = Visibility.Collapsed;
                    btnNo.Visibility = Visibility.Collapsed;
                    return;
                case MessageBoxButton.YesNoCancel:
                    btnYes.Visibility = Visibility.Visible;
                    btnYes.Focus();
                    btnNo.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnOK.Visibility = Visibility.Collapsed;
                    return;
                case MessageBoxButton.YesNo:
                    btnYes.Visibility = Visibility.Visible;
                    btnYes.Focus();
                    btnNo.Visibility = Visibility.Visible;
                    btnOK.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    return;
            }
            btnOK.Visibility = Visibility.Visible;
            btnOK.Focus();
            btnYes.Visibility = Visibility.Collapsed;
            btnNo.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;
        }

        private void DisplayImage(MessageBoxImage image)
        {
            //Icon icon;
            PackIconMaterialDesignKind icon;
            switch(image)
            {
                case MessageBoxImage.Hand:
                    //icon = SystemIcons.Hand;
                    icon = PackIconMaterialDesignKind.Error;
                    SystemSounds.Hand.Play();
                    break;
                case MessageBoxImage.Question:
                    //icon = SystemIcons.Question;
                    imageMessageBoxQ.Kind = PackIconZondiconsKind.Question;
                    imageMessageBoxQ.Visibility = Visibility.Visible;
                    SystemSounds.Question.Play();
                    return;
                case MessageBoxImage.Exclamation:
                    //icon = SystemIcons.Exclamation;
                    icon = PackIconMaterialDesignKind.Warning;
                    SystemSounds.Exclamation.Play();
                    break;
                case MessageBoxImage.Asterisk:
                    //icon = SystemIcons.Information;
                    icon = PackIconMaterialDesignKind.Info;
                    SystemSounds.Asterisk.Play();
                    break;
                default:
                    icon = PackIconMaterialDesignKind.None;
                    imageMessageBox.Kind = icon;
                    imageMessageBox.Visibility = Visibility.Collapsed;
                    imageMessageBoxQ.Visibility = Visibility.Collapsed;
                    return;

            }
            //if image
            //imageMessageBox.Source = icon.ToImageSource();
            imageMessageBox.Kind = icon;
            imageMessageBox.Visibility = Visibility.Visible;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            base.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            base.Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            base.Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            base.Close();
        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                case "en-GB":
                case "en-AU":
                case "en-CA":
                case "en-ZN":
                case "en-IE":
                case "en-ZA":
                case "en-JM":
                case "en-029":
                case "en-BZ":
                case "en-TT":
                case "en-ZW":
                case "en-PH":
                    dict.Source = new Uri("pack://application:,,,/IgrisLib;component\\IgrisLib\\MessageBox\\Resources\\en-US.xaml", UriKind.Absolute);
                    break;
                case "fr-FR":
                case "fr-BE":
                case "fr-CA":
                case "fr-CH":
                case "fr-LU":
                case "fr-MC":
                    dict.Source = new Uri("pack://application:,,,/IgrisLib;component\\MessageBox\\Resources\\fr-FR.xaml", UriKind.Absolute);
                    break;
                default:
                    dict.Source = new Uri("pack://application:,,,/IgrisLib;component\\IgrisLib\\MessageBox\\Resources\\en-US.xaml", UriKind.Absolute);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }
    }
}
