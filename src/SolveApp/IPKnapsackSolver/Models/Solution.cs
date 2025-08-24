using System;
using System.Collections.Generic;
using System.Text;

namespace IPKnapsackSolver.Models
{    // Class to represent a solution
    public class Solution
    {
        public double ObjectiveValue { get; set; }
        public Dictionary<string, double> VariableValues { get; set; }
        // Constructor to initialize the Solution
        public Solution()
        {
            ObjectiveValue = 0.0;
            VariableValues = new Dictionary<string, double>();
        }
        // Method to convert the solution to a string
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Objective Value: {ObjectiveValue}");
            foreach (var kvp in VariableValues)
            {
                builder.AppendLine($"{kvp.Key} = {kvp.Value}");
            }
            return builder.ToString();
        }
    }
}