using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static FEM_App.FEM_Fire.FEM_Fire_Main;
using win = System.Windows;


namespace FEM_App.FEM_Fire
{
	class FireCSSSolver
	{
		private FireCSS FireCSS { get; set; }

		private CancellationTokenSource Token { get; set; }

		public FireCSSSolver(FireCSS fireCSS, CancellationTokenSource token)
		{
			FireCSS = fireCSS;
			Token = token;
		}

		public void Calculate(CalculationSetupFire setup, RedrawSceneCallback redrawScene,
			CalculationStartedCallback calcStarted, CalculationFinishedCallback calcFinished, bool doEvents = false)
		{
			Task.Run(() =>
			{
				calcStarted.Invoke();

				double fireLeft = FireCSS.EnvironmentLeft.Temperature;
				double fireTop = FireCSS.EnvironmentTop.Temperature;
				double fireRight = FireCSS.EnvironmentRight.Temperature;
				double fireBottom = FireCSS.EnvironmentBottom.Temperature;
				double finalTime = setup.FinalTime;
				double timeStep = setup.TimeStep;
				int animationSpeed = setup.AnimationSpeed;
				int timeSteps = (int)(finalTime / timeStep);

				// extend mesh of boundary (fire) elements
				var noElemWidth = FireCSS.FEMElements.GetLength(0);
				var noElemHeight = FireCSS.FEMElements.GetLength(1);
				var noElemWidthExt = noElemWidth + 2;
				var noElemHeightExt = noElemHeight + 2;

				int refreshRate = noElemWidth * noElemHeight;

				var tempMesh = new FEMElement2D[noElemWidthExt, noElemHeightExt];

				for (int i = 0; i < noElemWidthExt; i++)
				{
					for (int j = 0; j < noElemHeightExt; j++)
					{
						// set fire elements
						if (i == 0)
						{
							tempMesh[i, j] = new FEMElement2D(new win.Point(), 0, 0, fireLeft);
						}
						else if (i == noElemWidthExt - 1)
						{
							tempMesh[i, j] = new FEMElement2D(new win.Point(), 0, 0, fireRight);
						}
						else if (j == 0)
						{
							tempMesh[i, j] = new FEMElement2D(new win.Point(), 0, 0, fireBottom);
						}
						else if (j == noElemHeightExt - 1)
						{
							tempMesh[i, j] = new FEMElement2D(new win.Point(), 0, 0, fireTop);
						}
						else
						{
							tempMesh[i, j] = FireCSS.FEMElements[i - 1, j - 1];
						}
					}
				}

				// calculate
				for (int t = 0; t < timeSteps; t++)
				{
					var tempChanges = new double[noElemWidthExt, noElemHeightExt];

					// get temp change
					for (int i = 0; i < noElemWidthExt; i++)
					{
						for (int j = 0; j < noElemHeightExt; j++)
						{
							// skip fire elements
							if (i == 0 || i == noElemWidthExt - 1 || j == 0 || j == noElemHeightExt - 1)
							{
								continue;
							}

							// not correct...temporary solution
							double tempChangeInElem =
								 (((tempMesh[i - 1, j].Temperature
								+ tempMesh[i + 1, j].Temperature
								+ tempMesh[i, j - 1].Temperature
								+ tempMesh[i, j + 1].Temperature) / 4) - tempMesh[i, j].Temperature);

							// magic happens here..
							// get some number between 0 (zero temp change) and 1 (full temp change) based on parameters
							double lambdaRatio = Math.Max(0.01, 1 - 1 / (tempMesh[i, j].Lambda));
							//double timeRatio = Math.Max(0.01, 1 - 1 / (timeStep * 20));
							//double sizeRatio = Math.Max(0.01, 1 - 1 / (tempMesh[i, j].Size * 1000));
							double timeSizeRatio = Math.Max(0.01, 1 - 1 / (timeStep / tempMesh[i, j].Size));
							double ratio = lambdaRatio * timeSizeRatio; // timeRatio * sizeRatio;

							tempChanges[i, j] = tempChangeInElem * ratio;
						}
					}

					// apply temp change
					for (int i = 0; i < noElemWidthExt; i++)
					{
						for (int j = 0; j < noElemHeightExt; j++)
						{
							// skip fire elements
							if (i == 0 || i == noElemWidthExt - 1 || j == 0 || j == noElemHeightExt - 1)
							{
								continue;
							}

							tempMesh[i, j].Temperature += tempChanges[i, j];
						}
					}

					// action mode
					if (doEvents && (t % animationSpeed == 0))
					{
						redrawScene.Invoke();
						Thread.Sleep(refreshRate);
					}
					if (Token.IsCancellationRequested)
					{
						calcFinished.Invoke(true);
						return;
					}
				}

				var middle = tempMesh[noElemWidthExt / 2, noElemHeightExt / 2];

				calcFinished.Invoke(false);
				redrawScene.Invoke();
			});
		}
	}
}
