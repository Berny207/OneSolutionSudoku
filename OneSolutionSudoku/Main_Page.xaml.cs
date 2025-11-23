using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
using WinForms = System.Windows.Forms;

namespace OneSolutionSudoku
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Main_Page : Page, Ipage
    {
        public Main_Page()
        {
            InitializeComponent();
			languageHandler.ChangeLanguage += OnChangeLanguage;
			languageHandler.SetLanguage();
		}
		private void Button_End_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}
		private void Button_OwnCreation_Click(object sender, RoutedEventArgs e)
		{
			Sudoku testDoku = new Sudoku();
			List<Coordinates> toUpdate = testDoku.UpdateSpace(new Coordinates(0, 0), 5);
			foreach (Coordinates coordinate in toUpdate)
			{
				Console.WriteLine(coordinate.row + " " + coordinate.column);
			}
			Console.WriteLine(toUpdate.Count);
		}
		private void Button_Generate_Click(object sender, RoutedEventArgs e)
		{
			Sudoku generatedBaseplate = BaseplateGenerator.GenerateBaseplate();
			System.Windows.MessageBox.Show(generatedBaseplate.ToString());
		}
		private void Button_Options_Click(object sender, RoutedEventArgs e)
		{
			bool success = NavigationService.Navigate(new Options_Page());
		}
		public void OnChangeLanguage(object sender, string selectedLanguague)
		{
			Dictionary<string, string> languageKorpus = OneSolutionSudoku.languageHandler.languages[selectedLanguague];
			foreach (string key in languageKorpus.Keys)
			{
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
