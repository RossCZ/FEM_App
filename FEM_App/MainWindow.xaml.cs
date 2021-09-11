using FEM_App.FEM_Column;
using FEM_App.FEM_Fire;
using FEM_App.FEM_Rope;
using FEM_App.FEM_Wall;
using System.Windows;

namespace FEM_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FEM_Column_Click(object sender, RoutedEventArgs e)
        {
            var win = new FEM_Column_Main();
            win.Show();
            //this.Close();
        }

        private void FEM_Rope_Click(object sender, RoutedEventArgs e)
        {
            var win = new FEM_Rope_Main();
            win.Show();
            //this.Close();
        }

		private void FEM_Fire_Click(object sender, RoutedEventArgs e)
		{
			var win = new FEM_Fire_Main();
			win.Show();
			//this.Close();
		}

		private void FEM_Wall_Click(object sender, RoutedEventArgs e)
		{
			var win = new FEM_Wall_Main();
			win.Show();
			//this.Close();
		}
	}
}
