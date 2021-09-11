using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.Common
{
	public static class FEMHelper
	{
		public static void FillDeformationVector(double[] dGlob, double[] nonSupDef, IList<int> supIndexes)
		{
			var diffRow = 0;
			for (int i = 0; i < dGlob.Length; i++)
			{
				if (!supIndexes.Contains(i))
				{
					dGlob[i] = nonSupDef[i - diffRow];
				}
				else
				{
					diffRow++;
					continue;
				}
			}
		}
	}
}
