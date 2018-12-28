using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Brain.Utils
{
    public class MessageBox
    {
        public static async Task<MessageBoxResult> ShowAsync(
                string messageBoxText,
                string caption,
                MessageBoxButton button)
        {

            var md = new MessageDialog(messageBoxText, caption);
            var result = MessageBoxResult.None;
            if (button.HasFlag(MessageBoxButton.OK))
                md.Commands.Add(new UICommand("OK", cmd => result = MessageBoxResult.OK));

            if (button.HasFlag(MessageBoxButton.Yes))
                md.Commands.Add(new UICommand("Yes", cmd => result = MessageBoxResult.Yes));

            if (button.HasFlag(MessageBoxButton.No))
                md.Commands.Add(new UICommand("No", cmd => result = MessageBoxResult.No));

            if (button.HasFlag(MessageBoxButton.Cancel))
            {
                md.Commands.Add(new UICommand("Cancel", cmd => result = MessageBoxResult.Cancel));
                md.CancelCommandIndex = (uint)md.Commands.Count - 1;
            }
            await md.ShowAsync();
            return result;
        }

        public static async Task<MessageBoxResult> ShowAsync(string messageBoxText)
        {
            return await ShowAsync(messageBoxText, null, MessageBoxButton.OK);
        }
    }

    [Flags]
    public enum MessageBoxButton
    {
        // Only OK button
        OK = 1,
        // Only Cancel button
        Cancel = 2,
        // Both OK and Cancel buttons
        OKCancel = OK | Cancel,

        // Only Yes button
        Yes = 4,
        // Only No button
        No = 8,
        // Both Yes and No buttons
        YesNo = Yes | No,
    }

    public enum MessageBoxResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7,
    }
}
