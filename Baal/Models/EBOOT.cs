using IgrisLib.Mvvm;
using System.Windows.Media.Imaging;

namespace Baal.Models
{
    public class EBOOT : ViewModelBase
    {
        public BitmapSource FileThumbnail { get => GetValue(() => FileThumbnail); set => SetValue(() => FileThumbnail, value); }

        public string Name { get => GetValue(() => Name); set => SetValue(() => Name, value); }

        public string Game { get => GetValue(() => Game); set => SetValue(() => Game, value); }

        public string Mode { get => GetValue(() => Mode); set => SetValue(() => Mode, value); }

        public string Path { get => GetValue(() => Path); set => SetValue(() => Path, value); }

        public bool IsSelected { get => GetValue(() => IsSelected); set => SetValue(() => IsSelected, value); }
    }
}
