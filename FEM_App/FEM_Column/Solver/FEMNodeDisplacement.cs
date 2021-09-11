namespace FEM_App.FEM_Column
{
	public class FEMNodeDisplacement
	{
		public double u { get; set; }
		public double w { get; set; }
		public double ro { get; set; }

		public FEMNodeDisplacement(double u, double w, double ro)
		{
			this.u = u;
			this.w = w;
			this.ro = ro;
		}
	}
}
