using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FEM_App.FEM_Fire
{
	class FireCSSMesher
	{
		public static void MeshFireCSS(FireCSS css, CalculationSetupFire calcSetup)
		{
			double initTemp = calcSetup.InitialTemperature;
			double elemSize = 0.01 / calcSetup.ElementsPerUnitLength;
			int noElemWidth = (int)(css.Width / elemSize);
			int noElemHeight = (int)(css.Height / elemSize);

			css.FEMElements = new FEMElement2D[noElemWidth, noElemHeight];

			for (int i = 0; i < noElemWidth; i++)
			{
				for (int j = 0; j < noElemHeight; j++)
				{
					var position = new Point(((double)i * elemSize) + elemSize * 0.5 - css.Width / 2,
						((double)j * elemSize) + elemSize * 0.5);
					css.FEMElements[i, j] = new FEMElement2D(position, elemSize, css.Lambda, initTemp);
				}
			}
		}
	}
}
