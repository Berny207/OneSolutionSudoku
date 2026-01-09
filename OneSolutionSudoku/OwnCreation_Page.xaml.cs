using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Binding = System.Windows.Data.Binding;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace OneSolutionSudoku
{
	/// <summary>
	/// Interaction logic for OwnCreation_Page.xaml
	/// </summary>
	public partial class OwnCreation_Page : Page, IPage
	{
		int BorderThickness = 5;

		static TextBox[,] textBoxes = new TextBox[9, 9];
		static string IncorrectInput="";
		static string CheckMessageOneSolution = "";
		static string CheckMessageMoreSolution = "";
		static string CheckMessageInvalid = "";
		static string CheckMessageNoSolution = "";
		static string LoadFileFailed = "";
		public OwnCreation_Page()
		{
			InitializeComponent();
			DataContext = this;
			languageHandler.ChangeLanguage += OnChangeLanguage;
			languageHandler.SetLanguage();
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Border border = new Border
					{
						BorderBrush = System.Windows.Media.Brushes.Black,
						BorderThickness = new Thickness((column % 3 == 0 ? 1 : 0) * BorderThickness, (row % 3 == 0 ? 1 : 0) * BorderThickness, ((column + 1) % 3 == 0 ? 1 : 0) * BorderThickness, ((row + 1) % 3 == 0 ? 1 : 0) * BorderThickness),
						//BorderThickness = new Thickness((column % 3 == 0 ? 1 : 0) * BorderThickness, 0, 0, 0),
						Margin = new Thickness(0),
						Width=50,
						Height=50
						
					};

					TextBox textbox = new TextBox
					{
						Margin = new Thickness(0),
						TextAlignment = TextAlignment.Center,
						VerticalContentAlignment = VerticalAlignment.Center,
						Style = (Style)System.Windows.Application.Current.FindResource("SudokuCellTextBox"),
					};

					textbox.PreviewTextInput += Preview_Text_Input;
					border.Child = textbox;
					textBoxes[row, column] = textbox;
					Grid.SetRow(border, row);
					Grid.SetColumn(border, column);
					SudokuGrid.Children.Add(border);
				}
			}
		}
		/// <summary>
		/// only updates front-end sudoku. Doesn't call SudokuSavingHandler
		/// </summary>
		/// <param name="sudoku"></param>
		/// <returns></returns>
		private static bool LoadSudoku(Sudoku sudoku)
		{
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Coordinates coordinates = new Coordinates(row, column);
					int cellValue = sudoku.GetCell(coordinates).Value;
					List<int> allowedValues = new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
					// Fix: Use Array.IndexOf instead of Contains for int[]
					if (allowedValues.Contains(cellValue)==true)
					{
						if (cellValue == 0)
						{
							textBoxes[row, column].Text = null;
						}
						else
						{
							textBoxes[row, column].Text = cellValue.ToString();
						}
						continue;
					}
					MessageBox.Show(IncorrectInput);
					return false;
				}
			}
			return true;
		}
		/// <summary>
		/// only updates back-end sudoku value. Doesn't call SudokuSavingHandler
		/// </summary>
		/// <param name="sudoku"></param>
		/// <returns></returns>
		private static Sudoku SaveSudoku()
		{
			Sudoku sudoku = new Sudoku();
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					string text = textBoxes[row, column].Text;
					if (string.IsNullOrEmpty(text))
					{
						text = "0";
					}
					try
					{
						sudoku.SetCell(new Coordinates(row, column), int.Parse(text));
					}
					catch 
					{
						MessageBox.Show(IncorrectInput);
						return null;
					}
				}
			}
			return sudoku;
		}
		private void ButtonCheckClick(object sender, RoutedEventArgs e)
		{
			userSudoku = SaveSudoku();
			if (userSudoku == null)
			{
				return;
			}
			// Is it a valid Sudoku?
			bool isValid = userSudoku.IsValid();
			if (!isValid)
			{
				MessageBox.Show(CheckMessageInvalid);
				return;
			}

			// Does it have A solution?
			Sudoku solvedSudoku = userSudoku.Clone();
			bool isSolvable = MainAlgoritmhs.SolveSudoku(solvedSudoku);
			if (!isSolvable)
			{
				MessageBox.Show(CheckMessageNoSolution);
				return;
			}
			// Is that solution unique
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Coordinates coordinates = new Coordinates(row, column);
					int solvedValue = solvedSudoku.GetCell(coordinates).Value;
					if (userSudoku.GetCell(coordinates).Value != 0)
					{
						continue;
					}
					List<int> possibleValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
					possibleValues.Remove(solvedValue);
					foreach (int possibleValue in possibleValues)
					{
						Sudoku cloneDoku = userSudoku.Clone();
						cloneDoku.SetCell(coordinates, possibleValue);
						if (!cloneDoku.IsValid())
						{
							continue;
						}
						if (MainAlgoritmhs.SolveSudoku(cloneDoku))
						{
							MessageBox.Show(CheckMessageMoreSolution);
							return;
						}
					}
				}
			}
			MessageBox.Show(CheckMessageOneSolution);
		}

		private void ButtonSolveClick(object sender, RoutedEventArgs e)
		{
			userSudoku = SaveSudoku();
			if(userSudoku == null)
			{
				return;
			}
			// Is it a valid Sudoku?
			bool isValid = userSudoku.IsValid();
			if (!isValid)
			{
				MessageBox.Show(CheckMessageInvalid);
				return;
			}

			MainAlgoritmhs.SolveSudoku(userSudoku);
			LoadSudoku(userSudoku);
		}
		public string sudokuFileNameInput { get; set; }
		internal Sudoku userSudoku { get; set; } = new Sudoku();
		private void ButtonSaveClick(object sender, RoutedEventArgs e)
		{
			userSudoku = SaveSudoku();
			if (userSudoku == null)
			{
				return;
			}
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.InitialDirectory = SudokuSavingHandler.saveLocation;
			dialog.Filter = "Text files (*.txt)|*.txt";
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				string selectedPath = dialog.FileName;
				SudokuSavingHandler.SaveSudoku(userSudoku, selectedPath);
			}
		}

		private void ButtonLoadClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.InitialDirectory = SudokuSavingHandler.saveLocation;
			dialog.Filter = "Text files (*.txt)|*.txt";
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				string selectedPath = dialog.FileName;
				userSudoku = SudokuSavingHandler.LoadSudoku(selectedPath);
				if (userSudoku != null) {
					LoadSudoku(userSudoku);
				}
				else
				{
					MessageBox.Show(LoadFileFailed);
				}
			}
		}
		private void ButtonBackClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Main_Page());
		}
		private static readonly Regex number_regex = new Regex("[^1-9]");
		private static bool IsTextNumerical(string text)
		{
			return number_regex.IsMatch(text);
		}
		private void Preview_Text_Input(object sender, TextCompositionEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if(tb.Text.Length >= 1)
			{
				e.Handled = true;
				return;
			}
			e.Handled = IsTextNumerical(e.Text);
		}

		private void ButtonClearClick(object sender, RoutedEventArgs e)
		{
			userSudoku = new Sudoku();
			LoadSudoku(userSudoku);
		}
		public void OnChangeLanguage(object sender, string selectedLanguague)
		{
			Dictionary<string, string> languageKorpus = OneSolutionSudoku.languageHandler.LanguageKorpuses[selectedLanguague];
			foreach (string key in languageKorpus.Keys)
			{
				if(key == "Message_SudokuCheck_OneSolution")
				{
					CheckMessageOneSolution = languageKorpus[key];
				}
				else if(key == "Message_SudokuCheck_MoreSolutions")
				{
					CheckMessageMoreSolution = languageKorpus[key];
				}
				else if(key == "Message_SudokuCheck_Invalid")
				{
					CheckMessageInvalid = languageKorpus[key];
				}
				else if(key == "Message_SudokuCheck_NoSolution")
				{
					CheckMessageNoSolution = languageKorpus[key];
				}
				else if (key == "Message_LoadFile_Failed")
				{
					LoadFileFailed = languageKorpus[key];
				}
				else
				if (key == "Alert_Incorrect_Input")
				{
					IncorrectInput = languageKorpus[key];
				}
				var findMeResult = this.FindName(key);
				if (findMeResult is TextBlock textBlock)
				{
					textBlock.Text = languageKorpus[key];
				}
				else if (findMeResult is System.Windows.Controls.Button button)
				{
					button.Content = languageKorpus[key];
				}
			}
		}
	}
}
