using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#if NETFX_CORE

#endif

#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Data;
#endif


namespace Brain.Extensions
{
    public static class TextBoxExtensions
    {

        /*
#if WINDOWS_PHONE
        // Force textbox to update viewmodel on any change.  
        // This is not supported with the default WP7 implementation
        public static void UpdateSourceOnPropertyChanged(this TextBox textBox)
        {
            textBox.TextChanged +=
                (sender, args) =>
                {
                    var textbox = sender as TextBox;
                    if (textbox == null) return;

                    var binding = textbox.GetBindingExpression(TextBox.TextProperty);
                    if (binding == null) return;

                    binding.UpdateSource();
                };
        }

        public static void UpdateSourceOnPropertyChanged(this PasswordBox textBox)
        {
            textBox.PasswordChanged +=
                (sender, args) =>
                {
                    var passwordBox = sender as PasswordBox;
                    if (passwordBox == null) return;

                    var binding = passwordBox.GetBindingExpression(PasswordBox.PasswordProperty);
                    if (binding == null) return;

                    binding.UpdateSource();
                };
        }
#endif
         */

        private const int SCROLL_PADDING = 30;
        public static void SetScrollTo(this Control control, ScrollViewer scrollViewer)
        {
            if (control == null) return;
            if (scrollViewer == null) return;

            control.GotFocus +=
                (sender, args) =>
                {
                    var transform = control.TransformToVisual(scrollViewer);

                    var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));

                    if (positionInScrollViewer.Y < 0 ||
                        positionInScrollViewer.Y > 200) //scrollViewer.ViewportHeight)
                    {
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + positionInScrollViewer.Y - SCROLL_PADDING);
                    }

                    if (positionInScrollViewer.X < 0 ||
                        positionInScrollViewer.X > scrollViewer.ViewportWidth)
                    {
                        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + positionInScrollViewer.X - SCROLL_PADDING);
                    }

                };
        }
    }
}
