using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Brain.Controls
{
    public class SquareUserControl : UserControl
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var parentGrid = Parent as Grid;
            if (parentGrid != null)
            {
                if ((parentGrid.ColumnDefinitions.Count > 0) &&
                    (parentGrid.RowDefinitions.Count > 0))
                {
                    double columnWidth = parentGrid.ColumnDefinitions[0].ActualWidth;
                    double rowHeight = parentGrid.RowDefinitions[0].ActualHeight;
                    double len = Math.Min(columnWidth, rowHeight);
                    availableSize = new Size(len, len);
                }
            }

            Size baseSize = base.MeasureOverride(availableSize);
            double sideLength = Math.Max(baseSize.Width, baseSize.Height);
            return new Size(sideLength, sideLength);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double sideLength = Math.Min(finalSize.Width, finalSize.Height);
            Size result = base.ArrangeOverride(new Size(sideLength, sideLength));
            return result;
        }
    }
}