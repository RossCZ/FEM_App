using FEM_App.FEM_Column;
using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;

namespace FEM_App.FEM_Wall
{
	public class WallMesher
	{
		public static void MeshWall(Wall wall, CalculationSetupWall setup)
		{
			var startPoint = new Point();
			var currentPoint = startPoint;

			foreach (var layer in wall.Layers)
			{
				int noOfElements = (int)Math.Ceiling(layer.Width / setup.ElementSize);
				layer.FEMElements = new List<FEMElement1D>();

				double elementSize = layer.Width / noOfElements;

				var nodeList = new FEMNode[noOfElements + 1];
				for (int i = 0; i < nodeList.Length; i++)
				{
					var position = new Point(currentPoint.X, currentPoint.Y);
					var femNode = new FEMNode(i + 1, position);
					femNode.InitialTemperature = setup.InitialTemperature;
					nodeList[i] = femNode;

					if (i < nodeList.Length - 1)
					{
						currentPoint = new Point(currentPoint.X + elementSize, position.Y);
					}
				}
				layer.FEMNodes = nodeList.ToList();

				// create elements
				for (int i = 1; i < nodeList.Length; i++)
				{
					var femElem = new FEMElement1D(i, nodeList[i - 1], nodeList[i]);
					femElem.Lambda = layer.Lambda;
					layer.FEMElements.Add(femElem);
				}
			}
		}
	}
}
