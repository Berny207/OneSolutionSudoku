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
	public partial class OwnCreation_Page : Page, Ipage
	{
		int BorderThickness = 5;

		static TextBox[,] textBoxes = new TextBox[9, 9];
		static string IncorrectInput="";
		static string CheckMessageOneSolution = "";
		static string CheckMessageMoreSolution = "";
		static string CheckMessageInvalid = "";
		static string CheckMessageNoSolution = "";
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
					Border b = new Border
					{
						BorderBrush = System.Windows.Media.Brushes.Black,
						BorderThickness = new Thickness((column % 3 == 0 ? 1 : 0) * BorderThickness, (row % 3 == 0 ? 1 : 0) * BorderThickness, ((column + 1) % 3 == 0 ? 1 : 0) * BorderThickness, ((row + 1) % 3 == 0 ? 1 : 0) * BorderThickness),
						//BorderThickness = new Thickness((column % 3 == 0 ? 1 : 0) * BorderThickness, 0, 0, 0),
						Margin = new Thickness(0)
					};

					TextBox tb = new TextBox
					{
						Margin = new Thickness(0),
						TextAlignment = TextAlignment.Center,
						VerticalContentAlignment = VerticalAlignment.Center,
						Style = (Style)System.Windows.Application.Current.FindResource("SudokuCellTextBox"),
					};

					tb.PreviewTextInput += Preview_Text_Input;
					b.Child = tb;
					textBoxes[row, column] = tb;
					Grid.SetRow(b, row);
					Grid.SetColumn(b, column);
					SudokuGrid.Children.Add(b);
				}
			}
		}
		private static void LoadSudoku(Sudoku sudoku)
		{
			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					Coordinates coordinates = new Coordinates(row, column);
					int cellValue = sudoku.GetCell(coordinates).value;
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
				}
			}
		}
		private static Sudoku SaveSudoku(Sudoku sudoku)
		{
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
					}
				}
			}
			return sudoku;
		}
		private void Button_Check_Click(object sender, RoutedEventArgs e)
		{
			userSudoku = SaveSudoku(userSudoku);
			// Is it a valid Sudoku?
			bool isValid = userSudoku.IsValid();
			if (!isValid)
			{
				MessageBox.Show(CheckMessageInvalid);
				return;
			}

			// Does it have A solution?
			Sudoku solvedSudoku = userSudoku.Clone();
			bool isSolvable = SudokuPuncturer.SolveSudoku(solvedSudoku);
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
					int solvedValue = solvedSudoku.GetCell(coordinates).value;
					if (userSudoku.GetCell(coordinates).value != 0)
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
						if (SudokuPuncturer.SolveSudoku(cloneDoku))
						{
							MessageBox.Show(CheckMessageMoreSolution);
							return;
						}
					}
				}
			}
			MessageBox.Show(CheckMessageOneSolution);
		}
		public string sudokuFileNameInput { get; set; }
		internal Sudoku userSudoku { get; set; } = new Sudoku();
		private void Button_Save_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.InitialDirectory = SudokuSavingHandler.saveLocation;
			dialog.Filter = "Text files (*.txt)|*.txt";
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				string selectedPath = dialog.FileName;
				userSudoku = SaveSudoku(userSudoku);
				SudokuSavingHandler.SaveSudoku(userSudoku, selectedPath);
			}
		}

		private void Button_Load_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.InitialDirectory = SudokuSavingHandler.saveLocation;
			dialog.Filter = "Text files (*.txt)|*.txt";
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				string selectedPath = dialog.FileName;
				userSudoku = SudokuSavingHandler.LoadSudoku(selectedPath);
				LoadSudoku(userSudoku);
			}
		}
		private void Button_Back_Click(object sender, RoutedEventArgs e)
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

		public void OnChangeLanguage(object sender, string selectedLanguague)
		{
			Dictionary<string, string> languageKorpus = OneSolutionSudoku.languageHandler.languages[selectedLanguague];
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
