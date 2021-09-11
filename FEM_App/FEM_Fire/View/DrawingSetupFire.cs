using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Fire
{
	class DrawingSetupFire
	{
		public DrawingSetupFire()
		{
			MinTemperature = 20;
			MaxTemperature = 800;
		}

		public int ZoomFactor { get; set; }

		public bool DrawElements { get; set; }

		public double MinTemperature { get; set; }

		public double MaxTemperature { get; set; }
	}
}
