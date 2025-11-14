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
    public partial class Main_Page : Page
    {
        public Main_Page()
        {
            InitializeComponent();
        }
		private void Button_End_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}
		private void Button_OwnCreation_Click(object sender, RoutedEventArgs e)
		{

		}
		private void Button_Generate_Click(object sender, RoutedEventArgs e)
		{

		}
		private void Button_Options_Click(object sender, RoutedEventArgs e)
		{
			bool success = NavigationService.Navigate(new Options_Page());
		}
	}
}
