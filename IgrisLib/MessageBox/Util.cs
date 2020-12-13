using System.Drawing;
using System.Windows;
//using System.Windows.Interop;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;

namespace IgrisLib.MessageBox
{
    internal static class Util
    {
        //internal static ImageSource ToImageSource(this Icon icon)
        //{
        //    return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        //}

        internal static string TryAddKeyboardAccellerator(this string input)
        {
            if (input.Contains("_"))
            {
                return input;
            }
            return $"_{input}";
        }
    }
}
