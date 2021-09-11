using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace FEM_App.FEM_Wall
{
	/// <summary>
	/// Interaction logic for FEM_Wall_Main.xaml
	/// </summary>
	public partial class FEM_Wall_Main : Window
	{
		private Wall ActualWall { get; set; }

		private WallPresenter Presenter { get; set; }

		private bool InitializationFinished { get; set; }

		public FEM_Wall_Main()
		{
			InitializeComponent();
			WindowState = WindowState.Maximized;

			ActualWall = new Wall();
			DG_layers.ItemsSource = ActualWall.Layers;
			GetDefaultWallData();
			Presenter = new WallPresenter(MainScene, ActualWall);

			InitializationFinished = true;
			RefreshUI_calcSettings(null, null);
		}

		private CalculationSetupWall GetCalculationSetup()
		{
			var setup = new CalculationSetupWall();
			setup.ElementSize = (double)NB_FEMElemSize.Value;
			setup.InitialTemperature = (double)NB_tempIn.Value;
			setup.FirstNodeTemperature = (double)NB_tempOut.Value;
			setup.H_InTheLastNode = (double)NB_HWallSurface.Value;
			return setup;
		}

		private DrawingSetupWall GetDrawingSetup()
		{
			var setup = new DrawingSetupWall();
			setup.ZoomFactor = (int)NB_zoom.Value;
			setup.DetailedTextResults = (bool)CB_detailedTextResults.IsChecked;
			return setup;
		}

		private void GetDefaultWallData()
		{
			string name_Conc = "Concrete";
			string name_Ins = "Polystyrene";

			ActualWall.Layers.Add(new WallLayer(name_Conc, 0.07, 1.58));
			ActualWall.Layers.Add(new WallLayer(name_Ins, 0.08, 0.04));
			ActualWall.Layers.Add(new WallLayer(name_Conc, 0.15, 1.58));
		}

		private void RedrawWall()
		{
			if (Presenter != null)
			{
				var drSetup = GetDrawingSetup();

				Presenter.PresentTextInfo(ActualWall, GetCalculationSetup(), TB_TextResults, drSetup.DetailedTextResults);
				Presenter.RedrawWall(drSetup);
			}
		}

		private void BTN_Calculate_Click(object sender, RoutedEventArgs e)
		{
			if (ActualWall.Layers.Any())
			{
				var setup = GetCalculationSetup();
				WallMesher.MeshWall(ActualWall, setup);
				var solver = new WallSolver(ActualWall, setup);
				solver.Calculate();

				RedrawWall();
			}
		}

		private void BTN_Delete_Click(object sender, RoutedEventArgs e)
		{
			var selectedItemInx = DG_layers.SelectedIndex;

			if (SelectedIndexExists(selectedItemInx))
			{
				ActualWall.Layers.RemoveAt(selectedItemInx);
				UpdateDataGridSource();
				RefreshUI_calcSettings(null, null);
			}
		}

		private void BTN_Add_Click(object sender, RoutedEventArgs e)
		{
			var newLayer = ActualWall.Layers.Any() ? new WallLayer(ActualWall.Layers.LastOrDefault()) : new WallLayer();
			var status = new EditationStatus();
			var editWindow = new EditationWindow(newLayer, status);
			editWindow.ShowDialog();

			if (status.Status == true)
			{
				ActualWall.Layers.Add(newLayer);
				UpdateDataGridSource();
				RefreshUI_calcSettings(null, null);
			}
		}

		private void BTN_Edit_Click(object sender, RoutedEventArgs e)
		{
			var selectedItemInx = DG_layers.SelectedIndex;

			if (SelectedIndexExists(selectedItemInx))
			{
				var status = new EditationStatus();
				var editWindow = new EditationWindow(ActualWall.Layers[selectedItemInx], status);
				editWindow.ShowDialog();

				if (status.Status == true)
				{
					UpdateDataGridSource();
					RefreshUI_calcSettings(null, null);
				}
			}
		}

		private void UpdateDataGridSource()
		{
			// or use ObservableCollection to notify changes automatically
			DG_layers.ItemsSource = null;
			DG_layers.ItemsSource = ActualWall.Layers;
		}

		private bool SelectedIndexExists(int selectedIndex)
		{
			return (selectedIndex > -1 && selectedIndex < ActualWall.Layers.Count);
		}

		private void DG_layers_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
		{
			e.Row.Header = (e.Row.GetIndex() + 1).ToString();
		}

		private void RefreshUI_calcSettings(object sender, RoutedEventArgs e)
		{
			if (InitializationFinished)
			{
				UpdateWall();
				RedrawWall();
			}
		}

		private void RefreshUI(object sender, RoutedEventArgs e)
		{
			if (InitializationFinished)
			{
				RedrawWall();
			}
		}

		private void UpdateWall()
		{
			if (ActualWall != null)
			{
				foreach (var layer in ActualWall.Layers)
				{
					layer.FEMElements.Clear();
					layer.FEMNodes.Clear();
				}
			}
		}
	}
}
