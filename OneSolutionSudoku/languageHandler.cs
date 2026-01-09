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
			{"Label_Title", "Sudoku App"},
			{"Button_End", "Opustit"},
			{"Button_OwnCreation", "Vlastní tvorba"},
			{"Button_Generation", "Generovat"},
			{"Button_Options", "Možnosti"},
			{"Label_ColorsSelection", "Barvy"},
			{"Label_LanguageSelection", "Jazyk"},
			{"Button_Confirm", "Potvrdit"},
			{"Button_Back", "Zpět"},
			{"Button_FolderSelect", "Vyber základní složku"},
			{"Button_Defaults", "Původní nastavení"},
			{"Button_Generate", "Generovat"},
			{"Label_FullCellAmountLabel", "Počet vyplněných polí"},
			{"Label_SudokuNameLabel", "Název Sudoku souboru"},
			{"Button_Check", "Zkontrolovat"},
			{"Button_Save", "Uložit"},
			{"Button_Load", "Načti" },
			{"Alert_InvalidGeneration_Input", "Počet plných polí musí být mezi 17 a 81"},
			{"Alert_Incorrect_Input", "Zadaná hodnota není číslo"},
			{"Message_SudokuCheck_OneSolution", "Sudoku má jedno řešení"},
			{"Message_SudokuCheck_MoreSolutions", "Sudoku má více než jedno řešení" },
			{"Message_SudokuCheck_Invalid", "Sudoku není validní"},
			{"Message_SudokuCheck_NoSolution", "Sudoku nemá řešení" },
			{"Button_Stop", "Zastavit"},
			{"Alert_Cancelled_Input", "Generace zastavena"},
			{"Button_Solve", "Vyřešit"},
			{"Label_PrimaryColor", "Primární barva" },
			{"Label_SecondaryColor", "Interakční barva" },
			{"Label_BackgroundColor", "Barva pozadí" },
			{"Label_DefaultPathLocation", "Výchozí složka úložiště"},
			{"Message_LoadFile_Failed", "Nemohl jsem načíst data z vybraného souboru"},
			{"Button_Clear", "Smazat"  }

		};
		private static readonly Dictionary<string, string> englishLanguage = new Dictionary<string, string>
		{
			{"Label_Title", "Sudoku App"},
			{"Button_End", "Quit"},
			{"Button_OwnCreation", "Create"},
			{"Button_Generation", "Generate"},
			{"Button_Options", "Options"},
			{"Label_ColorsSelection", "Colors"},
			{"Label_LanguageSelection", "Language"},
			{"Button_Confirm", "Confirm"},
			{"Button_Back", "Back"},
			{"Button_FolderSelect", "Select default folder"},
			{"Button_Defaults", "Default settings"},
			{"Button_Generate", "Generate"},
			{"Label_FullCellAmountLabel", "Amount of full fields"},
			{"Label_SudokuNameLabel", "Sudoku file name"},
			{"Button_Check", "Check"},
			{"Button_Save", "Save"},
			{"Button_Load", "Load" },
			{"Alert_InvalidGeneration_Input", "Amount of full cells must be between 17 and 81"},
			{"Alert_Incorrect_Input", "Invalid number value"},
			{"Message_SudokuCheck_OneSolution", "Sudoku is unique"},
			{"Message_SudokuCheck_MoreSolutions", "Sudoku is not unique" },
			{"Message_SudokuCheck_Invalid", "Sudoku is invalid"},
			{"Message_SudokuCheck_NoSolution", "Sudoku doesn't have solution" },
			{"Button_Stop", "Stop"},
			{"Alert_Cancelled_Input", "Generation cancelled"},
			{"Button_Solve", "Solve"},
			{"Label_PrimaryColor", "Primary color" },
			{"Label_SecondaryColor", "Interactive color" },
			{"Label_BackgroundColor", "Background color" },
			{"Label_DefaultPathLocation", "Default save folder location"},
			{"Message_LoadFile_Failed", "Couldn't load data from selected file"},
			{"Button_Clear", "Clear"}
		};
		public static readonly Dictionary<string, string> LanguageNames = new Dictionary<string, string>
		{
			{"cs", "Čeština"},
			{"en", "English"}
		};
		public static readonly Dictionary<string, Dictionary<string, string>> LanguageKorpuses = new Dictionary<string, Dictionary<string, string>>
		{
			{"cs", czechLanguage },
			{"en", englishLanguage }
		};
		public static void SetLanguage()
		{
			ChangeLanguage?.Invoke("langSender", SelectedLanguage);
		}
		public static string DefaultLanguage = "en";
		private static string? _selectedLanguage;
		public static string SelectedLanguage
		{
			get => _selectedLanguage;
			set
			{
				if(value == null)
				{
					_selectedLanguage = DefaultLanguage;
				}
				else if (LanguageNames.ContainsKey(value))
				{
					_selectedLanguage = value;
				}
				else
				{
					_selectedLanguage = DefaultLanguage;
				}
			}
		}
		public static void LoadLanguage()
		{
			languageHandler.SelectedLanguage = settingsHandler.LoadSetting("language");
		}
	}
}
