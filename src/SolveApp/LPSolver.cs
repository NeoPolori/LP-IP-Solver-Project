using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IPKnapsackSolver.Models;

namespace SolveApp
{
    public static class LPSolver
    {
        /// <summary>
        /// Normalizes a linear expression so both "3x1" and "3*x1" work.
        /// Returns a dictionary of variable -> coefficient.
        /// </summary>
        public static Dictionary<string, double> ParseExpression(string input)
        {
            var variables = new Dictionary<string, double>();

            // Ensure * is optional, convert "3x1" -> "3*x1"
            string normalized = Regex.Replace(input, @"(\d)(x\d+)", "$1*$2");

            // Pattern to capture coefficient * variable
            string pattern = @"([+-]?\s*\d*\.?\d*)\s*\*\s*(x\d+)";
            foreach (Match match in Regex.Matches(normalized, pattern))
            {
                string coefStr = match.Groups[1].Value.Replace(" ", "");
                string variable = match.Groups[2].Value;

                double coef = string.IsNullOrEmpty(coefStr) || coefStr == "+" ? 1 :
                              coefStr == "-" ? -1 :
                              double.Parse(coefStr);

                if (variables.ContainsKey(variable))
                    variables[variable] += coef;
                else
                    variables[variable] = coef;
            }

            return variables;
        }

        /// <summary>
        /// Converts a LinearProgram model into canonical (conical) form string.
        /// </summary>
        public static string ToConicalForm(LinearProgram model)
        {
            var sb = new StringBuilder();

            // Objective
            sb.AppendLine($"{model.Type.ToUpper()} Z = " +
                string.Join(" + ", model.ObjectiveCoefficients
                    .Select((c, i) => $"{c}*{model.Variables[i]}")));

            sb.AppendLine("Subject to:");

            int slackCount = 1;

            foreach (var constraint in model.Constraints)
            {
                var lhs = string.Join(" + ",
                    constraint.Coefficients.Select((c, i) => $"{c}*{model.Variables[i]}"));

                string row = lhs;

                if (constraint.Operator == "<=")
                {
                    row += $" + s{slackCount} = {constraint.RHS}";
                    slackCount++;
                }
                else if (constraint.Operator == ">=")
                {
                    row += $" - s{slackCount} = {constraint.RHS}";
                    slackCount++;
                }
                else // "="
                {
                    row += $" = {constraint.RHS}";
                }

                sb.AppendLine(row);
            }

            // Add non-negativity conditions
            sb.AppendLine("All variables >= 0");
            if (slackCount > 1)
            {
                sb.AppendLine($"Slack variables s1..s{slackCount - 1} >= 0");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Dummy solver (replace later with simplex).
        /// </summary>
        public static Solution Solve(LinearProgram model)
        {
            // For now just return dummy result
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