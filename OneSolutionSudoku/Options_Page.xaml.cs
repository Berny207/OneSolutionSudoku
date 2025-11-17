using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;

namespace OneSolutionSudoku
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Options_Page : Page, Ipage, INotifyPropertyChanged
    {
        public Options_Page()
        {
            InitializeComponent();
            DataContext = this;
			OneSolutionSudoku.languageHandler.ChangeLanguage += OnChangeLanguage;
			languageHandler.SetLanguage();
		}
		public List<string> Languages { get; set; } = new List<string>();

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		/// <summary>
		/// Processes the Back button click event and navigates to the Main Page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Main_Page());
        }
		/// <summary>
		/// Processes the Confirm button click event, sets new settings, saves the new settings, and applies them.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Sets default settings and calls confirm button click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Defaults_Click(object sender, RoutedEventArgs e)
		{
			boundPrimaryColor = colorHandler.defaultPrimaryColor.Color;
			OnPropertyChanged(nameof(boundPrimaryColor));
			boundSecondaryColor = colorHandler.defaultSecondaryColor.Color;
			OnPropertyChanged(nameof(boundSecondaryColor));
			boundBackgroundColor = colorHandler.defaultBackgroundColor.Color;
			OnPropertyChanged(nameof(boundBackgroundColor));
			boundLanguage = languageHandler.languageNames[languageHandler.defaultLanguage];
			OnPropertyChanged(nameof(boundLanguage));
			SudokuSavingHandler.saveLocation = SudokuSavingHandler.defaultSaveLocation;
			Button_Confirm_Click(sender, e);
		}
		/// <summary>
		/// Processes the Folder Select button click event and opens a folder browser dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_FolderSelect_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.InitialDirectory = SudokuSavingHandler.saveLocation;
			DialogResult result = dialog.ShowDialog();
			if(result == DialogResult.OK)
			{
				string selectedPath = dialog.SelectedPath;
				SudokuSavingHandler.saveLocation = selectedPath;
				settingsHandler.SaveSettings();
			}
		}
		public Color boundPrimaryColor { get; set; } = colorHandler.PrimaryColor.Color;
		public Color boundSecondaryColor { get; set; } = colorHandler.SecondaryColor.Color;
		public Color boundBackgroundColor { get; set; } = colorHandler.BackgroundColor.Color;
		public string boundLanguage { get; set; } = languageHandler.languageNames[languageHandler.selectedLanguage];
		/// <summary>
		/// Processes language change events and updates UI elements accordingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="selectedLanguague"></param>
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
