using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Wall
{
	public class CalculationSetupWall
	{
		/// <summary>
		/// m
		/// </summary>
		public double ElementSize { get; set; }

		/// <summary>
		/// °C
		/// </summary>
		public double InitialTemperature { get; set; }

		/// <summary>
		/// °C
		/// </summary>
		public double FirstNodeTemperature { get; set; }

		/// <summary>
		/// W/m2/K
		/// </summary>
		public double H_InTheLastNode { get; set; }

		public CalculationSetupWall()
		{
			ElementSize = 0.01;
			InitialTemperature = 20.0;
			FirstNodeTemperature = 45.0;
			H_InTheLastNode = 8.9;
		}
	}
}
