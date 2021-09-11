using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Fire
{
	class EnvironmentOfCSS
	{
		public EnvironmentOfCSS(double temperature)
		{
			Temperature = temperature;
		}

		public double Temperature { get; set; }
	}
}
