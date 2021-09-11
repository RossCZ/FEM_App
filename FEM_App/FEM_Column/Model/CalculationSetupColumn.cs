using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_App.FEM_Column
{
    public class CalculationSetupColumn
    {
        public int LoadIncrements { get; set; }

        public int NumberOfElements { get; set; }

        public double InitialMiddleDeflection { get; set; }

        public CalculationSetupColumn()
        {

        }
    }
}
