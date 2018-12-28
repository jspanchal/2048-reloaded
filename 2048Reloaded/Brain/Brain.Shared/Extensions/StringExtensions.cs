using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
#if NETFX_CORE

#endif

#if WINDOWS_PHONE
using System.Windows.Media;
#endif


namespace Brain.Extensions
{
    public static class StringExtensions
    {
        public static Brush ToColorBrush(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return default(Brush);

            text = text.Replace("#", "")
                       .Replace(";", "");

            if (text.Length == 6)
            {
                byte r = Convert.ToByte(text.Substring(0, 2), 16);
                byte g = Convert.ToByte(text.Substring(2, 2), 16);
                byte b = Convert.ToByte(text.Substring(4, 2), 16);
                return new SolidColorBrush(Color.FromArgb(255, r, g, b));
            }
            else if (text.Length == 8)
            {
                byte a = Convert.ToByte(text.Substring(0, 2), 16);
                byte r = Convert.ToByte(text.Substring(2, 2), 16);
                byte g = Convert.ToByte(text.Substring(4, 2), 16);
                byte b = Convert.ToByte(text.Substring(6, 2), 16);
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
            return default(Brush);
        }

        public static string Reverse(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

    }
}
