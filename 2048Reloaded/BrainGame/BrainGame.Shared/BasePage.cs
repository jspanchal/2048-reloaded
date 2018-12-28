using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BrainGame.Common;

namespace BrainGame
{
    public class BasePage : Page
    {
        private NavigationHelper _navigationHelper;

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper ?? (_navigationHelper = new NavigationHelper(this)); }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationHelper.OnNavigatedFrom(e);
        }


#if WINDOWS_APP
        public bool IsWindowsApp { get { return true; } }
        public bool IsWindowsPhoneApp { get { return false; } }
#endif

#if WINDOWS_PHONE_APP
        public bool IsWindowsApp
        {
            get { return false; }
        }

        public bool IsWindowsPhoneApp
        {
            get { return true; }
        }
#endif
    }
}