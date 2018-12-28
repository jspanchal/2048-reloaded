#if WINDOWS_PHONE
using System.Windows.Threading;
#endif
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
#if NETFX_CORE

#endif
#if ANDROID
#endif
#if PORTABLE
#endif


namespace Brain.Utils
{

    public static class Execute
    {
        public static async Task OnUIThread(Action action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () => action());
        }

        public static void DelayAction(TimeSpan delay, Action action)
        {
            new Task(
                async () =>
                    {
                        await Task.Delay(delay);
                        await OnUIThread(action);
                    }).Start();

        }

    }


}
