using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Brain.Animate;
using BrainGame.Common;
using UniversalRateReminder;
#if WINDOWS_APP
using Windows.UI.ApplicationSettings;
#endif

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace BrainGame
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public const string FamilyPackageName = "23236DrDevelop.Binary_03x8wr787bwr6";

#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        ///     Initializes the singleton instance of the <see cref="App" /> class. This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used when the application is launched to open a specific file, to display
        ///     search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 0;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
    // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    transitions = new TransitionCollection();
                    foreach (Transition c in rootFrame.ContentTransitions)
                    {
                        transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof (HubPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            AnimationManager.RegisterSplashScreen(e.SplashScreen);

            // Ensure the current window is active
            Window.Current.Activate();

            RatePopup.LaunchLimit = 5;
            RatePopup.RateButtonText = "rate";
            RatePopup.CancelButtonText = "cancel";
            RatePopup.Title = "Rate 2048 Reloaded";
            RatePopup.Content = "Show your love by rating 5 star :-)";
            RatePopup.CheckRateReminder();
        }


#if WINDOWS_APP
        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);

            SettingsPane.GetForCurrentView().CommandsRequested += (sender, eventArgs) =>
            {
                var privacyCommand = new SettingsCommand("privacy", "Privacy Policy",
                    async handler =>
                    {
                        await Launcher.LaunchUriAsync(new Uri("http://brainoffline.github.io/privacy.html"));
                    });
                eventArgs.Request.ApplicationCommands.Add(privacyCommand);
            };
        }
#endif

#if WINDOWS_PHONE_APP
        /// <summary>
        ///     Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = transitions ?? new TransitionCollection {new NavigationThemeTransition()};
            rootFrame.Navigated -= RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}