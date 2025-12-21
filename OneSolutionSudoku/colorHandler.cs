using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace OneSolutionSudoku
{
	static class colorHandler
	{
		public static SolidColorBrush defaultPrimaryColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d9d9d9"));
		public static SolidColorBrush defaultSecondaryColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b3b3b3"));
		public static SolidColorBrush defaultBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#21211f"));
		public static SolidColorBrush PrimaryColor { get; set; }
		public static SolidColorBrush SecondaryColor { get; set; }
		public static SolidColorBrush BackgroundColor { get; set; }

		public static void loadColors(string hexPrimaryColor, string hexSecondaryColor, string hexBackgroundColor)
		{
			string loadedHexPrimaryColor = settingsHandler.LoadSetting("Primary color");
			string loadedHexSecondaryColor = settingsHandler.LoadSetting("Secondary color");
			string loadedHexBackgroundColor = settingsHandler.LoadSetting("Background color");
			if (Regex.IsMatch(loadedHexPrimaryColor, @"^#[0-9A-Fa-f]{6}$"))
			{
				colorHandler.PrimaryColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(loadedHexPrimaryColor));
			}
			else
			{
				colorHandler.PrimaryColor = defaultPrimaryColor;
			}
			if (Regex.IsMatch(loadedHexSecondaryColor, @"^#[0-9A-Fa-f]{6}$"))
			{
				colorHandler.SecondaryColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(loadedHexSecondaryColor));
			}
			else
			{
				colorHandler.SecondaryColor = defaultSecondaryColor;
			}
			if (Regex.IsMatch(loadedHexBackgroundColor, @"^#[0-9A-Fa-f]{6}$"))
			{
				colorHandler.BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(loadedHexBackgroundColor));
			}
			else
			{
				colorHandler.BackgroundColor = defaultBackgroundColor;
			}
		}
	}
}
