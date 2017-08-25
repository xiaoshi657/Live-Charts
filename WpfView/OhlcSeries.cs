﻿//The MIT License(MIT)

//Copyright(c) 2016 Alberto Rodriguez Orozco & LiveCharts Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using LiveCharts.Configurations;
using LiveCharts.Definitions.Points;
using LiveCharts.Definitions.Series;
using LiveCharts.Helpers;
using LiveCharts.Series;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Wpf.Points;

namespace LiveCharts.Wpf
{
    /// <summary>
    /// The OHCL series defines a financial series, add this series to a cartesian chart
    /// </summary>
    public class OhlcSeries : Series, IFinancialSeriesView
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of OhclSeries class
        /// </summary>
        public OhlcSeries()
        {
            Core = new OhlcCore(this);
            InitializeDefuaults();
        }

        /// <summary>
        /// Initializes a new instance of OhclSeries class with a given mapper
        /// </summary>
        /// <param name="configuration"></param>
        public OhlcSeries(BiDimensinalMapper configuration)
        {
            Core = new OhlcCore(this);
            Configuration = configuration;
            InitializeDefuaults();
        }

        #endregion

        #region Private Properties

        #endregion

        #region Properties

        /// <summary>
        /// The maximum column width property
        /// </summary>
        public static readonly DependencyProperty MaxColumnWidthProperty = DependencyProperty.Register(
            "MaxColumnWidth", typeof (double), typeof (OhlcSeries), new PropertyMetadata(default(double)));
        /// <summary>
        /// Gets or sets the maximum with of a point, a point will be capped to this width.
        /// </summary>
        public double MaxColumnWidth
        {
            get { return (double) GetValue(MaxColumnWidthProperty); }
            set { SetValue(MaxColumnWidthProperty, value); }
        }

        /// <summary>
        /// The increase brush property
        /// </summary>
        public static readonly DependencyProperty IncreaseBrushProperty = DependencyProperty.Register(
            "IncreaseBrush", typeof (Brush), typeof (OhlcSeries), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets the brush of the point when close value is grater than open value
        /// </summary>
        public Brush IncreaseBrush
        {
            get { return (Brush) GetValue(IncreaseBrushProperty); }
            set { SetValue(IncreaseBrushProperty, value); }
        }

        /// <summary>
        /// The decrease brush property
        /// </summary>
        public static readonly DependencyProperty DecreaseBrushProperty = DependencyProperty.Register(
            "DecreaseBrush", typeof (Brush), typeof (OhlcSeries), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// Gets or sets the brush of the point when close value is less than open value
        /// </summary>
        public Brush DecreaseBrush
        {
            get { return (Brush) GetValue(DecreaseBrushProperty); }
            set { SetValue(DecreaseBrushProperty, value); }
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// This method runs when the update starts
        /// </summary>
        protected override void OnSeriesUpdateStart()
        {
            //do nothing on updateStart
        }

        /// <summary>
        /// Gets the point view.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        protected override IChartPointView GetPointView(ChartPoint point, string label)
        {
            var pbv = (OhlcPointView)point.View;

            if (pbv == null)
            {
                pbv = new OhlcPointView
                {
                    IsNew = true,
                    HighToLowLine = new Line(),
                    OpenLine = new Line(),
                    CloseLine = new Line()
                };

                Core.Chart.View.AddToDrawMargin(pbv.HighToLowLine);
                Core.Chart.View.AddToDrawMargin(pbv.OpenLine);
                Core.Chart.View.AddToDrawMargin(pbv.CloseLine);
            }
            else
            {
                pbv.IsNew = false;
                point.SeriesView.Core.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.HighToLowLine);
                point.SeriesView.Core.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.OpenLine);
                point.SeriesView.Core.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.CloseLine);
                point.SeriesView.Core.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.DataLabel);
            }

            pbv.HighToLowLine.StrokeThickness = StrokeThickness;
            pbv.CloseLine.StrokeThickness = StrokeThickness;
            pbv.OpenLine.StrokeThickness = StrokeThickness;

            pbv.HighToLowLine.StrokeDashArray = StrokeDashArray;
            pbv.CloseLine.StrokeDashArray = StrokeDashArray;
            pbv.OpenLine.StrokeDashArray = StrokeDashArray;

            pbv.HighToLowLine.Visibility = Visibility;
            pbv.CloseLine.Visibility = Visibility;
            pbv.OpenLine.Visibility = Visibility;

            var i = Panel.GetZIndex(this);
            Panel.SetZIndex(pbv.HighToLowLine, i);
            Panel.SetZIndex(pbv.CloseLine, i);
            Panel.SetZIndex(pbv.OpenLine, i);

            //if (Core.Chart.View.RequiresHoverShape && pbv.HoverShape == null)
            //{
            //    pbv.HoverShape = new Rectangle
            //    {
            //        Fill = Brushes.Transparent,
            //        StrokeThickness = 0
            //    };

            //    Panel.SetZIndex(pbv.HoverShape, int.MaxValue);
            //    Core.Chart.View.EnableHoveringFor(pbv.HoverShape);
            //    Core.Chart.View.AddToDrawMargin(pbv.HoverShape);
            //}

            //if (pbv.HoverShape != null) pbv.HoverShape.Visibility = Visibility;

            if (DataLabels)
            {
                pbv.DataLabel = UpdateLabelContent(new DataLabelViewModel
                {
                    FormattedText = label,
                    Point = point
                }, pbv.DataLabel);
            }

            if (!DataLabels && pbv.DataLabel != null)
            {
                Core.Chart.View.RemoveFromDrawMargin(pbv.DataLabel);
                pbv.DataLabel = null;
            }

            if (point.Open < point.Close)
            {
                pbv.HighToLowLine.Stroke = IncreaseBrush;
                pbv.CloseLine.Stroke = IncreaseBrush;
                pbv.OpenLine.Stroke = IncreaseBrush;
            }
            else
            {
                pbv.HighToLowLine.Stroke = DecreaseBrush;
                pbv.CloseLine.Stroke = DecreaseBrush;
                pbv.OpenLine.Stroke = DecreaseBrush;
            }

            return pbv;
        }

        #endregion

        #region Private Methods

        private void InitializeDefuaults()
        {
            SetCurrentValue(StrokeThicknessProperty, 2.5d);
            SetCurrentValue(MaxColumnWidthProperty, 35d);
            SetCurrentValue(MaxWidthProperty, 25d);
            SetCurrentValue(IncreaseBrushProperty, new SolidColorBrush(Color.FromRgb(76, 174, 80)));
            SetCurrentValue(DecreaseBrushProperty, new SolidColorBrush(Color.FromRgb(238, 83, 80)));

            Func<ChartPoint, string> defaultLabel = x => $"O: {x.Open}, H: {x.High}, L: {x.Low} C: {x.Close}";

            SetCurrentValue(LabelPointProperty, defaultLabel);

            DefaultFillOpacity = 1;
        }

        #endregion
    }
}
