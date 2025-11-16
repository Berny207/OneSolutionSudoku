using System;
using System.Collections.Generic;
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

namespace OneSolutionSudoku
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Options_Page : Page, Ipage
    {
        public Options_Page()
        {
            InitializeComponent();
            DataContext = this;
			OneSolutionSudoku.languageHandler.ChangeLanguage += OnChangeLanguage;
			languageHandler.SetLanguage();
		}
		public List<string> Languages { get; set; } = new List<string>();
		private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Main_Page());
        }
		private void Button_Confirm_Click(object sender, RoutedEventArgs e)
		{
			colorHandler.PrimaryColor = new SolidColorBrush(boundPrimaryColor);
			colorHandler.SecondaryColor = new SolidColorBrush(boundSecondaryColor);
			colorHandler.BackgroundColor = new SolidColorBrush(boundBackgroundColor);
			if (boundLanguage == null) return;
			languageHandler.selectedLanguage = languageHandler.languageNames.FirstOrDefault(x => x.Value == boundLanguage).Key;
			languageHandler.SetLanguage();
			settingsHandler.SaveSettings();
			App.Instance.changeColors();
		}
		public Color boundPrimaryColor { get; set; } = colorHandler.PrimaryColor.Color;
		public Color boundSecondaryColor { get; set; } = colorHandler.SecondaryColor.Color;
		public Color boundBackgroundColor { get; set; } = colorHandler.BackgroundColor.Color;
		public string boundLanguage { get; set; } = languageHandler.languageNames[languageHandler.selectedLanguage];
        
		public void OnChangeLanguage(object sender, string selectedLanguague)
        {
			Languages = new List<string>();
			List<string> languageNames = OneSolutionSudoku.languageHandler.languageNames.Values.ToList();
			foreach(string languagueName in languageNames)
			{
				Languages.Add(languagueName);
			}
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
