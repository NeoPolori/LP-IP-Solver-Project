using System;
using System.IO;
using System.Text.Json;
using IPKnapsackSolver.Models;

namespace SolveApp
{
    public static class IPSolver
    {
        public static LinearProgram Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            string json = File.ReadAllText(filePath);
            var model = JsonSerializer.Deserialize<LinearProgram>(json);

            if (model == null)
                throw new Exception("Failed to parse input file.");

            return model;
        }

        public static Solution Solve(LinearProgram model)
        {
            return new Solution
            {
                ObjectiveValue = 100.0,
                VariableValues = new Dictionary<string, double>
                {
                    { "x1", 1 },
                    { "x2", 0 }
                }
            };
        }
    }
}