using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FEM_App.Common
{
    public class DrawHelper
    {
        private Canvas canvas;

        public DrawHelper(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void DrawLinesFromPoints(Point[] points, Pen pen, int zoom, bool close = false)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i], points[i + 1], pen, zoom);
            }

            if (close)
            {
                DrawLine(points[points.Length - 1], points[0], pen, zoom);
            }
        }

		internal void DrawSquareFill(Point posCenter, double size, Pen pen, int zoom, Color fill)
		{
			var pt1 = new Point(posCenter.X + size / 2, posCenter.Y + size / 2);
			var pt2 = new Point(posCenter.X - size / 2, posCenter.Y + size / 2);
			var pt3 = new Point(posCenter.X - size / 2, posCenter.Y - size / 2);

			DrawSolidRectangle(pt2, pt3, pt1, fill, zoom);
		}

		internal void DrawSquareLines(Point posCenter, double size, Pen pen, int zoom)
		{
			var pt1 = new Point(posCenter.X + size / 2, posCenter.Y + size / 2);
			var pt2 = new Point(posCenter.X - size / 2, posCenter.Y + size / 2);
			var pt3 = new Point(posCenter.X - size / 2, posCenter.Y - size / 2);
			var pt4 = new Point(posCenter.X + size / 2, posCenter.Y - size / 2);

			DrawLine(pt1, pt2, pen, zoom);
			DrawLine(pt2, pt3, pen, zoom);
			DrawLine(pt3, pt4, pen, zoom);
			DrawLine(pt4, pt1, pen, zoom);
		}

		public void ClearCanvas()
        {
            canvas.Children.Clear();
        }

        public void DrawLine(Point pt1, Point pt2, Pen pen, int zoom)
        {
            var pt1Local = GetPointCoordinatesForCanvas(pt1, zoom);
            var pt2Local = GetPointCoordinatesForCanvas(pt2, zoom);
            if (pt1Local.X < 0 || pt1Local.Y < 0 || pt2Local.X < 0 || pt2Local.Y < 0)
            {
                SomethingIsOutsideOfCanvas();
                return;
            }

            var line = new Line()
            {
                X1 = pt1Local.X,
                Y1 = pt1Local.Y,
                X2 = pt2Local.X,
                Y2 = pt2Local.Y
            };

            line.Stroke = pen.Brush;
            line.StrokeThickness = pen.Thickness;
            canvas.Children.Add(line);
        }

		public void DrawSolidRectangle(Point upperLeft, Point ptDown, Point ptLeft, Color fill, int zoom)
		{
			Rectangle rect;
			rect = new Rectangle();
			rect.Stroke = new SolidColorBrush(fill);

			var upperLeftLocal = GetPointCoordinatesForCanvas(upperLeft, zoom);
			var ptDownLocal = GetPointCoordinatesForCanvas(ptDown, zoom);
			var ptLeftLocal = GetPointCoordinatesForCanvas(ptLeft, zoom);

			if (upperLeftLocal.X < 0 || upperLeftLocal.Y < 0)
			{
				SomethingIsOutsideOfCanvas();
				return;
			}

			rect.Width = Math.Abs(upperLeftLocal.X - ptLeftLocal.X);
			rect.Height = Math.Abs(upperLeftLocal.Y - ptDownLocal.Y);

			rect.Fill = new SolidColorBrush(fill);

			Canvas.SetLeft(rect, upperLeftLocal.X);
			Canvas.SetTop(rect, upperLeftLocal.Y);
			canvas.Children.Add(rect);
		}

        private void SomethingIsOutsideOfCanvas()
        {
            var middle = new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2);
            DrawText(middle, "Zoom is too large, some elements are not shown", 20, Colors.Red, 0);
        }

        public void DrawText(Point position, string message, double fontSize, Color color, int zoom, 
			HorizontalAlignment horAlignment = HorizontalAlignment.Center)
        {
            TextBlock text = new TextBlock();
            text.Text = message;
            text.FontSize = fontSize;
			text.Foreground = new SolidColorBrush(color);
            text.HorizontalAlignment = horAlignment;
            text.VerticalAlignment = VerticalAlignment.Center;

            Point point;
			if (zoom == 0)
            {
                point = position;
            }
			else
            {
                point = GetPointCoordinatesForCanvas(position, zoom);
            }
            point.Y -= fontSize / 2;
			if (horAlignment == HorizontalAlignment.Center)
			{
				point.X -= (message.Count() * fontSize * 0.5) / 2;
			}

            text.Margin = new Thickness(point.X, point.Y, 0, 0);
            canvas.Children.Add(text);
        }

		public void DrawArrow(Point pt, Vector dirAndSize, Pen pen, int zoom)
        {
            var pt2 = pt - dirAndSize;
            DrawLine(pt, pt2, pen, zoom);

            var sizeSides = 0.3;
			var vecLeft = GeomHelper.GetRotatedVector(new Vector(dirAndSize.X * sizeSides, dirAndSize.Y * sizeSides), 30);
            var pt3 = pt - vecLeft;
            DrawLine(pt, pt3, pen, zoom);

            var vecRight = GeomHelper.GetRotatedVector(new Vector(dirAndSize.X * sizeSides, dirAndSize.Y * sizeSides), -30);
            var pt4 = pt - vecRight;
            DrawLine(pt, pt4, pen, zoom);
        }

        public void DrawCircle(Point pt, double radius, Pen pen, int zoom)
        {
            var position = GetPointCoordinatesForCanvas(pt, zoom);
            if (position.X < 0 || position.Y < 0)
            {
                SomethingIsOutsideOfCanvas();
                return;
            }

            var circ = new Ellipse();
            var size = radius < 0 ? Math.Abs(radius * 100) : radius * zoom;
            circ.Width = size;
            circ.Height = size;

            double left = position.X - (size / 2);
            double top = position.Y - (size / 2);

            circ.Margin = new Thickness(left, top, 0, 0);

            circ.Stroke = pen.Brush;
            circ.StrokeThickness = pen.Thickness;
            canvas.Children.Add(circ);
        }

		private Point GetPointCoordinatesForCanvas(Point pt, int zoom)
        {
            var h = canvas.ActualHeight;
            var offsetX = canvas.ActualWidth / 2;
            var offsetY = h * 0.08;

            var xCoord = offsetX + pt.X * zoom;
            var yCoord = h - pt.Y * zoom - offsetY;

            return new Point(xCoord, yCoord);
        }
    }
}
