# ATAS Indicators

This repository contains custom indicators for the ATAS trading platform.

## Indicators

### 25-Point Levels Indicator

Marks 25-point level intervals on an ES futures chart based on a user-provided ratio.

**Features:**
- User-configurable ratio parameter to convert SPX 25-point levels to ES levels
- Draws horizontal lines at calculated level intervals
- Automatically adjusts levels around the current price range

**Parameters:**
- **Ratio** (default: 10.0): The conversion ratio from SPX to ES. For example, if the ratio is set to 10, SPX levels at 5975, 6000, 6025 will convert to ES levels at 597.5, 600.0, 602.5.

## Building the Indicator

### Prerequisites
- Visual Studio 2022 or later (or .NET 8.0 SDK)
- ATAS platform installed
- ATAS.Indicators.dll (located in your ATAS installation directory)

### Build Steps

1. Ensure `ATAS.Indicators.dll` is available in your ATAS installation directory (typically `%APPDATA%\ATAS\`)

2. Build the project:
   ```bash
   dotnet build
   ```

3. Copy the compiled DLL to the ATAS indicators directory:
   ```bash
   copy bin\Debug\net8.0\CustomIndicators.dll %APPDATA%\ATAS\Indicators\
   ```

4. Restart ATAS or reload indicators

## Usage

1. Open ATAS platform
2. Add the "25-Point Levels" indicator to your chart
3. Configure the Ratio parameter based on your needs:
   - For standard ES/SPX conversion, use 10.0
   - Adjust as needed for different instruments or scaling preferences
4. The indicator will draw horizontal lines at 25-point intervals (adjusted by your ratio)

## Development

The indicator is written in C# and uses the ATAS Indicators API. It follows standard ATAS development practices:

- Inherits from `Indicator` base class
- Uses `LineSeries` for drawing horizontal levels
- Implements parameter validation with `[Display]` and `[Range]` attributes
- Supports dynamic recalculation when parameters change

## License

This project is open source. Please refer to the repository license for details.
