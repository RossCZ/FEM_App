using System;
using System.Windows;

namespace FEM_App.Common
{
	/// <summary>
	/// Interaction logic for ErrWindow.xaml
	/// </summary>
	public partial class ErrWindow : Window
    {
        public ErrWindow(Exception e)
        {
            InitializeComponent();

			TB_errMessage.Text = e.Message;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
