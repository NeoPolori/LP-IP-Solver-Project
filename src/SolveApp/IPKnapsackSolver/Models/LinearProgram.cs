using System.Collections.Generic;
using IPKnapsackSolver.Models;

namespace IPKnapsackSolver.Models

{
    // Class to represent a Linear Program
    public class LinearProgram
{
    public string Type { get; set; }
    public string ObjectiveType { get; set; } // e.g., "Maximize" or "Minimize"
    public List<double> ObjectiveCoefficients { get; set; }
    public List<Constraint> Constraints { get; set; }
    public List<string> Variables { get; set; }
    public List<string> SignRestrictions { get; set; } // e.g., "x1 >= 0", "x2 unrestricted"

    // Constructor to initialize the Linear Program
    public LinearProgram()
    {
        Type = string.Empty;
        ObjectiveType = string.Empty;
        ObjectiveCoefficients = new List<double>();
        Constraints = new List<Constraint>();
        Variables = new List<string>();
        SignRestrictions = new List<string>();
    }
}
}
    



