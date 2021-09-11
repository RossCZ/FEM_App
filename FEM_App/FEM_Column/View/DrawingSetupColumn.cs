namespace FEM_App.FEM_Column
{
	public class DrawingSetupColumn
    {
        public int ZoomFactor { get; set; }

		public double DeformationScale { get; set; }

		public bool NodeLabels { get; set; }

		public bool NodeResults_Deformation { get; set; }

		public bool NodeResults_Force { get; set; }

		public bool ElementLabels { get; set; }

		public bool OriginalStructure { get; set; }

		public bool DrawLCS { get; set; }

		public double NodeSize { get; set; }

		public double ForceBaseSize { get; set; }

		public double SupportBaseSize { get; set; }

		public double LCSSignSize { get; set; }

		public DrawingSetupColumn()
        {
			NodeSize = -0.15;
			ForceBaseSize = 80.0;
			SupportBaseSize = 50.0;
			LCSSignSize = 80.0;
		}
    }
}
