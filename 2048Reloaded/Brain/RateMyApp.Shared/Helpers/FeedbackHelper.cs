/**
 * Copyright (c) 2013 Nokia Corporation. All rights reserved.
 *
 * Nokia, Nokia Connecting People, Nokia Developer, and HERE are trademarks
 * and/or registered trademarks of Nokia Corporation. Other product and company
 * names mentioned herein may be trademarks or trade names of their respective
 * owners.
 *
 * See the license text file delivered with this project for more information.
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using PropertyChanged;

namespace RateMyApp.Helpers
{
    public enum FeedbackState
    {
        Inactive = 0,
        Active,
        FirstReview,
        SecondReview,
        Feedback
    }

    /// <summary>
    /// This helper class controls the behaviour of the FeedbackOverlay control.
    /// When the app has been launched FirstCount times the initial prompt is shown.
    /// If the user reviews no more prompts are shown. When the app has been
    /// launched SecondCount times and not been reviewed, the prompt is shown.
    /// </summary>
    [ImplementPropertyChanged]
    public class FeedbackHelper
    {
        // Constants
        public const string LaunchCountKey = "RATE_MY_APP_LAUNCH_COUNT";
        public const string ReviewedKey = "RATE_MY_APP_REVIEWED";
        public const string LastLaunchDateKey = "RATE_MY_APP_LAST_LAUNCH_DATE";

        // Members
        public event PropertyChangedEventHandler PropertyChanged;
        public static readonly FeedbackHelper Default = new FeedbackHelper();

        public DateTime LastLaunchDate { get; set; }
        public bool IsReviewed { get; set; }
        public FeedbackState State { get; set; }
        public int LaunchCount { get; set; }
        public int FirstCount { get; set; }
        public int SecondCount { get; set; }
        public bool CountDays { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        private FeedbackHelper()
        {
            State = FeedbackState.Active;
        }

        /// <summary>
        /// Called when FeedbackLayout control is instantiated, which is
        /// supposed to happen when application's main page is instantiated.
        /// </summary>
        public void Launching()
        {
#if DEBUG
            var license = Windows.ApplicationModel.Store.CurrentAppSimulator.LicenseInformation;
#else
            var license = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation;
#endif

            // Only load state if app is not trial, app is not activated after
            // being tombstoned, and state has not been loaded before.
            if (!license.IsTrial && 
                // TODO:  PhoneApplicationService.Current.StartupMode == StartupMode.Launch && 
                State == FeedbackState.Active)
            {
                LoadState();
            }

            // Uncomment for testing
            //State = FeedbackState.FirstReview;
            //State = FeedbackState.SecondReview;
        }

        /// <summary>
        /// Call when user has reviewed.
        /// </summary>
        public void Reviewed()
        {
            IsReviewed = true;
            StoreState();
        }

        /// <summary>
        /// Reset review and feedback launch counter and review state.
        /// </summary>
        public void Reset()
        {
            LaunchCount = 0;
            IsReviewed = false;
            LastLaunchDate = DateTime.Now;
            StoreState();
        }

        /// <summary>
        /// Loads last state from storage and works out the new state.
        /// </summary>
        private void LoadState()
        {
            try
            {
                LaunchCount = StorageHelper.GetSetting<int>(LaunchCountKey);
                IsReviewed = StorageHelper.GetSetting<bool>(ReviewedKey);
                LastLaunchDate = StorageHelper.GetSetting<DateTime>(LastLaunchDateKey);

                if (!IsReviewed)
                {
                    if (!CountDays || LastLaunchDate.Date < DateTime.Now.Date)
                    {
                        LaunchCount++;
                        LastLaunchDate = DateTime.Now;
                    }

                    if (LaunchCount == FirstCount)
                    {
                        State = FeedbackState.FirstReview;
                    }
                    else if (LaunchCount == SecondCount)
                    {
                        State = FeedbackState.SecondReview;
                    }

                    StoreState();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FeedbackHelper.LoadState - Failed to load state, Exception: {0}", ex);
            }
        }

        /// <summary>
        /// Stores current state.
        /// </summary>
        private void StoreState()
        {
            try
            {
                StorageHelper.StoreSetting(LaunchCountKey, LaunchCount, true);
                StorageHelper.StoreSetting(ReviewedKey, IsReviewed, true);
                StorageHelper.StoreSetting(LastLaunchDateKey, LastLaunchDate, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FeedbackHelper.StoreState - Failed to store state, Exception: {0}", ex);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public async void Review()
        {
#if WINDDOWS_PHONE_APP
            await Launcher.LaunchUriAsync(CurrentApp.LinkUri);
#endif

#if WINDOWS_APP
            await Launcher.LaunchUriAsync(new Uri(
                    String.Format("ms-windows-store:Review?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
#endif

            Reviewed();
        }
    }
}
