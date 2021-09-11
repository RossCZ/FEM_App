using FEM_App.Common;
using System;
using System.Windows;

namespace FEM_App.FEM_Column
{
    /// <summary>
    /// Interaction logic for FEM_Column.xaml
    /// </summary>
    public partial class FEM_Column_Main : Window
    {
        private ColumnPresenter Presenter { get; set; }

        private Column ActualColumn { get; set; }

		private bool InitializationFinished { get; set; }

        public FEM_Column_Main()
        {
            InitializeComponent();
			WindowState = WindowState.Maximized;
			//WindowStyle = WindowStyle.None; // this hides upper controls
            ActualColumn = new Column();
            Presenter = new ColumnPresenter(MainScene, ActualColumn);

			InitializationFinished = true;
			RefreshUI_calcSettings(null, null);
		}

		private void RefreshUI_calcSettings(object sender, RoutedEventArgs e)
		{
			if (InitializationFinished)
			{
				CalculateButtonEnabled(true);
				TB_stressOnArea.Text = string.Format("{0:F2} MPa", (((double)NB_verticalForceSize.Value * 1e3) / ((double)NB_areaA.Value / 1e4)) / 1e6);
				UpdateColumn();

				if ((bool)CB_autoCalculate.IsChecked && sender != null) // disable endless loop for error redraw
				{
					CalculateEvent(sender, e);
				}
				else
				{
					RedrawColumn();
				}
			}
		}

		private void RefreshUI(object sender, RoutedEventArgs e)
        {
			if (InitializationFinished)
			{
				RedrawColumn();
			}
        }

		private void CalculateButtonEnabled(bool enabled)
		{
			if (BTN_calculate != null)
			{
				BTN_calculate.IsEnabled = enabled;
				TB_calculated.Text = enabled ? "Not calculated..." : "Calculated!";
			}
		}

		private void CalculateEvent(object sender, RoutedEventArgs e)
		{
			try
			{
				CalculateColumn();
			}
			catch (Exception ex)
			{
				var win = new ErrWindow(ex);
				win.Show();
				RefreshUI_calcSettings(null, null);
				return;
			}
			RefreshUI(null, null);
		}

		private void CalculateColumn()
		{
			ActualColumn.ClearResults();
			var solver = new ColumnSolver(ActualColumn, GetCalculationSetup());
			solver.Calculate();
			CalculateButtonEnabled(false);
		}

		private void UpdateColumn()
        {
            if (ActualColumn != null)
            {
                var calcSetup = GetCalculationSetup();

                ActualColumn.Height = (double)NB_height.Value;
				ActualColumn.E = (double)NB_modulusE.Value * 1e9;
				ActualColumn.A = (double)NB_areaA.Value / 1e4;
				ActualColumn.Iy = (double)NB_momentI.Value / 1e8;

				ActualColumn.Forces.Clear();
				ActualColumn.Supports.Clear();

				// no position -> general way would require position and mapping to fem elements when meshing
				var verticalForceValue = (bool)CB_verticalForce.IsChecked ? (double)NB_verticalForceSize.Value * 1e3 : 0;
				ActualColumn.Forces.Add(new Force(0, verticalForceValue, 0));
				var horizontalForceValue = (bool)CB_horizontalForce.IsChecked ? (double)NB_horizontalForceSize.Value * 1e3 : 0;
				ActualColumn.Forces.Add(new Force(horizontalForceValue, 0, 0));

				var begSupport = new Support((bool)CB_begSupportX.IsChecked, (bool)CB_begSupportY.IsChecked, (bool)CB_begSupportRz.IsChecked);
				var endSupport = new Support((bool)CB_endSupportX.IsChecked, (bool)CB_endSupportY.IsChecked, (bool)CB_endSupportRz.IsChecked);
				ActualColumn.Supports.Add(begSupport);
				ActualColumn.Supports.Add(endSupport);

				ColumnMesher.MeshColumn(ActualColumn, calcSetup);
            }
        }

        private void RedrawColumn()
        {
            if (Presenter != null)
            {
                var drSetup = GetDrawingSetup();
                Presenter.RedrawColumn(drSetup);
            }
        }

        private DrawingSetupColumn GetDrawingSetup()
        {
            var drSetup = new DrawingSetupColumn();

            drSetup.ZoomFactor = (int)NB_zoom.Value;
			drSetup.NodeLabels = (bool)CB_nodeLabels.IsChecked;
			drSetup.ElementLabels = (bool)CB_elementLabels.IsChecked;
			drSetup.NodeResults_Force = (bool)CB_results_force.IsChecked;
			drSetup.NodeResults_Deformation = (bool)CB_results_deformation.IsChecked;
			drSetup.DeformationScale = (double)NB_deformationScale.Value * 100;
			drSetup.OriginalStructure = (bool)CB_origStructure.IsChecked;
			drSetup.DrawLCS = (bool)CB_drawLCS.IsChecked;

			return drSetup;
        }

		private CalculationSetupColumn GetCalculationSetup()
		{
			var calcSetup = new CalculationSetupColumn();
			calcSetup.NumberOfElements = (int)NB_elements.Value;
			calcSetup.LoadIncrements = (int)NB_increments.Value;
			calcSetup.InitialMiddleDeflection = (double)NB_initDef.Value / 1e3;
			return calcSetup;
		}
	}
}
