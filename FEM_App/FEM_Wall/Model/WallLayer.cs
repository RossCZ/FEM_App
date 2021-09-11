using FEM_App.FEM_Column;
using System;
using System.Collections.Generic;

namespace FEM_App.FEM_Wall
{
	public class WallLayer
	{
		public WallLayer(WallLayer src) : this(true)
		{
			Name = src.Name;
			Width = src.Width;
			Lambda = src.Lambda;
		}

		public WallLayer() : this(true)
		{
			Name = "Layer";
			Width = 0.1;
			Lambda = 0.15;
		}

		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param name="name">name</param>
		/// <param name="width">m</param>
		/// <param name="lambda">W/m/K</param>
		public WallLayer(string name, double width, double lambda) : this(true)
		{
			Name = name;
			Width = width;
			Lambda = lambda;
		}

		public void UpdateLayer(string name, double width, double lambda)
		{
			Name = name;
			Width = width;
			Lambda = lambda;
		}

		private WallLayer(bool initializeLists)
		{
			if (initializeLists)
			{
				FEMElements = new List<FEMElement1D>();
				FEMNodes = new List<FEMNode>();
			}
		}

		public string Name { get; private set; }

		public double Width { get; private set; }

		public double Lambda { get; private set; }

		public IList<FEMElement1D> FEMElements { get; set; }

		public IList<FEMNode> FEMNodes { get; set; }
	}
}
