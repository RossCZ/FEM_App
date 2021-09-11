using FEM_App.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace FEM_App.FEM_Wall
{
    public class WallPresenter
    {
		private DrawHelper Draw { get; set; }

		private DrawingSetupWall Setup { get; set; }

		private Wall ActualWall { get; set; }

		private Pen[] PenList { get; set; }

		public WallPresenter(Canvas canvas, Wall wall)
		{
			Draw = new DrawHelper(canvas);
			ActualWall = wall;

			CreatePens();
		}

		private void CreatePens()
		{
			PenList = new Pen[]
			{
				new Pen(Brushes.Black, 3),
				new Pen(Brushes.Black, 1),
				new Pen(Brushes.Green, 2),
				new Pen(Brushes.DarkRed, 2),
				new Pen(Brushes.LightGray, 1),
				new Pen(Brushes.Red, 2),
			};
		}

		public void RedrawWall(DrawingSetupWall drSetup)
		{
			Setup = drSetup;

			Draw.ClearCanvas();

			DrawLayers();
		}

		private double ZeroXOffset()
		{
			return (-1.0 * ActualWall.Width / 2);
		}

		private double MaxHeight()
		{
			return (ActualWall.Width / 3);
		}

		private void DrawLayers()
		{
			int zoom = (int)(Setup.ZoomFactor * 10 / ActualWall.Width);
			double height = MaxHeight();
			var currentX = ZeroXOffset();
			var tempMin = 0.0;
			var tempMax = 0.0; 

			if (ActualWall.Layers.Any(l => l.FEMNodes.Any()))
			{
				tempMin = ActualWall.Layers.Min(l => l.FEMNodes.Min(n => n.Temperature));
				tempMax = ActualWall.Layers.Max(l => l.FEMNodes.Max(n => n.Temperature));
			}

			foreach (var layer in ActualWall.Layers)
			{
				// beginning
				var sp = new Point(currentX, 0);
				var ep = new Point(currentX, height);
				Draw.DrawLine(sp, ep, PenList[1], zoom);

				// end
				sp = new Point(currentX + layer.Width, 0);
				ep = new Point(currentX + layer.Width, height);
				Draw.DrawLine(sp, ep, PenList[1], zoom);

				// temperature
				if (layer.FEMNodes.Any())
				{
					var first = layer.FEMNodes.FirstOrDefault();
					var last = layer.FEMNodes.LastOrDefault();

					// line
					var sp_temp = new Point(currentX, GetTemperatureYPosition(first.Temperature, tempMin, tempMax));
					var ep_temp = new Point(currentX + layer.Width, GetTemperatureYPosition(last.Temperature, tempMin, tempMax));
					Draw.DrawLine(sp_temp, ep_temp, PenList[5], zoom);

					// label
					var pt_offX = height * 0.02;
					var pt_offY = height * 0.05;
					Draw.DrawText(new Point(sp_temp.X + pt_offX, sp_temp.Y + pt_offY), string.Format("{0:F2} °C", first.Temperature), 12, Colors.Black, zoom, HorizontalAlignment.Left);
					Draw.DrawText(new Point(ep_temp.X + pt_offX, ep_temp.Y + pt_offY), string.Format("{0:F2} °C", last.Temperature), 12, Colors.Black, zoom, HorizontalAlignment.Left);
				}

				currentX += layer.Width;
			}
		}

		private double GetTemperatureYPosition(double temperature, double min, double max)
		{
			double maxHeight = MaxHeight();
			double tempHeight = maxHeight * 0.8;
			var begOffset = maxHeight * 0.1;

			var spanAll = max - min;
			var spanThis = temperature - min;

			if (spanAll == 0)
			{
				return tempHeight + begOffset;
			}
			else
			{
				return (spanThis / spanAll) * tempHeight + begOffset;
			}
		}

		public void PresentTextInfo(Wall wall, CalculationSetupWall setup, TextBlock textBlock, bool addResults = false)
		{
			var textResult = string.Format("Wall; width {0:F3} m; layers: {1}; T_out: {2:F1} °C; T_in: {3:F1} °C; h_wall->in: {4:F2} W/m2/K\n",
				wall.Width, wall.Layers.Count, setup.FirstNodeTemperature, setup.InitialTemperature, setup.H_InTheLastNode);

			for (int i = 0; i < wall.Layers.Count; i++)
			{
				var layer = wall.Layers[i];

				textResult += string.Format("\tLayer {0} - {1}; width {2:F3} m; lambda {3:F3} W/m/K", i + 1, layer.Name, layer.Width, layer.Lambda);
				textResult += "\n";

				if (addResults)
				{
					var nodeCount = layer.FEMNodes.Count;
					for (int j = 0; j < nodeCount; j++)
					{
						var node = layer.FEMNodes[j];
						textResult += string.Format("\t\t {0};\t x: {1:F3} m;\t T: {2:F1} °C", j + 1, node.Position_Original.X, node.Temperature);
						textResult += "\n";
					}
				}
			}

			textBlock.Text = textResult;
		}
	}
}
