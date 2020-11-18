using Baal.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Baal
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private int RainbowTick = 0;

        public MainWindow()
        {
            ViewModel = new MainViewModel(DialogCoordinator.Instance);
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.001)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public MainViewModel ViewModel { get => DataContext as MainViewModel; set => DataContext = value; }

        private Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (RainbowTick >= 361) RainbowTick = 0;
            else RainbowTick++;
            Color Rainbow = ColorFromHSV(RainbowTick, 1, 1);
            SolidColorBrush brush = new SolidColorBrush(Rainbow);
            //DukezCredit.Foreground = brush;
            //MeCredit.Foreground = brush;
            //SonyCredit.Foreground = brush;
            //HowToUseText.Foreground = brush;
            //PSText.Foreground = brush;
        }
    }
}
