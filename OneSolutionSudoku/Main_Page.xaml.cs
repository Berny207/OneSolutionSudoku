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
using Label = System.Windows.Controls.Label;
using WinForms = System.Windows.Forms;

namespace OneSolutionSudoku
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Main_Page : Page, IPage
    {
		MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
		static Random random = new Random();
		public Main_Page()
        {
            InitializeComponent();
			languageHandler.ChangeLanguage += OnChangeLanguage;
			languageHandler.SetLanguage();
		}
		private void ButtonEndClick(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}
		private void ButtonOwnCreationClick(object sender, RoutedEventArgs e)
		{
			mainWindow.MainFrame.Navigate(new OwnCreation_Page());
		}
		private void ButtonGenerateClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Generate_Page());
		}
		private void ButtonOptionsClick(object sender, RoutedEventArgs e)
		{
			bool success = NavigationService.Navigate(new Options_Page());
		}
		public void OnChangeLanguage(object sender, string selectedLanguague)
		{
			Dictionary<string, string> languageKorpus = OneSolutionSudoku.languageHandler.LanguageKorpuses[selectedLanguague];
			foreach (string key in languageKorpus.Keys)
			{
				var findMeResult = this.FindName(key);
				if (findMeResult is Label label)
				{
					label.Content = languageKorpus[key];
				}
				else if (findMeResult is Button button)
				{ 
					button.Content = languageKorpus[key];
				}
			}
		}
	}
}
