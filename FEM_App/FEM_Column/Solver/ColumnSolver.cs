using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using win = System.Windows;
using Accord.Math;
using FEM_App.Common;

namespace FEM_App.FEM_Column
{
	public class ColumnSolver
	{
		private Column Column { get; set; }

		private CalculationSetupColumn Setup { get; set; }

		public ColumnSolver(Column column, CalculationSetupColumn setup)
		{
			Column = column;
			Setup = setup;
		}

		public void Calculate()
		{
			for (int i = 1; i <= Setup.LoadIncrements; i++)
			{
				CalculateIteration(i);
			}
		}

		private void CalculateIteration(int itNumber)
		{
			var incrementRatio = 1.0 / Setup.LoadIncrements;

			// nodes count
			var nodeCount = Column.FEMElements.Count + 1;

			var KeGlob = Matrix.Create(nodeCount * 3, nodeCount * 3, 0.0);
			var dGlob = Vector.Create(nodeCount * 3, 0.0);
			var FGlob = Vector.Create(nodeCount * 3, 0.0);

			// generate
			for (int i = 0; i < Column.FEMElements.Count; i++)
			{
				var element = Column.FEMElements.ElementAt(i).Value;

				// stiffness matrix
				var mA = (Column.E * Column.A) / (element.Length);
				var mI3 = (12 * Column.E * Column.Iy) / (Math.Pow(element.Length, 3));
				var mI2 = (6 * Column.E * Column.Iy) / (Math.Pow(element.Length, 2));
				var mI1 = (4 * Column.E * Column.Iy) / (element.Length);
				var mI0 = (2 * Column.E * Column.Iy) / (element.Length);

				var stiffMtxRows = new double[6,6]
				{
					{ mA, 0.0, 0.0, -mA, 0.0, 0.0 },
					{ 0.0, mI3, -mI2, 0.0, -mI3, -mI2 },
					{ 0.0, -mI2, mI1, 0.0, mI2, mI0 },
					{ -mA, 0.0, 0.0, mA, 0.0, 0.0 },
					{ 0.0, -mI3, mI2, 0.0, mI3, mI2 },
					{ 0.0, -mI2, mI0, 0.0, mI2, mI1 },
				};
				var ke = Matrix.Create(stiffMtxRows);

				// transformation matrix
				var c = Math.Cos(element.AngleToGCS);
				var s = Math.Sin(element.AngleToGCS);
				var transfMtxRows = new double[6, 6]
				{
					{ c, s, 0.0, 0.0, 0.0, 0.0 },
					{ -s, c, 0.0, 0.0, 0.0, 0.0 },
					{ 0.0, 0.0, 1.0, 0.0, 0.0, 0.0 },
					{ 0.0, 0.0, 0.0, c, s, 0.0 },
					{ 0.0, 0.0, 0.0, -s, c, 0.0 },
					{ 0.0, 0.0, 0.0, 0.0, 0.0, 1.0 },
				};
				var T = Matrix.Create(transfMtxRows);
				var TtranspKe = Matrix.Dot(T.Transpose(), ke);
				var Ke = Matrix.Dot(TtranspKe, T);

				// add to global stiffness matrix
				MathHelper.AddElementMatrxToGlobalMatrix(KeGlob, Ke, i * 3, i * 3, 3);

				// deformation vector
				var snD = element.StartNode.NodalDisplacement;
				var enD = element.EndNode.NodalDisplacement;
				var defVecRows = new double[6]
				{
					snD.u,
					snD.w,
					snD.ro,
					enD.u,
					enD.w,
					enD.ro,
				};
				var defVector = Vector.Create(defVecRows);

				// add to global deformation vector
				MathHelper.AddElementVectorToGlobalVector(dGlob, defVector, i * 3, 3);

				// force vector
				var snF = element.StartNode.NodalForce;
				var enF = element.EndNode.NodalForce;
				var forceVecRows = new double[6]
				{
					snF.Fx,
					snF.Fy,
					snF.M,
					enF.Fx,
					enF.Fy,
					enF.M,
				};
				var forceVector = Vector.Create(forceVecRows);

				// increment effect
				forceVector = forceVector.Multiply(incrementRatio);

				// add to global force vector
				MathHelper.AddElementVectorToGlobalVector(FGlob, forceVector, i * 3, 3);
			}

			// apply increment of external forces
			ExternalForces(FGlob, incrementRatio);

			// calculate deformation and force vector
			// FEM: [K]{d}={F}
			// solve non-support deformations
			SolveNonSupportDeformations(KeGlob, dGlob, FGlob);

			// solve forces
			var resultForce = Matrix.Dot(KeGlob, dGlob);
			//var totalForce = AddPrimaryAndSecondaryReactions(resultForce, FGlob);

			UpdateNodeData(dGlob, resultForce);
		}

		private double[] AddPrimaryAndSecondaryReactions(double[] resultForce, double[] fGlob)
		{
			var result = new double[resultForce.Length];
			for (int i = 0; i < resultForce.Length; i++)
			{
				result[i] = resultForce[i] + fGlob[i];
			}
			return result;
		}

		private void SolveNonSupportDeformations(double[,] keGlob, double[] dGlob, double[] fGlob)
		{
			var supIndexes = GetSupportsIndexes();
			var keGlobRed = MathHelper.GetMatrixWithoutIndexes(keGlob, supIndexes);

			// check stiffness matrix (properly supported structure)
			if (keGlobRed.IsSingular())
			{
				throw new ExeptionSingularMtx();
			}

			var fGlobRed = MathHelper.GetVectorWithoutIndexes(fGlob, supIndexes);

			// solve nonsupport deformations 
			var dGlobRed = Matrix.Dot(keGlobRed.Inverse(), fGlobRed);

			// fill deformation vector
			FEMHelper.FillDeformationVector(dGlob, dGlobRed, supIndexes);
		}

		private IList<int> GetSupportsIndexes()
		{
			var indexes = new List<int>();

			for (int i = 0; i < Column.FEMNodes.Count; i++)
			{
				if (Column.FEMNodes[i].Support != null)
				{
					if (Column.FEMNodes[i].Support.X)
					{
						indexes.Add(i * 3 + 0);
					}
					if (Column.FEMNodes[i].Support.Y)
					{
						indexes.Add(i * 3 + 1);
					}
					if (Column.FEMNodes[i].Support.Ry)
					{
						indexes.Add(i * 3 + 2);
					}
				}
			}

			return indexes;
		}
		
		private void ExternalForces(double[] fGlob, double incrementRatio)
		{
			for (int i = 0; i < Column.FEMNodes.Count; i++)
			{
				if (Column.FEMNodes[i].Force != null)
				{
					fGlob[i * 3 + 0] = Column.FEMNodes[i].Force.Fx * incrementRatio;
					fGlob[i * 3 + 1] = Column.FEMNodes[i].Force.Fy * incrementRatio;
					fGlob[i * 3 + 2] = Column.FEMNodes[i].Force.M * incrementRatio;
				}
			}
		}

		private void UpdateNodeData(double[] resultDef, double[] resultForce)
		{
			for (int i = 0; i < Column.FEMNodes.Count; i++)
			{
				Column.FEMNodes[i].NodalDisplacement.u += resultDef[i * 3 + 0];
				Column.FEMNodes[i].NodalDisplacement.w += resultDef[i * 3 + 1];
				Column.FEMNodes[i].NodalDisplacement.ro += resultDef[i * 3 + 2];

				Column.FEMNodes[i].NodalForce.Fx += resultForce[i * 3 + 0];
				Column.FEMNodes[i].NodalForce.Fy += resultForce[i * 3 + 1];
				Column.FEMNodes[i].NodalForce.M += resultForce[i * 3 + 2];

				var deformedPosition = new win.Point(Column.FEMNodes[i].Position_Deformed.X + resultDef[i * 3 + 0],
					Column.FEMNodes[i].Position_Deformed.Y + resultDef[i * 3 + 1]);
				Column.FEMNodes[i].Position_Deformed = deformedPosition;
			}
		}

		private void SupportDeformations(double[] resultDef)
		{
			for (int i = 0; i < Column.FEMNodes.Count; i++)
			{
				if (Column.FEMNodes[i].Support != null)
				{
					if (Column.FEMNodes[i].Support.X)
					{
						resultDef[i * 3 + 0] = 0;
					}
					if (Column.FEMNodes[i].Support.Y)
					{
						resultDef[i * 3 + 1] = 0;
					}
					if (Column.FEMNodes[i].Support.Ry)
					{
						resultDef[i * 3 + 2] = 0;
					}
				}
			}
		}
	}
}
