using FEM_App.Common;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace FEM_App.FEM_Fire
{
	/// <summary>
	/// Interaction logic for FEM_Fire_Main.xaml
	/// </summary>
	public partial class FEM_Fire_Main : Window
	{
		private FireCSSPresenter Presenter { get; set; }

		private FireCSS ActualFireCSS { get; set; }

		private CancellationTokenSource Token { get; set; }

		private bool InitializationFinished { get; set; }

		public delegate void RedrawSceneCallback();

		public delegate void CalculationFinishedCallback(bool cancelled);

		public delegate void CalculationStartedCallback();

		public FEM_Fire_Main()
		{
			InitializeComponent();
			WindowState = WindowState.Maximized;
			ActualFireCSS = new FireCSS();
			Presenter = new FireCSSPresenter(MainScene, ActualFireCSS);
			Token = new CancellationTokenSource();

			InitializationFinished = true;
			RefreshUI_calcSettings(null, null);
		}

		private void RefreshUI(object sender, RoutedEventArgs e)
		{
			if (InitializationFinished)
			{
				RedrawFireCSS();
			}
		}

		private void RefreshUI_calcSettings(object sender, RoutedEventArgs e)
		{
			if (InitializationFinished)
			{
				CalculateButtonEnabled(true);
				UpdateFireCSS();
				RedrawFireCSS();
			}
		}

		private void UpdateFireCSS()
		{
			if (ActualFireCSS != null)
			{
				var calcSetup = GetCalculationSetup();

				TimeSpan result = TimeSpan.FromSeconds(calcSetup.FinalTime);
				TB_finalTimeFormatted.Text = string.Format(result.ToString("hh':'mm':'ss"));

				ActualFireCSS.Width = (double)NB_cssWidth.Value * 1e-3;
				ActualFireCSS.Height = (double)NB_cssHeight.Value * 1e-3;
				ActualFireCSS.Lambda = (double)NB_thermalConductivity.Value;

				ActualFireCSS.EnvironmentLeft = new EnvironmentOfCSS((double)NB_tempLeft.Value);
				ActualFireCSS.EnvironmentTop = new EnvironmentOfCSS((double)NB_tempTop.Value);
				ActualFireCSS.EnvironmentRight = new EnvironmentOfCSS((double)NB_tempRight.Value);
				ActualFireCSS.EnvironmentBottom = new EnvironmentOfCSS((double)NB_tempBottom.Value);

				FireCSSMesher.MeshFireCSS(ActualFireCSS, calcSetup);
			}
		}

		private void RedrawFireCSS()
		{
			if (Presenter != null)
			{
				var drSetup = GetDrawingSetup();
				Presenter.RedrawFireCSS(drSetup);
			}
		}

		private DrawingSetupFire GetDrawingSetup()
		{
			var drSetup = new DrawingSetupFire();

			drSetup.ZoomFactor = (int)NB_zoom.Value;
			drSetup.DrawElements = (bool)CB_femElements.IsChecked;

			return drSetup;
		}

		private void CalculateEvent(object sender, RoutedEventArgs e)
		{
			ActualFireCSS.ClearResults();
			Token = new CancellationTokenSource();

			try
			{
				// callbacks
				CalculationStartedCallback calcStarted = () =>
				{
					CalculationInProgress(true);
				};
				CalculationFinishedCallback calcFinished = (bool cancelled) =>
				{
					CalculationInProgress(false, cancelled);
				};
				RedrawSceneCallback redrawCallback = Presenter.RedrawFireCSS;

				var solver = new FireCSSSolver(ActualFireCSS, Token);
				solver.Calculate(GetCalculationSetup(), redrawCallback, calcStarted, calcFinished, (bool)CB_actionMode.IsChecked);
			}
			catch (Exception ex)
			{
				var win = new ErrWindow(ex);
				win.Show();
				RefreshUI_calcSettings(null, null);
				return;
			}
			//RefreshUI(null, null);
		}

		private void CalculationInProgress(bool inProgress, bool cancelled = false)
		{
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				if (inProgress)
				{
					// https://stackoverflow.com/questions/29198900/disabling-controls-during-processing-a-function
					//CanAction = false; // implementace VM : INotifyPropertyChanged
					ControlsEnabled(false);
					CalculateButtonEnabled(false, true);
				}
				else
				{
					ControlsEnabled(true);
					CalculateButtonEnabled(cancelled, false);
				}
			}));
		}

		private void ControlsEnabled(bool isEnabled)
		{
			BlockCSS.IsEnabled = isEnabled;
			BlockTemperature.IsEnabled = isEnabled;
			BlockFEM.IsEnabled = isEnabled;
			BlockDrawingSettings.IsEnabled = isEnabled;
			NB_animSpeed.IsEnabled = isEnabled;
		}

		private CalculationSetupFire GetCalculationSetup()
		{
			var calcSetup = new CalculationSetupFire();
			calcSetup.ElementsPerUnitLength = (int)NB_numElemPerUnit.Value;
			calcSetup.FinalTime = (double)NB_finalTime.Value;
			calcSetup.TimeStep = (double)NB_timeStep.Value;
			calcSetup.InitialTemperature = (double)NB_tempInit.Value;
			calcSetup.AnimationSpeed = (int)NB_animSpeed.Value / 10;

			return calcSetup;
		}

		private void CalculateButtonEnabled(bool enabled, bool inProgress = false)
		{
			if (BTN_calculate != null)
			{
				BTN_calculate.IsEnabled = enabled;
				if (inProgress)
				{
					TB_calculated.Text = "In progress...";
				}
				else
				{
					TB_calculated.Text = enabled ? "Not calculated..." : "Calculated!";
				}
			}
		}

		private void ActionModeChanged(object sender, RoutedEventArgs e)
		{
			if (!(bool)CB_actionMode.IsChecked)
			{
				Token.Cancel();
			}
			else
			{
				CalculateButtonEnabled(true);
			}
		}
	}
}
