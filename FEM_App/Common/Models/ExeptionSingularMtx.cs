using System;

namespace FEM_App.Common
{
	public class ExeptionSingularMtx : Exception
	{
		public ExeptionSingularMtx()
		{
		}

		public override string Message
		{
			get
			{
				return "Stiffness matrix is singular. Structure is not supported properly.";
			}
		}
	}
}
