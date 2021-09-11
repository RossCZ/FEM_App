using System.Windows;

namespace FEM_App.FEM_Column
{
	public class FEMNode
	{
		public FEMNode(int id, Point position)
		{
			Id = id;
			Position_Deformed = position;
			Position_Original = position;
			NodalDisplacement = new FEMNodeDisplacement(0, 0, 0);
			NodalForce = new FEMNodeForce(0, 0, 0);
			Support = null;
			Force = null;
		}

		public Point Position_Deformed { get; set; }

		public Point Position_Original { get; set; }

		public int Id { get; set; }

		// mechanical data
		public FEMNodeDisplacement NodalDisplacement { get; set; }

		public FEMNodeForce NodalForce { get; set; }

		public Support Support { get; set; }

		public Force Force { get; set; }

		// heat transfer data
		public double InitialTemperature { get; set; }

		public double Temperature { get; set; }
	}
}
