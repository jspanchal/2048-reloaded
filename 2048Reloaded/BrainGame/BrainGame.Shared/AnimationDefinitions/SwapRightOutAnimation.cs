using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Brain.Animate;

namespace BrainGame
{
    public class SwapRightOutAnimation : AnimationDefinition
    {
        public SwapRightOutAnimation()
        {
            Duration = 0.5;
        }

        public override IEnumerable<Timeline> CreateAnimation(FrameworkElement element)
        {
            var frame = Window.Current.Content as Frame;
            double width = (frame != null) ? frame.ActualWidth : 1024;

            return new Timeline[]
            {
                element.AnimateProperty(AnimationProperty.TranslateX)
                    .AddEasingKeyFrame(0.0, 0)
                    .AddEasingKeyFrame(Duration/2, width/2, new QuadraticEase())
                    .AddEasingKeyFrame(Duration, 0.0, new QuadraticEase {EasingMode = EasingMode.EaseInOut}),
                element.AnimateObjectProperty(AnimationProperty.ZIndex)
                    .AddDiscreteKeyFrame(Duration/2, 0),
                element.AnimateProperty(AnimationProperty.CentreOfRotationX)
                    .AddDiscreteKeyFrame(0.0, 0),
                element.AnimateProperty(AnimationProperty.RotationY)
                    .AddEasingKeyFrame(0, 0)
                    .AddEasingKeyFrame(Duration/2, -25, new QuadraticEase {EasingMode = EasingMode.EaseOut})
                    .AddEasingKeyFrame(Duration, 0),
                element.AnimateProperty(AnimationProperty.ScaleX)
                    .AddEasingKeyFrame(0.0, 1)
                    .AddEasingKeyFrame(Duration, 0.95), //, new CubicEase { EasingMode = EasingMode.EaseIn });
                element.AnimateProperty(AnimationProperty.ScaleY)
                    .AddEasingKeyFrame(0.0, 1)
                    .AddEasingKeyFrame(Duration, 0.95), //, new CubicEase { EasingMode = EasingMode.EaseIn });

                element.AnimateProperty(AnimationProperty.Opacity)
                    .AddEasingKeyFrame(Duration*0.99, 1)
                    .AddEasingKeyFrame(Duration, 0)
            };
        }
    }
}