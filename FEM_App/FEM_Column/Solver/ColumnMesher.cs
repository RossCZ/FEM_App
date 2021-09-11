using System;
using System.Collections.Generic;
using System.Linq;
using win = System.Windows;

namespace FEM_App.FEM_Column
{
	public static class ColumnMesher
	{
		public static void MeshColumn(Column col, CalculationSetupColumn setup)
		{
			col.FEMElements = new Dictionary<int, FEMElement1D>();

			double elemSize = col.Height / setup.NumberOfElements;

			// create nodes
			var nodeList = new FEMNode[setup.NumberOfElements + 1];
			var currentPoint = col.StartPoint;
			for (int i = 0; i < nodeList.Length; i++)
			{
				var position = new win.Point(currentPoint.X + GetInitialDeformationInNode(i + 1, nodeList.Length, setup.InitialMiddleDeflection),
					currentPoint.Y);
				nodeList[i] = new FEMNode(i + 1, position);

				currentPoint = new win.Point(currentPoint.X, position.Y + elemSize);
			}
			col.FEMNodes = nodeList.ToList();

			// create elements
			for (int i = 1; i < nodeList.Length; i++)
			{
				var femElem = new FEMElement1D(i, nodeList[i - 1], nodeList[i]);
				col.FEMElements.Add(i, femElem);
			}

			// no position -> general way would require position and mapping to fem elements when meshing
			// add force (last node)
			nodeList.LastOrDefault().Force = col.Forces[0];
			if (nodeList.Length > 2)
			{
				nodeList.ElementAt(nodeList.Length / 2).Force = col.Forces[1];
			}

			// add support (first and last node)
			nodeList.FirstOrDefault().Support = col.Supports[0];
			nodeList.LastOrDefault().Support = col.Supports[1];
		}

		private static double GetInitialDeformationInNode(int nodeNo, int totalNodes, double initialMiddleDef)
		{
			// half parabolic function from -1 to 1 is used
			var nodeNoDouble = (double)(nodeNo - 1);
			double halfNodeNo = ((double)totalNodes - 1) / 2;
			double transfPosition = nodeNoDouble - halfNodeNo;
			double position = transfPosition / halfNodeNo;

			var weight = -1.0 * Math.Pow(position, 2) + 1.0;
			var deformationInNode = weight * initialMiddleDef;

			return deformationInNode;
		}
	}
}
