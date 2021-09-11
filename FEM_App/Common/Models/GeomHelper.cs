using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FEM_App.Common
{
    public static class GeomHelper
    {
		public static Vector GetRotatedVector(Vector vector, double angleDeg)
        {
            var radians = angleDeg * Math.PI / 180;
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector(ca * vector.X - sa * vector.Y, sa * vector.X + ca * vector.Y);
        }
    }
}
