using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using FEM_App.Common;

namespace FEM_App.FEM_Wall
{
	public class WallSolver
	{
		private Wall Wall { get; set; }
		private CalculationSetupWall Setup { get; set; }

		public WallSolver(Wall wall, CalculationSetupWall setup)
		{
			Wall = wall;
			Setup = setup;
		}

		public void Calculate()
		{
			var area = 0.4;
			var nodeCount = Wall.Layers.Sum(l => l.FEMElements.Count()) + 1;
			var currInx = 0;

			var dGlob = Vector.Create(nodeCount, 0.0);
			var KeGlob = Matrix.Create(nodeCount, nodeCount, 0.0);
			var FGlob = Vector.Create(nodeCount, 0.0);

			for (int i = 0; i < Wall.Layers.Count; i++)
			{
				var layer = Wall.Layers[i];
				var elementCount = layer.FEMElements.Count();

				for (int j = 0; j < elementCount; j++)
				{
					var element = layer.FEMElements[j];

					// temperature transfer "stiffness"
					var kA_l = element.Lambda * area / element.Length;

					var stiffMtxRows = new double[2, 2]
					{
						{ kA_l, -kA_l },
						{ -kA_l, kA_l },
					};

					// element matrix
					var ke = Matrix.Create(stiffMtxRows);

					MathHelper.AddElementMatrxToGlobalMatrix(KeGlob, ke, currInx, currInx, 1);

					// force vector = convention outside (=0)
					var forceVecRows = new double[2]
					{
						0,
						0,
					};
					var forceVector = Vector.Create(forceVecRows);

					// add to global force vector
					MathHelper.AddElementVectorToGlobalVector(FGlob, forceVector, currInx, 1);

					currInx++;
				}
			}

			// boundary conditions (first and last node)
			// first node = heat source
			dGlob[0] = Setup.FirstNodeTemperature;

			// last node = heat output
			var hA = Setup.H_InTheLastNode * area;
			var hATinit = Setup.H_InTheLastNode * area * Setup.InitialTemperature;
			KeGlob[nodeCount - 1, nodeCount- 1] += hA;

			FGlob[nodeCount - 1] += hATinit;

			// SOLUTION
			var supIndexes = new List<int>() { 0 };
			var keGlobRed = MathHelper.GetMatrixWithoutIndexes(KeGlob, supIndexes);

			// check stiffness matrix (properly supported structure)
			if (keGlobRed.IsSingular())
			{
				throw new ExeptionSingularMtx();
			}

			var fGlobRed = MathHelper.GetVectorWithoutIndexes(FGlob, supIndexes);
			// tohle odvodit proč to tak je... (pokud redukuji matici o nultý index -> force vector na indexu 0+1 -= tuhost na [1,0] * podporová teplota
			fGlobRed[0] -= KeGlob[1, 0] * Setup.FirstNodeTemperature;

			// solve nonsupport deformations 
			var dGlobRed = Matrix.Dot(keGlobRed.Inverse(), fGlobRed);
			FEMHelper.FillDeformationVector(dGlob, dGlobRed, supIndexes);

			// WRITE DATA TO NODES
			var currResInx = 0;
			for (int i = 0; i < Wall.Layers.Count; i++)
			{
				var layer = Wall.Layers[i];
				var layerNodeCount = layer.FEMNodes.Count();

				for (int j = 0; j < layerNodeCount; j++)
				{
					layer.FEMNodes[j].Temperature = dGlob[currResInx];
					currResInx++;
				}
				// end node of previous layer is the same as start node of next layer
				currResInx--;
			}
		}
	}
}
