using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSolutionSudoku
{
	public static class settingsHandler
	{
		private static string fileName = "settings.ini";
		private static char delimeter = '?';
		public static void SaveSettings()
		{
			FileStream settingsFile = File.Create(fileName);
			StreamWriter writer = new StreamWriter(settingsFile);
			writer.WriteLine($"language{delimeter}{languageHandler.selectedLanguage}");
			writer.WriteLine($"Primary color{delimeter}#{colorHandler.PrimaryColor.Color.R:X2}{colorHandler.PrimaryColor.Color.G:X2}{colorHandler.PrimaryColor.Color.B:X2}");
			writer.WriteLine($"Secondary color{delimeter}#{colorHandler.SecondaryColor.Color.R:X2}{colorHandler.SecondaryColor.Color.G:X2}{colorHandler.SecondaryColor.Color.B:X2}");
			writer.WriteLine($"Background color{delimeter}#{colorHandler.BackgroundColor.Color.R:X2}{colorHandler.BackgroundColor.Color.G:X2}{colorHandler.BackgroundColor.Color.B:X2}");
			writer.WriteLine($"Save location{delimeter}{SudokuSavingHandler.saveLocation}");
			writer.Close();
			settingsFile.Close();
		}
		public static string LoadSetting(string key)
		{
			StreamReader settingsFile;
			try
			{
				settingsFile = new(fileName);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
			string line = settingsFile.ReadLine();
			while (line != null)
			{
				string[] splitLine = line.Split(delimeter);
				if (splitLine[0] == key)
				{
					settingsFile.Close();
					return splitLine[1];	
				}
				line = settingsFile.ReadLine();
			}
			settingsFile.Close();
			return null;
		}
	}
}
