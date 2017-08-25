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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LiveCharts.Configurations;
using LiveCharts.Definitions.Series;
using LiveCharts.Series;

namespace LiveCharts.Wpf
{
    /// <summary>
    /// Compares trend and proportion, this series must be added in a cartesian chart.
    /// </summary>
    public class VerticalStackedAreaSeries : VerticalLineSeries, IVerticalStackedAreaSeriesView
    {
        #region Fields

        private int _activeSplitters;
        private int _splittersCollector;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of VerticalStackedAreaSeries class
        /// </summary>
        public VerticalStackedAreaSeries()
        {
            Core = new VerticalStackedAreaCore(this);
            InitializeDefuaults();
        }

        /// <summary>
        /// Initializes a new instance of VerticalStackedAreaSeries class, with a given mapper
        /// </summary>
        public VerticalStackedAreaSeries(BiDimensinalMapper configuration)
        {
            Core = new VerticalStackedAreaCore(this);
            Configuration = configuration;
            InitializeDefuaults();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The stack mode property
        /// </summary>
        public static readonly DependencyProperty StackModeProperty = DependencyProperty.Register(
            "StackMode", typeof (StackMode), typeof (VerticalStackedAreaSeries), new PropertyMetadata(default(StackMode)));
        /// <summary>
        /// Gets or sets the series stack mode, values or percentage
        /// </summary>
        public StackMode StackMode
        {
            get { return (StackMode) GetValue(StackModeProperty); }
            set { SetValue(StackModeProperty, value); }
        }
        #endregion

        #region Overridden Methods

        /// <summary>
        /// This method runs when the update starts
        /// </summary>
        protected override void OnSeriesUpdateStart()
        {
            _activeSplitters = 0;

            if (_splittersCollector == int.MaxValue - 1)
            {
                //just in case!
                PathCollection.ForEach(s => s.SplitterCollectorIndex = 0);
                _splittersCollector = 0;
            }

            _splittersCollector++;

            if (Figure != null && Values != null)
            {
                var yIni = ChartFunctions.ToDrawMargin(Values.GetTracker(this).YLimit.Min, AxisOrientation.Y, Core.Chart, ScalesYAt);

                if (Core.Chart.View.DisableAnimations)
                    Figure.StartPoint = new Point(0, yIni);
                else
                    Figure.BeginAnimation(PathFigure.StartPointProperty,
                        new PointAnimation(new Point(0, yIni),
                            Core.Chart.View.AnimationsSpeed));
            }

            if (IsPathInitialized)
            {
                Core.Chart.View.EnsureElementBelongsToCurrentDrawMargin(Path);
                Path.Stroke = Stroke;
                Path.StrokeThickness = StrokeThickness;
                Path.Fill = Fill;
                Path.Visibility = Visibility;
                Path.StrokeDashArray = StrokeDashArray;
                return;
            }

            IsPathInitialized = true;

            Path = new Path
            {
                Stroke = Stroke,
                StrokeThickness = StrokeThickness,
                Fill = Fill,
                Visibility = Visibility,
                StrokeDashArray = StrokeDashArray
            };

            Panel.SetZIndex(Path, Panel.GetZIndex(this));

            var geometry = new PathGeometry();
            Figure = new PathFigure();
            geometry.Figures.Add(Figure);
            Path.Data = geometry;
            Core.Chart.View.AddToDrawMargin(Path);

            var y = ChartFunctions.ToDrawMargin(((ISeriesView) this).ActualValues.GetTracker(this).YLimit.Min, AxisOrientation.Y, Core.Chart, ScalesYAt);
            Figure.StartPoint = new Point(0, y);

            var i = Core.Chart.View.Series.IndexOf(this);
            Panel.SetZIndex(Path, Core.Chart.View.Series.Count - i);
        }

        #endregion

        #region Public Methods 


        #endregion

        #region Private Methods

        private void InitializeDefuaults()
        {
            SetCurrentValue(LineSmoothnessProperty, .7d);
            SetCurrentValue(PointGeometrySizeProperty, 0d);
            SetCurrentValue(PointForegroundProperty, Brushes.White);
            SetCurrentValue(ForegroundProperty, new SolidColorBrush(Color.FromRgb(229, 229, 229)));
            SetCurrentValue(StrokeThicknessProperty, 0d);
            SetCurrentValue(StackModeProperty, StackMode.Values);

            Func<ChartPoint, string> defaultLabel = x => Core.CurrentXAxis.GetFormatter()(x.X);
            SetCurrentValue(LabelPointProperty, defaultLabel);

            DefaultFillOpacity = 1;
            PathCollection = new List<LineSeriesPathHelper>();
        }

        #endregion
    }
}
