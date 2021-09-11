using System.Windows;

namespace FEM_App.FEM_Wall
{
	/// <summary>
	/// Interaction logic for EditationWindow.xaml
	/// </summary>
	public partial class EditationWindow : Window
	{
		private WallLayer layer;
		private EditationStatus status;


		public EditationWindow(WallLayer layer, EditationStatus status)
		{
			InitializeComponent();

			this.layer = layer;
			this.status = status;
			this.status.Status = false;

			TB_Name.Text = layer.Name;
			NB_width.Value = (decimal)layer.Width;
			NB_lambda.Value = (decimal)layer.Lambda;
		}

		private void BTN_OK_Click(object sender, RoutedEventArgs e)
		{
			layer.UpdateLayer(TB_Name.Text, (double)NB_width.Value, (double)NB_lambda.Value.Value);
			status.Status = true;

			this.Close();
		}
	}
}
