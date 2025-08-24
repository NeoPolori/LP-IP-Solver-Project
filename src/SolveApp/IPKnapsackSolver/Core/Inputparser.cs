using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IPKnapsackSolver.Models;

namespace Core
{
    public static class InputParser
    {
        public static LinearProgram ParseInputFile(string path)
        {
            var lines = File.ReadAllLines(path);
            var lp = new LinearProgram
            {
                ObjectiveCoefficients = new List<double>(),
                Constraints = new List<Constraint>(),
                SignRestrictions = new List<string>()
            };

            // Parse objective line
            var objTokens = lines[0].Split(' ');
            lp.ObjectiveType = objTokens[0];
            for (int i = 1; i < objTokens.Length; i += 2)
            {
                double coeff = double.Parse(objTokens[i + 1]);
                lp.ObjectiveCoefficients.Add(objTokens[i] == "-" ? -coeff : coeff);
            }

            // Parse constraints
            for (int i = 1; i < lines.Length - 1; i++)
            {
                var tokens = lines[i].Split(' ');
                var constraint = new Constraint
                {
                    Coefficients = new List<double>(),
                    Relation = tokens[tokens.Length - 2],
                    RHS = double.Parse(tokens[tokens.Length - 1])
                };

                for (int j = 0; j < lp.ObjectiveCoefficients.Count; j++)
                {
                    double coeff = double.Parse(tokens[2 * j + 1]);
                    constraint.Coefficients.Add(tokens[2 * j] == "-" ? -coeff : coeff);
                }

                lp.Constraints.Add(constraint);
            }

            // Parse sign restrictions
            lp.SignRestrictions = lines[^1].Split(' ').ToList();

            return lp;
        }
    }
}