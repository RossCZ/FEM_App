using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Fire
{
	class CalculationSetupFire
	{
		public int ElementsPerUnitLength { get; set; }

		public double TimeStep { get; set; }

		public double FinalTime { get; set; }

		public double InitialTemperature { get; set; }

		public int AnimationSpeed { get; set; }

		public CalculationSetupFire()
		{

		}
	}
}
