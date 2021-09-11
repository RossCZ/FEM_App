using FEM_App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FEM_App.FEM_Fire
{
	class FireCSSPresenter
	{
		private DrawHelper Draw { get; set; }

		private FireCSS ActualFireCSS { get; set; }

		private DrawingSetupFire Setup { get; set; }

		public void RedrawFireCSS(DrawingSetupFire setup)
		{
			Setup = setup;
			RedrawFireCSS();
		}

		public FireCSSPresenter(Canvas canvas, FireCSS fireCSS)
		{
			Draw = new DrawHelper(canvas);
			ActualFireCSS = fireCSS;
		}

		public void RedrawFireCSS()
		{
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				Draw.ClearCanvas();

				// todo
				var pen = new Pen(Brushes.LightGray, 1);
				int zoom = 20 * Setup.ZoomFactor;
				double minTemp = Setup.MinTemperature;
				double maxTemp = Setup.MaxTemperature;

				// mesh
				for (int i = 0; i < ActualFireCSS.FEMElements.GetLength(0); i++)
				{
					for (int j = 0; j < ActualFireCSS.FEMElements.GetLength(1); j++)
					{
						var element = ActualFireCSS.FEMElements[i, j];
						Color fill = GetColorForTemperature(minTemp, maxTemp, element.Temperature);
						Draw.DrawSquareFill(element.Position, element.Size, pen, zoom, fill);

						if (Setup.DrawElements)
						{
							Draw.DrawSquareLines(element.Position, element.Size, pen, zoom);
						}
					}
				}
			}));
		}

		private Color GetColorForTemperature(double minTemp, double maxTemp, double temperature)
		{
			const double hMin = 0;
			const double hMinSkip = 0.20 * 360;
			const double hMaxSkip = 0.46 * 360;
			const double hMax = 0.62 * 360;

			double hRel = (((hMax - hMin) - (hMaxSkip - hMinSkip)) * ((maxTemp - temperature) / (maxTemp - minTemp)));
			double h = hMin + hRel;
			if (h > hMinSkip && h < hMaxSkip)
			{
				h += (hMaxSkip - hMinSkip);
			}

			double l = 0.5;
			double s = 1.0;

			int r, g, b;

			ColorHelper.HlsToRgb(h, l, s, out r, out g, out b);

			var color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);

			return color;
		}
	}
}
