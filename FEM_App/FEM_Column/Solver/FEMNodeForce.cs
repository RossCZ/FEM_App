using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Column
{
	public class FEMNodeForce
	{
		public double Fx { get; set; }
		public double Fy { get; set; }
		public double M { get; set; }

		public FEMNodeForce(double Fx, double Fy, double M)
		{
			this.Fx = Fx;
			this.Fy = Fy;
			this.M = M;
		}
	}
}
