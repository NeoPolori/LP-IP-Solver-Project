using System;

namespace IPKnapsackSolver.Models
{
    public class SensitivityReport
    {
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Report: {Message}";
        }
    }
}