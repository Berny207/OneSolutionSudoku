using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace OneSolutionSudoku
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void changeColors()
        {
            Application.Current.Resources["PrimaryColor"] = colorHandler.PrimaryColor;
			Application.Current.Resources["SecondaryColor"] = colorHandler.SecondaryColor;
			Application.Current.Resources["BackgroundColor"] = colorHandler.BackgroundColor;
		}
        public static App Instance = (App)Application.Current;
	}

}
