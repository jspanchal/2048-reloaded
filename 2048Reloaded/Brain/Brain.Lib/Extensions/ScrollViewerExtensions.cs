#if NETFX_CORE

#endif
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#endif
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Brain.Extensions
{
    public static class ScrollViewerExtensions
    {
        public static void ScrollTo(this ScrollViewer scrollViewer, FrameworkElement element)
        {
            GeneralTransform transform = element.TransformToVisual(scrollViewer);

            Point position = transform.TransformPoint(new Point(0, 0));

            scrollViewer.ScrollToVerticalOffset(position.Y);
        }
    }
}