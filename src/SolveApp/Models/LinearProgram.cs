using System.Collections.Generic;
using SolveApp.Models;

namespace SolveApp.Models
{
    // Class to represent a Linear Program
    public class LinearProgram
    {
        public string Type { get; set; }
        public double[] ObjectiveCoefficients { get; set; }
        public List<Constraint> Constraints { get; set; }
        public string[] Variables { get; set; }
    }
}

