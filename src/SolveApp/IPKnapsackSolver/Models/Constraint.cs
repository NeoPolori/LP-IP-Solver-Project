namespace IPKnapsackSolver.Models

{
    // Class to represent a constraint
    public class Constraint
{
    public List<double> Coefficients { get; set; }
    public string Relation { get; set; } // e.g., "<=", ">=", "="
    public double RHS { get; set; }      // Right-hand side value

    public Constraint()
    {
        Coefficients = new List<double>();
        Relation = string.Empty;
        RHS = 0.0;
    }
}
}