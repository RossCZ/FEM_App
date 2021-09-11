using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FEM_App.FEM_Fire
{
	class FEMElement2D
	{
		public FEMElement2D(Point position, double size, double lambda, double initTemp)
		{
			Position = position;
			Size = size;
			Lambda = lambda;
			Temperature = initTemp;
			InitialTemperature = initTemp;
		}

		public Point Position { get; set; }

		public double Size { get; set; }

		public double Lambda { get; set; }

		public double Temperature { get; set; }

		public double InitialTemperature { get; set; }
	}
}
