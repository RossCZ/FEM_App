using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace FEM_App.FEM_Column
{
	public class Column
	{
		public Column()
		{
			Height = 5.0;
			Forces = new List<Force>();
			Supports = new List<Support>();
			E = 30.0e9;
			A = 0.1;
			Iy = 0.001;
		}

		public Point StartPoint
		{
			get
			{
				return new Point();
			}
		}

		public Point EndPoint
		{
			get
			{
				return new Point(0, Height);
			}
		}

		public double Height { get; set; }

		public IList<Force> Forces { get; set; }

		public IList<Support> Supports { get; set; }

		public IDictionary<int, FEMElement1D> FEMElements { get; set; }

		public IList<FEMNode> FEMNodes { get; set; }

		public double E { get; set; }

		public double Iy { get; set; }

		public double A { get; set; }

		internal void ClearResults()
		{
			foreach (var node in FEMNodes)
			{
				node.NodalDisplacement = new FEMNodeDisplacement(0, 0, 0);
				node.NodalForce = new FEMNodeForce(0, 0, 0);
			}
		}
	}
}
