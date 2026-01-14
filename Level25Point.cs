namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using ATAS.Indicators.Drawing;

	[DisplayName("25-Point Levels")]
	[Description("Marks 25-point level intervals on an ES futures chart based on a user-provided ratio")]
	public class Level25Point : Indicator
	{
		#region Fields

		private decimal _ratio = 10.0m;
		private int _lastCalculatedBar = -1;
		private bool _levelsInitialized = false;

		#endregion

		#region Properties

		[Display(Name = "Ratio", 
			GroupName = "Settings", 
			Description = "Ratio to convert 25-point SPX levels to ES levels (e.g., 10 means SPX/10 = ES)",
			Order = 10)]
		[Range(0.01, 1000.0)]
		public decimal Ratio
		{
			get => _ratio;
			set
			{
				if (_ratio != value)
				{
					_ratio = Math.Max(0.01m, value);
					_levelsInitialized = false;
					RecalculateValues();
				}
			}
		}

		#endregion

		#region Constructor

		public Level25Point()
			: base(true)
		{
			DenyToChangePanel = true;
			EnableCustomDrawing = true;
			SubscribeToDrawingEvents(DrawingLayouts.Historical);
		}

		#endregion

		#region Protected methods

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar != CurrentBar - 1)
				return;

			// Initialize levels only once or when ratio changes
			if (!_levelsInitialized || bar != _lastCalculatedBar)
			{
				_lastCalculatedBar = bar;
				InitializeLevels();
			}
		}

		protected override void OnRender(RenderContext context, DrawingLayouts layout)
		{
			if (ChartInfo == null || Container == null)
				return;

			// Levels are drawn via LineSeries, so no custom rendering needed
		}

		#endregion

		#region Private methods

		private void InitializeLevels()
		{
			if (_levelsInitialized)
				return;

			// Clear existing levels
			LineSeries.Clear();

			// Get current price to determine which levels to draw
			var currentPrice = (decimal)GetCandle(CurrentBar - 1).Close;

			// Calculate the 25-point interval for ES based on the ratio
			// SPX 25-point levels are: ..., 5975, 6000, 6025, 6050, ...
			// If ratio = 10, ES levels would be: ..., 597.5, 600.0, 602.5, 605.0, ...
			var levelInterval = 25.0m / _ratio;

			// Determine a range around current price to draw levels
			// Draw levels from 10 intervals below to 10 intervals above current price
			var baseLevel = Math.Floor(currentPrice / levelInterval) * levelInterval;
			var numberOfLevels = 21; // 10 below, current, 10 above

			for (int i = -10; i <= 10; i++)
			{
				var levelPrice = baseLevel + (i * levelInterval);

				var line = new LineSeries($"Level_{levelPrice:F2}")
				{
					Value = (double)levelPrice,
					Color = System.Drawing.Color.Gray,
					Width = 1,
					LineDashStyle = LineDashStyle.Solid,
					IsHidden = false
				};

				LineSeries.Add(line);
			}

			_levelsInitialized = true;
		}

		#endregion
	}
}
