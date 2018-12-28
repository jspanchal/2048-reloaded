using Windows.Foundation;
using Windows.UI.Xaml;
#if NETFX_CORE

#endif

#if WINDOWS_PHONE
using System.Windows;
#endif

namespace Brain.Extensions
{
    public static class FrameworkExtensions
    {
        public static Size GetSize(this FrameworkElement element)
        {
            var width = element.ActualWidth;
            var height = element.ActualHeight;

            if ((element.Width > 0) && (element.ActualWidth <= 0))  // Before Loaded
                width = element.Width;

            if ((element.Height > 0) && (element.ActualHeight <= 0)) // Before Loaded
                height = element.Height;

            return new Size(width, height);
        }
    }
}
