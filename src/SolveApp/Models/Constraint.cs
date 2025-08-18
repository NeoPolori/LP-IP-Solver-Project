namespace SolveApp.Models
{
    // Class to represent a constraint
    public class Constraint
    {
        public double[] Coefficients { get; set; }
        public string Relation { get; set; } // "<=", ">=", "="
        public double RHS { get; set; }
    }
}