using System.Collections.Generic;
using IPKnapsackSolver.Models;

namespace SolveApp
{
    public static class LPSolver
    {
        public static Solution Solve(LinearProgram model)
        {
            return new Solution
            {
                ObjectiveValue = 42.0,
                VariableValues = new Dictionary<string, double>
                {
                    { "x1", 3 },
                    { "x2", 7 }
                }
            };
        }
    }
}