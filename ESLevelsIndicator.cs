using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Collections.Generic;

namespace ATAS.Indicators.Custom
{
    /// <summary>
    /// ES Futures Level Indicator
    /// Dynamically marks 25-point level intervals on ES futures charts based on a manually entered ratio.
    /// Converts SPX values to ES futures levels similar to MenthorQ's approach.
    /// </summary>
    [DisplayName("ES Levels Indicator")]
    [Description("Marks 25-point level intervals on ES futures based on SPX conversion ratio")]
    public class ESLevelsIndicator : Indicator
    {
        #region Fields

        private decimal _ratio = 1.005m;
        private readonly List<decimal> _calculatedLevels = new List<decimal>();
        private const decimal LEVEL_INTERVAL = 25.0m;
        private readonly Pen _levelPen;

        #endregion

        #region Properties

        /// <summary>
        /// Ratio for converting SPX values to ES futures levels.
        /// Default is 1.005, which represents the typical spread between SPX and ES.
        /// Users can adjust this based on current market conditions.
        /// </summary>
        [Display(Name = "SPX to ES Ratio", 
                 Description = "Conversion ratio from SPX to ES futures (e.g., 1.005)", 
                 GroupName = "Parameters", 
                 Order = 10)]
        [Range(0.9, 1.1)]
        public decimal Ratio
        {
            get => _ratio;
            set
            {
                if (_ratio == value)
                    return;

                _ratio = value;
                RecalculateValues();
            }
        }

        /// <summary>
        /// Color for the level lines
        /// </summary>
        [Display(Name = "Level Color", 
                 Description = "Color for the 25-point level lines", 
                 GroupName = "Visualization", 
                 Order = 20)]
        public Color LevelColor { get; set; } = Color.DodgerBlue;

        /// <summary>
        /// Line width for the level lines
        /// </summary>
        [Display(Name = "Line Width", 
                 Description = "Width of the level lines", 
                 GroupName = "Visualization", 
                 Order = 30)]
        [Range(1, 5)]
        public int LineWidth { get; set; } = 1;

        /// <summary>
        /// Number of levels to display above current price
        /// </summary>
        [Display(Name = "Levels Above", 
                 Description = "Number of 25-point levels to show above current price", 
                 GroupName = "Parameters", 
                 Order = 40)]
        [Range(1, 50)]
        public int LevelsAbove { get; set; } = 10;

        /// <summary>
        /// Number of levels to display below current price
        /// </summary>
        [Display(Name = "Levels Below", 
                 Description = "Number of 25-point levels to show below current price", 
                 GroupName = "Parameters", 
                 Order = 50)]
        [Range(1, 50)]
        public int LevelsBelow { get; set; } = 10;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ESLevelsIndicator class
        /// </summary>
        public ESLevelsIndicator()
        {
            // Enable custom drawing to use OnRender
            EnableCustomDrawing = true;

            // Subscribe to drawing events for rendering levels
            SubscribeToDrawingEvents(DrawingLayouts.Final);

            // Initialize the pen for drawing levels
            _levelPen = new Pen(LevelColor, LineWidth);

            // Set the panel to overlay on the main chart
            Panel = IndicatorDataProvider.CandlePanel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called on each bar calculation. Computes the levels based on current price and ratio.
        /// </summary>
        /// <param name="bar">Bar index</param>
        /// <param name="value">Bar value</param>
        protected override void OnCalculate(int bar, decimal value)
        {
            try
            {
                // Only calculate on the last bar to get current price
                if (bar != CurrentBar - 1)
                    return;

                // Validate ratio
                if (_ratio <= 0)
                {
                    AddAlert("Warning", "Invalid ratio value. Using default 1.005");
                    _ratio = 1.005m;
                }

                // Get current price (close of current bar)
                var currentPrice = (decimal)GetCandle(bar).Close;

                // Calculate levels
                CalculateLevels(currentPrice);
            }
            catch (Exception ex)
            {
                // Error handling - log the error
                AddAlert("Error", $"Error in OnCalculate: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates the 25-point level intervals around the current price
        /// </summary>
        /// <param name="currentPrice">Current ES futures price</param>
        private void CalculateLevels(decimal currentPrice)
        {
            try
            {
                _calculatedLevels.Clear();

                // Convert current ES price back to SPX equivalent for baseline
                // Then round to nearest 25-point interval
                var spxEquivalent = currentPrice / _ratio;
                var baseLevel = Math.Round(spxEquivalent / LEVEL_INTERVAL) * LEVEL_INTERVAL;

                // Calculate levels above and below
                for (int i = -LevelsBelow; i <= LevelsAbove; i++)
                {
                    var spxLevel = baseLevel + (i * LEVEL_INTERVAL);
                    var esLevel = spxLevel * _ratio;
                    _calculatedLevels.Add(esLevel);
                }
            }
            catch (Exception ex)
            {
                AddAlert("Error", $"Error calculating levels: {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the level lines on the chart
        /// </summary>
        /// <param name="context">Rendering context</param>
        /// <param name="layout">Drawing layout</param>
        protected override void OnRender(RenderContext context, DrawingLayouts layout)
        {
            try
            {
                if (context == null || ChartInfo == null)
                    return;

                // Update pen color and width in case user changed settings
                _levelPen.Color = LevelColor;
                _levelPen.Width = LineWidth;

                // Get visible bar range
                var firstVisibleBar = ChartInfo.FirstVisibleBarNumber;
                var lastVisibleBar = ChartInfo.LastVisibleBarNumber;

                if (firstVisibleBar < 0 || lastVisibleBar < 0)
                    return;

                // Calculate X coordinates for the visible range
                var x1 = ChartInfo.GetXByBar(firstVisibleBar);
                var x2 = ChartInfo.GetXByBar(lastVisibleBar);

                // Draw each level as a horizontal line
                foreach (var level in _calculatedLevels)
                {
                    try
                    {
                        // Convert price level to Y coordinate
                        var y = ChartInfo.GetYByPrice((double)level);

                        // Check if the level is within visible chart area
                        if (y >= 0 && y <= ChartInfo.Region.Height)
                        {
                            // Draw the horizontal line
                            context.DrawLine(_levelPen, x1, y, x2, y);

                            // Optionally draw the level price as text (small label)
                            var levelText = level.ToString("F2");
                            var font = new Font("Arial", 8);
                            var textBrush = new SolidBrush(LevelColor);
                            context.DrawString(levelText, font, textBrush, x2 - 50, y - 15);
                        }
                    }
                    catch
                    {
                        // Skip individual level if there's an error drawing it
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                // Error handling - log but don't crash the rendering
                AddAlert("Error", $"Error in OnRender: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup resources when indicator is disposed
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _levelPen?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
