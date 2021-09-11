using System.Collections.Generic;
using System.Linq;

namespace FEM_App.FEM_Wall
{
	public class Wall
	{
		public Wall()
		{
			Layers = new List<WallLayer>();
		}

		public IList<WallLayer> Layers { get; set; }

		public double Width
		{
			get
			{
				return Layers.Sum(l => l.Width);
			}
		}
	}
}
