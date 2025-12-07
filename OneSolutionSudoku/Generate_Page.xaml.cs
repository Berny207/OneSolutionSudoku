using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Button = System.Windows.Controls.Button;

namespace OneSolutionSudoku
{
	/// <summary>
	/// Interaction logic for Generate_Page.xaml
	/// </summary>
	public partial class Generate_Page : Page, Ipage
	{
		public Generate_Page()
		{
			InitializeComponent();
			DataContext = this;
			languageHandler.ChangeLanguage += OnChangeLanguage;
			languageHandler.SetLanguage();
		}
		private void cancelGeneration()
		{
			if (_cts != null)
			{
				_cts.Cancel();
			}
		}
		private void Button_Back_Click(object sender, RoutedEventArgs e)
		{
			cancelGeneration();
			NavigationService.Navigate(new Main_Page());
		}
		string InvalidInput;
		string IncorrectInput;
		string cancelledInput;
		private CancellationTokenSource _cts;

		private async void Button_Generate_Click(object sender, RoutedEventArgs e)
		{
			int fullCellAmount = 0;
			try
			{
				fullCellAmount = int.Parse(fullCellAmountInput);
			}
			catch
			{
				System.Windows.MessageBox.Show(IncorrectInput);
				return;
			}

			if (fullCellAmount < 17 || fullCellAmount > 81)
			{
				System.Windows.MessageBox.Show(InvalidInput);
				return;
			}
			_cts = new CancellationTokenSource();
			Sudoku generatedSudoku = new Sudoku();
			try
			{
				generatedSudoku = await Task.Run(() => SudokuPuncturer.GenerateSudoku(81 - fullCellAmount, _cts.Token));
			}
			catch
			{
				System.Windows.MessageBox.Show(cancelledInput);
				return;
			}
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.InitialDirectory = SudokuSavingHandler.saveLocation;
			dialog.Filter = "Text files (*.txt)|*.txt";
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				string selectedPath = dialog.FileName;
				SudokuSavingHandler.SaveSudoku(generatedSudoku, selectedPath);
			}
		}
		private void Button_Stop_Click(object sender, RoutedEventArgs e)
		{
			cancelGeneration();
		}
		private static readonly Regex number_regex = new Regex("[^0-9]");
		private static bool IsTextNumerical(string text)
		{
			return number_regex.IsMatch(text);
		}
		private void Preview_Text_Input(object sender, TextCompositionEventArgs e)
		{
			e.Handled = IsTextNumerical(e.Text);
		}
		public string fullCellAmountInput { get; set; }
		public void OnChangeLanguage(object sender, string selectedLanguague)
		{
			Dictionary<string, string> languageKorpus = OneSolutionSudoku.languageHandler.languages[selectedLanguague];
			foreach (string key in languageKorpus.Keys)
			{
				if(key == "Alert_InvalidGeneration_Input")
				{
					InvalidInput = languageKorpus[key];
				}
				if(key == "Alert_Incorrect_Input")
				{
					IncorrectInput = languageKorpus[key];
				}
				if(key == "Alert_Cancelled_Input")
				{
					cancelledInput = languageKorpus[key];
				}
				var findMeResult = this.FindName(key);
				if (findMeResult is TextBlock textBlock)
				{
					textBlock.Text = languageKorpus[key];
				}
				else if (findMeResult is Button button)
				{
					button.Content = languageKorpus[key];
				}
			}
		}
	}
}
