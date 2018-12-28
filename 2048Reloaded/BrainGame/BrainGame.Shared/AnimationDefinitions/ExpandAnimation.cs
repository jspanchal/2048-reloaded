using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Brain.Animate;

namespace BrainGame.AnimationDefinitions
{
    public class ExpandAnimation : AnimationDefinition
    {
        public double FinalScale = 1.1;

        public override IEnumerable<Timeline> CreateAnimation(FrameworkElement element)
        {
            return new Timeline[]
            {
                element.AnimateProperty(AnimationProperty.ScaleX)
                    .AddEasingKeyFrame(Duration, FinalScale, new CubicEase()),
                element.AnimateProperty(AnimationProperty.ScaleY)
                    .AddEasingKeyFrame(0.0, 1)
                    .AddEasingKeyFrame(Duration, FinalScale, new CubicEase())
            };
        }
    }
}