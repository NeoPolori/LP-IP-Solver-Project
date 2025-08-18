using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveApp
{
    
        /// <summary>
        /// Represents a Linear Programming (LP) model.
        /// </summary>
        public class LinearProgram
        {
            public string Type { get; set; } // "max" or "min"
            public double[] ObjectiveCoefficients { get; set; }
            public List<Constraint> Constraints { get; set; }
            public string[] Variables { get; set; }
        }
    
}
