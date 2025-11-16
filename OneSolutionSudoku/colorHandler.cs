using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
			if (loadedHexPrimaryColor != null)
			{
				colorHandler.PrimaryColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(loadedHexPrimaryColor));
			}
			else
			{
				colorHandler.PrimaryColor = defaultPrimaryColor;
			}
			if (loadedHexSecondaryColor != null)
			{
				colorHandler.SecondaryColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(loadedHexSecondaryColor));
			}
			else
			{
				colorHandler.SecondaryColor = defaultSecondaryColor;
			}
			if (loadedHexBackgroundColor != null)
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
