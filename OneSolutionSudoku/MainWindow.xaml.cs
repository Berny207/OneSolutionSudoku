using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Frame MainFramereference => MainFrame;

        public MainWindow()
        {
            InitializeComponent();
            languageHandler.LoadLanguage();
            colorHandler.LoadColors(settingsHandler.LoadSetting("Primary color"), settingsHandler.LoadSetting("Secondary color"), settingsHandler.LoadSetting("Background color"));
			App.Instance.ChangeColors();
            SudokuSavingHandler.saveLocation = settingsHandler.LoadSetting("Save location");
			MainFrame.Navigate(new Main_Page());
		}
	}
}