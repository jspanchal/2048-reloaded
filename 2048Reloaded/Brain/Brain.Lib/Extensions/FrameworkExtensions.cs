#if NETFX_CORE

#endif
#if WINDOWS_PHONE
using System.Windows;
#endif
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Brain.Extensions
{
    public static class FrameworkExtensions
    {
        public static Size GetSize(this FrameworkElement element)
        {
            double width = element.ActualWidth;
            double height = element.ActualHeight;

            if ((element.Width > 0) && (element.ActualWidth <= 0)) // Before Loaded
                width = element.Width;

            if ((element.Height > 0) && (element.ActualHeight <= 0)) // Before Loaded
                height = element.Height;

            return new Size(width, height);
        }


        public static T Child<T>(this FrameworkElement element, string name) where T : FrameworkElement
        {
            return FindChild(element, e => e.Name == name) as T;
        }

        public static FrameworkElement FindChild(this FrameworkElement element, Func<FrameworkElement, bool> predicate)
        {
            if (element != null)
            {
                int intCount = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < intCount; i++)
                {
                    var child = (FrameworkElement) VisualTreeHelper.GetChild(element, i);
                    if (child != null)
                    {
                        if (predicate(child))
                            return child;
                        child = FindChild(child, predicate);

                        if (child != null)
                            return child;
                    }
                }
            }
            return null;
        }
    }
}