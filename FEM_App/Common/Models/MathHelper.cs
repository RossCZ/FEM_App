using Accord.Math;
using System.Collections.Generic;

namespace FEM_App.Common
{
	public static class MathHelper
	{
		// General way is to add by node id
		// Each 3x3 part of the matrix to place where it should be (using nodeid which should correspond to matrix row and column)
		// For column: simplified solution
		// Whole 6x6 matrix is added to certain place in global matrix
		public static void AddElementMatrxToGlobalMatrix(double[,] globalMtx, double[,] elementMtx, int startRow, int startColumn, int nodeVarablesCount)
		{
			var elementVariablesCount = nodeVarablesCount * 2;
			for (int i = 0; i < elementVariablesCount; i++)
			{
				for (int j = 0; j < elementVariablesCount; j++)
				{
					globalMtx[startRow + i, startColumn + j] += elementMtx[i, j];
				}
			}
		}

		public static void AddElementVectorToGlobalVector(double[] globalVec, double[] elementVec, int startRow, int nodeVarablesCount)
		{
			var elementVariablesCount = nodeVarablesCount * 2;
			for (int i = 0; i < elementVariablesCount; i++)
			{
				globalVec[startRow + i] += elementVec[i];
			}
		}

		public static double[,] GetMatrixWithoutIndexes(double[,] matrix, IList<int> supIndexes)
		{
			var mtxSize = matrix.Rows() - supIndexes.Count;
			var reducedMatrix = new double[mtxSize, mtxSize];

			var diffRow = 0;
			var diffColumn = 0;
			for (int i = 0; i < matrix.Rows(); i++)
			{
				if (!supIndexes.Contains(i))
				{
					for (int j = 0; j < matrix.Columns(); j++)
					{
						if (!supIndexes.Contains(j))
						{
							reducedMatrix[i - diffRow, j - diffColumn] = matrix[i, j];
						}
						else
						{
							diffColumn++;
							continue;
						}
					}
					diffColumn = 0;
				}
				else
				{
					diffRow++;
					continue;
				}
			}

			return reducedMatrix;
		}

		public static double[] GetVectorWithoutIndexes(double[] vector, IList<int> supIndexes)
		{
			var vecSize = vector.Length - supIndexes.Count;
			var reducedVector = new double[vecSize];

			var diffRow = 0;
			for (int i = 0; i < vector.Length; i++)
			{
				if (!supIndexes.Contains(i))
				{
					reducedVector[i - diffRow] = vector[i];
				}
				else
				{
					diffRow++;
					continue;
				}
			}

			return reducedVector;
		}
	}
}
