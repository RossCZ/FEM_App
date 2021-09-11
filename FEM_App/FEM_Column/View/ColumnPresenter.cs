using FEM_App.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace FEM_App.FEM_Column
{
    public class ColumnPresenter
    {
        private DrawHelper Draw { get; set; }

        private Column ActualColumn { get; set; }

		private Pen[] PenList { get; set; }

        public ColumnPresenter(Canvas canvas, Column column)
        {
            Draw = new DrawHelper(canvas);
            ActualColumn = column;

            CreatePens();
        }

        private void CreatePens()
        {
            PenList = new Pen[]
            {
                new Pen(Brushes.Black, 3),
                new Pen(Brushes.Black, 1),
				new Pen(Brushes.Green, 2),
                new Pen(Brushes.DarkRed, 2),
				new Pen(Brushes.LightGray, 1),
				new Pen(Brushes.Red, 2),
			};
        }

        public void RedrawColumn(DrawingSetupColumn drSetup)
        {
            Draw.ClearCanvas();

			if (drSetup.OriginalStructure)
			{
				// original mesh
				DrawElements_Original(drSetup);
				DrawNodes_Original(drSetup);
			}

			// deformed mesh
			DrawElements_Deformed(drSetup);
			DrawNodes_Deformed(drSetup);
			DrawNodeResults(drSetup);

			DrawLoads(drSetup);
            DrawSupports(drSetup);
			DrawLCS(drSetup);

			// old 
			//DrawColumn(drSetup);
		}

		private void DrawLCS(DrawingSetupColumn drSetup)
		{
			if (drSetup.DrawLCS)
			{
				var size = drSetup.LCSSignSize / (double)drSetup.ZoomFactor;
				var origin = new Point(0, 0);
				var ptX = new Point(origin.X + size, 0);
				Draw.DrawLine(origin, ptX, PenList[5], drSetup.ZoomFactor);

				Draw.DrawText(new Point(ptX.X, ptX.Y - size * 0.2), "X", 20, Colors.Red, drSetup.ZoomFactor);

				var ptY = new Point(origin.X, origin.Y + size);
				Draw.DrawLine(origin, ptY, PenList[2], drSetup.ZoomFactor);

				Draw.DrawText(new Point(ptY.X - size * 0.2, ptY.Y), "Y", 20, Colors.Green, drSetup.ZoomFactor);
			}
		}

		private void DrawSupports(DrawingSetupColumn drSetup)
        {
			var nodesWithSupports = ActualColumn.FEMNodes.Where(fn => fn.Support != null);
			foreach (var femNode in nodesWithSupports)
			{
				// movable X
				if (femNode.Support.X && !femNode.Support.Y)
				{
					DrawXSupport(drSetup, femNode.Position_Original);
				}

				// movable Y
				if (!femNode.Support.X && femNode.Support.Y)
				{
					DrawYSupport(drSetup, femNode.Position_Original);
				}

				// fixed X
				if (femNode.Support.X && femNode.Support.Y)
				{
					DrawXYSupport(drSetup, femNode.Position_Original);
				}

				// fixed rotation (add square)
				if (femNode.Support.Ry)
				{
					DrawXYRzSupport(drSetup, femNode.Position_Original);
				}
			}
        }

		private void DrawXYSupport(DrawingSetupColumn drSetup, Point position)
		{
			double size = drSetup.SupportBaseSize / drSetup.ZoomFactor;

			var ptF1 = position;
			var ptF2 = new Point(ptF1.X - size / 3, ptF1.Y - size * 2 / 3);
			var ptF3 = new Point(ptF1.X + size / 3, ptF1.Y - size * 2 / 3);
			var arrayF = new Point[] { ptF1, ptF2, ptF3 };
			Draw.DrawLinesFromPoints(arrayF, PenList[3], drSetup.ZoomFactor, true);
		}

		private void DrawXYRzSupport(DrawingSetupColumn drSetup, Point position)
		{
			double size = drSetup.SupportBaseSize / drSetup.ZoomFactor;

			var ptF1 = new Point(position.X + size / 2, position.Y + size / 2);
			var ptF2 = new Point(position.X - size / 2, position.Y + size / 2);
			var ptF3 = new Point(position.X - size / 2, position.Y - size / 2);
			var ptF4 = new Point(position.X + size / 2, position.Y - size / 2);
			var arrayF = new Point[] { ptF1, ptF2, ptF3, ptF4 };
			Draw.DrawLinesFromPoints(arrayF, PenList[3], drSetup.ZoomFactor, true);
		}

		private void DrawXSupport(DrawingSetupColumn drSetup, Point position)
		{
			double size = drSetup.SupportBaseSize / drSetup.ZoomFactor;

			var ptM1 = position;
			var ptM2 = new Point(ptM1.X - size * 2 / 3, ptM1.Y - size / 3);
			var ptM3 = new Point(ptM1.X - size * 2 / 3, ptM1.Y + size / 3);
			var arrayM = new Point[] { ptM1, ptM2, ptM3 };
			Draw.DrawLinesFromPoints(arrayM, PenList[3], drSetup.ZoomFactor, true);

			var ptM4 = new Point(ptM1.X - size * 0.8, ptM1.Y - size / 3);
			var ptM5 = new Point(ptM1.X - size * 0.8, ptM1.Y + size / 3);
			Draw.DrawLine(ptM4, ptM5, PenList[3], drSetup.ZoomFactor);
		}

		private void DrawYSupport(DrawingSetupColumn drSetup, Point position)
		{
			double size = drSetup.SupportBaseSize / drSetup.ZoomFactor;

			var ptM1 = position;
			var ptM2 = new Point(ptM1.X - size / 3, ptM1.Y - size * 2 / 3);
			var ptM3 = new Point(ptM1.X + size / 3, ptM1.Y - size * 2 / 3);
			var arrayM = new Point[] { ptM1, ptM2, ptM3 };
			Draw.DrawLinesFromPoints(arrayM, PenList[3], drSetup.ZoomFactor, true);

			var ptM4 = new Point(ptM1.X - size / 3, ptM1.Y - size * 0.8);
			var ptM5 = new Point(ptM1.X + size / 3, ptM1.Y - size * 0.8);
			Draw.DrawLine(ptM4, ptM5, PenList[3], drSetup.ZoomFactor);
		}

		private void DrawLoads(DrawingSetupColumn drSetup)
        {
			var nodesWithForces = ActualColumn.FEMNodes.Where(fn => fn.Force != null && (fn.Force.Fx != 0 || fn.Force.Fy != 0 || fn.Force.M != 0));
			foreach (var femNode in nodesWithForces)
			{
				//var dirAndSizeVec = new Vector(0, -ActualColumn.ForceSize / drSetup.ZoomFactor); // relative size
				var dirAndSizeVec = new Vector(drSetup.ForceBaseSize * Math.Sign(femNode.Force.Fx) / drSetup.ZoomFactor, 
					drSetup.ForceBaseSize * Math.Sign(femNode.Force.Fy) / drSetup.ZoomFactor);
				var point = femNode.Position_Deformed;
				Draw.DrawArrow(point, dirAndSizeVec, PenList[2], drSetup.ZoomFactor);

				var end = point - dirAndSizeVec;

				var forceSize = Math.Sqrt(Math.Pow(femNode.Force.Fx, 2) + Math.Pow(femNode.Force.Fy, 2));
				Draw.DrawText(end, string.Format("{0} kN", forceSize / 1e3), 14, Colors.Green, drSetup.ZoomFactor);
			}
        }

        private void DrawElements_Original(DrawingSetupColumn drSetup)
		{
			foreach (var femElement in ActualColumn.FEMElements.Values)
			{
				Draw.DrawLine(femElement.StartNode.Position_Original, femElement.EndNode.Position_Original, PenList[4], drSetup.ZoomFactor);
			}
		}

		private void DrawElements_Deformed(DrawingSetupColumn drSetup)
		{
			foreach (var femElement in ActualColumn.FEMElements.Values)
			{
				var startDef = GetDeformedNodePosition(drSetup, femElement.StartNode);
				var endDef = GetDeformedNodePosition(drSetup, femElement.EndNode);
				Draw.DrawLine(startDef,	endDef, PenList[1], drSetup.ZoomFactor);

				if (drSetup.ElementLabels)
				{
					var middle = new Point((startDef.X + endDef.X) / 2,
					(startDef.Y + endDef.Y) / 2);
					Draw.DrawText(middle, string.Format("E{0}", femElement.Id), 12, Colors.Black, drSetup.ZoomFactor);
				}
			}
		}

		private void DrawNodes_Original(DrawingSetupColumn drSetup)
		{
			foreach (var femNode in ActualColumn.FEMNodes)
			{
				Draw.DrawCircle(femNode.Position_Original, drSetup.NodeSize, PenList[4], drSetup.ZoomFactor);
			}
		}

		private void DrawNodes_Deformed(DrawingSetupColumn drSetup)
		{
			foreach (var femNode in ActualColumn.FEMNodes)
			{
				var pozDef = GetDeformedNodePosition(drSetup, femNode);
				Draw.DrawCircle(pozDef, drSetup.NodeSize, PenList[1], drSetup.ZoomFactor);

				if (drSetup.NodeLabels)
				{
					Draw.DrawText(pozDef, string.Format("N{0}", femNode.Id), 12, Colors.Black, drSetup.ZoomFactor);
				}
			}
		}

		private void DrawNodeResults(DrawingSetupColumn drSetup)
		{
			foreach (var femNode in ActualColumn.FEMNodes)
			{
				string resultText = string.Empty;
				if (drSetup.NodeResults_Force)
				{
					resultText += string.Format("Fx: {0:F2} kN, Fy: {1:F2} kN, M: {2:F2} kNm",
						femNode.NodalForce.Fx / 1e3, femNode.NodalForce.Fy / 1e3, femNode.NodalForce.M / 1e3);
					if (drSetup.NodeResults_Deformation)
					{
						resultText += Environment.NewLine;
					}
				}

				if (drSetup.NodeResults_Deformation)
				{
					resultText += string.Format("u: {0:F3} mm, w: {1:F3} mm, ro: {2:F3} rad*10^5",
						femNode.NodalDisplacement.u * 1e3, femNode.NodalDisplacement.w * 1e3, femNode.NodalDisplacement.ro * 1e5);
				}

				if (!string.IsNullOrEmpty(resultText))
				{
					var defPoz = GetDeformedNodePosition(drSetup, femNode);
					Draw.DrawText(new Point(defPoz.X + 0.2, defPoz.Y),
						resultText, 12, Colors.Red, drSetup.ZoomFactor, HorizontalAlignment.Left);
				}
			}
		}

		private Point GetDeformedNodePosition(DrawingSetupColumn drSetup, FEMNode node)
		{
			var defPoz = new Point(node.Position_Original.X + node.NodalDisplacement.u * drSetup.DeformationScale,
					node.Position_Original.Y + node.NodalDisplacement.w * drSetup.DeformationScale);

			return defPoz;
		}

		private void DrawColumn(DrawingSetupColumn drSetup)
		{
			Draw.DrawLine(ActualColumn.StartPoint, ActualColumn.EndPoint, PenList[0], drSetup.ZoomFactor);
		}
	}
}
