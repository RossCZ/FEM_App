using System;
using System.Windows;

namespace FEM_App.FEM_Column
{
    public class FEMElement1D
    {
		public FEMElement1D(int id, FEMNode startNode, FEMNode endNode)
        {
            Id = id;
            StartNode = startNode;
            EndNode = endNode;
        }

		public int Id { get; set; }

		public FEMNode StartNode { get; set; }

		public FEMNode EndNode { get; set; }

		public double Length
		{
			get
			{
				return (StartNode.Position_Deformed - EndNode.Position_Deformed).Length;
			}
		}

		public double HalfLength
		{
			get
			{
				return Length / 2;
			}
		}

		public double AngleToGCS
		{
			get
			{
				return Math.Atan2((EndNode.Position_Deformed.Y - StartNode.Position_Deformed.Y), (EndNode.Position_Deformed.X - StartNode.Position_Deformed.X));
			}
		}

		// heat transfer data
		public double Lambda { get; set; }
	}
}
