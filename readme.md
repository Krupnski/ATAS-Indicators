# ATAS Indicators

Custom indicators for the ATAS trading platform.

## ES Levels Indicator

### Overview

The ES Levels Indicator dynamically marks 25-point level intervals on ES futures charts based on a manually entered ratio. This indicator converts SPX values to ES futures levels, similar to MenthorQ's level conversion approach.

### Features

- **Dynamic Level Calculation**: Automatically calculates 25-point intervals based on current price
- **SPX to ES Conversion**: Uses a customizable ratio to convert SPX levels to ES futures
- **Visual Display**: Draws horizontal lines at each level with price labels
- **Configurable Parameters**:
  - **Ratio**: SPX to ES conversion ratio (default: 1.005)
  - **Level Color**: Color for the level lines
  - **Line Width**: Thickness of the level lines (1-5)
  - **Levels Above**: Number of levels to display above current price (1-50)
  - **Levels Below**: Number of levels to display below current price (1-50)

### Installation

1. **Prerequisites**: 
   - ATAS Platform installed
   - .NET 8.0 SDK or later
   - Visual Studio 2022 or JetBrains Rider

2. **Build the Indicator**:
   ```bash
   # Clone the repository
   git clone https://github.com/Krupnski/ATAS-Indicators.git
   cd ATAS-Indicators
   
   # Update the .csproj file to reference your ATAS.Indicators.dll
   # Edit ESLevelsIndicator.csproj and uncomment the Reference section
   # Update the path to point to your ATAS installation directory
   
   # Build the project
   dotnet build -c Release
   ```

3. **Install in ATAS**:
   - Copy the compiled `ESLevelsIndicator.dll` from `bin/Release/net8.0/` to your ATAS custom indicators folder
   - Default location: `Documents\ATAS\Indicators\`
   - Restart ATAS if it's running

### Usage

1. Open ATAS and load an ES futures chart
2. Add the "ES Levels Indicator" from the indicator list
3. Configure the parameters:
   - **Ratio**: Adjust based on current SPX/ES spread (typically between 1.000 and 1.010)
   - Set the number of levels to display above and below current price
   - Customize the visual appearance (color, line width)

### How It Works

The indicator uses the following logic:

1. **Ratio-Based Conversion**: The indicator uses the formula `ES Level = SPX Level × Ratio`
2. **Level Calculation**: 
   - Takes the current ES price and converts it back to SPX equivalent: `SPX = ES / Ratio`
   - Rounds to the nearest 25-point interval
   - Generates levels at 25-point increments above and below
   - Converts each SPX level to ES using the ratio
3. **Visual Rendering**: Draws horizontal lines at calculated ES levels across the visible chart range

### Example

If the current ES price is 5000 and the ratio is 1.005:
- SPX equivalent: 5000 / 1.005 ≈ 4975
- Base level (rounded): 4975
- Levels generated: 4925, 4950, 4975, 5000, 5025, 5050, etc.
- ES levels: 4949.63, 4975.25, 5000.88, 5026.50, 5052.13, 5077.75, etc.

### Understanding the Ratio

The ratio represents the conversion factor between SPX and ES futures:
- **Spread Method**: ES Price - SPX Price = Spread (e.g., 25 points)
- **Ratio Method**: ES Price / SPX Price = Ratio (e.g., 1.005)

The ratio varies based on:
- Interest rates
- Expected dividends
- Time to expiration
- Market conditions

Update the ratio periodically for accurate level placement.

### Technical Details

- **Base Class**: Inherits from `ATAS.Indicators.Indicator`
- **Framework**: .NET 8.0
- **API Methods**:
  - `OnCalculate`: Computes levels on each bar update
  - `OnRender`: Draws visual representation on the chart
- **Error Handling**: Includes try-catch blocks for edge cases
- **Resource Management**: Properly disposes of graphics resources

### Troubleshooting

**Levels not appearing:**
- Ensure the indicator is added to the correct chart panel
- Check that the ratio value is valid (between 0.9 and 1.1)
- Verify the number of levels above/below is greater than 0

**Incorrect level placement:**
- Update the ratio to match current market conditions
- Calculate current ratio: ES Price / SPX Price
- Typical range: 1.000 - 1.010

**Build errors:**
- Ensure ATAS.Indicators.dll reference path is correct in .csproj
- Verify .NET 8.0 SDK is installed
- Check that all using statements are resolved

### Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

### License

This project is open source. Please check the repository for license details.

### References

- [ATAS Indicator Development Guide](https://docs.atas.net/en/md_DataFeedsCore_2Docs_2en_20010__BasicIndicator.html)
- [ATAS Drawing API](https://docs.atas.net/en/md_DataFeedsCore_2Docs_2en_20070__Graphics.html)
- [MenthorQ Levels Conversion](https://menthorq.com/guide/levels-conversion/)
- [ATAS GitHub Indicators](https://github.com/AtasPlatform/Indicators)
