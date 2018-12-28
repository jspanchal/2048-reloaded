using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#if NETFX_CORE

#endif

#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#endif

namespace Brain.Extensions
{
    public static class ScrollViewerExtensions
    {
        public static void ScrollTo(this ScrollViewer scrollViewer, FrameworkElement element)
        {
            var transform = element.TransformToVisual(scrollViewer);

            var position = transform.TransformPoint(new Point(0, 0));

            scrollViewer.ScrollToVerticalOffset(position.Y);
        }
    }
}
