using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OneSolutionSudoku
{
	public static class languageHandler
	{
		public static EventHandler<string> ChangeLanguage;

		private static readonly Dictionary<string, string> czechLanguage = new Dictionary<string, string>
		{
			{"TextBox_Title", "Generátor Sudoku s jedním řešením"},
			{"Button_End", "Opustit"},
			{"Button_OwnCreation", "Vlastní tvorba"},
			{"Button_Generation", "Generovat"},
			{"Button_Options", "Možnosti"},
			{"TextBlock_DefaultSaveLocation", "Výchozí úložiště" },
			{"TextBlock_ColorsSelection", "Barvy"},
			{"TextBlock_LanguageSelection", "Jazyk"},
			{"Button_Confirm", "Potvrdit"},
			{"Button_Back", "Zpět"},
			{"Languague_Czech", "Čeština"},
			{"Languague_English", "Angličtina"}
		};
		private static readonly Dictionary<string, string> englishLanguage = new Dictionary<string, string>
		{
			{"TextBox_Title", "One Solution Sudoku Generator"},
			{"Button_End", "Quit"},
			{"Button_OwnCreation", "Create"},
			{"Button_Generation", "Generate"},
			{"Button_Options", "Options"},
			{"TextBlock_DefaultSaveLocation", "Default save file" },
			{"TextBlock_ColorsSelection", "Colors"},
			{"TextBlock_LanguageSelection", "Language"},
			{"Button_Confirm", "Confirm"},
			{"Button_Back", "Back"},
			{"Languague_Czech", "Czech"},
			{"Languague_English", "English"}
		};
		public static readonly Dictionary<string, string> languageNames = new Dictionary<string, string>
		{
			{"cs", "Čeština"},
			{"en", "English"}
		};
		public static readonly Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>
		{
			{"cs", czechLanguage },
			{"en", englishLanguage }
		};
		public static void SetLanguage()
		{
			ChangeLanguage?.Invoke("langSender", selectedLanguage);
		}
		public static string defaultLanguage = "en";
		private static string? _selectedLanguage;
		public static string selectedLanguage
		{
			get => _selectedLanguage;
			set
			{
				if(value == null)
				{
					_selectedLanguage = defaultLanguage;
				}
				else if (languageNames.ContainsKey(value))
				{
					_selectedLanguage = value;
				}
				else
				{
					_selectedLanguage = defaultLanguage;
				}
			}
		}
		public static void loadLanguage()
		{
			languageHandler.selectedLanguage = settingsHandler.LoadSetting("language");
		}
	}
}
