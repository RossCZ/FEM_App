using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Column
{
	public class Support
	{
		public Support(bool X, bool Y, bool Ry)
		{
			this.X = X;
			this.Y = Y;
			this.Ry = Ry;
		}

		public bool X { get; set; }
		public bool Y { get; set; }
		public bool Ry { get; set; }

		public double Fx { get; set; }
		public double Fy { get; set; }
		public double M { get; set; }
	}
}
