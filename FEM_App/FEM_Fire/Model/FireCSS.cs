using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Fire
{
	class FireCSS
	{
		public FireCSS()
		{
			Width = 0.2;
			Height = 0.5;
			Lambda = 1.5; // W/m/K
		}

		public FEMElement2D[,] FEMElements { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public double Lambda { get; set; }

		public EnvironmentOfCSS EnvironmentLeft { get; set; }

		public EnvironmentOfCSS EnvironmentTop { get; set; }

		public EnvironmentOfCSS EnvironmentRight { get; set; }

		public EnvironmentOfCSS EnvironmentBottom { get; set; }

		internal void ClearResults()
		{
			for (int i = 0; i < FEMElements.GetLength(0); i++)
			{
				for (int j = 0; j < FEMElements.GetLength(1); j++)
				{
					FEMElements[i, j].Temperature = FEMElements[i, j].InitialTemperature;
				}
			}
		}
	}
}
