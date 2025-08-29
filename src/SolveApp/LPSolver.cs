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
        static (List<double>, double) ParseEquation(string eq, int varCount, int slackIndex)
        {
            // Initialize coefficients array
            List<double> coeffs = new List<double>(new double[varCount]);

            // Extract coefficients
            var tokens = Regex.Matches(eq, @"([+-]?\d*\*?x\d+)");
            foreach (Match token in tokens)
            {
                var coefMatch = Regex.Match(token.Value, @"([+-]?\d*)\*?x(\d+)");
                string coefStr = coefMatch.Groups[1].Value;
                int idx = int.Parse(coefMatch.Groups[2].Value) - 1;

                double num = coefStr switch
                {
                    "" or "+" => 1,
                    "-" => -1,
                    _ => double.Parse(coefStr)
                };
                coeffs[idx] = num;
            }

            // Extract right-hand side
            var rhsMatch = Regex.Match(eq, @"=\s*(-?\d+)");
            double rhs = double.Parse(rhsMatch.Groups[1].Value);

            // Add slack variables
            coeffs.AddRange(new double[slackIndex]);
            if (eq.Contains("<="))
                coeffs.Add(1);
            else if (eq.Contains(">="))
                coeffs.Add(-1);
            else
                coeffs.Add(0);

            return (coeffs, rhs);
        }

        static void LpToCanonical()
        {
            Console.WriteLine("Enter LP problem in format:");
            Console.WriteLine("MAX z = -3x1 - 4x2");
            Console.Write("Objective: ");
            string obj = Console.ReadLine();

            // Parse objective function
            var varCountMatch = Regex.Matches(obj, @"x(\d+)");
            int varCount = 0;
            foreach (Match m in varCountMatch)
            {
                varCount = Math.Max(varCount, int.Parse(m.Groups[1].Value));
            }

            List<double> objCoeffs = new List<double>(new double[varCount]);
            var tokens = Regex.Matches(obj, @"([+-]?\d*\*?x\d+)");
            foreach (Match token in tokens)
            {
                var coefMatch = Regex.Match(token.Value, @"([+-]?\d*)\*?x(\d+)");
                string coefStr = coefMatch.Groups[1].Value;
                int idx = int.Parse(coefMatch.Groups[2].Value) - 1;

                double num = coefStr switch
                {
                    "" or "+" => 1,
                    "-" => -1,
                    _ => double.Parse(coefStr)
                };
                objCoeffs[idx] = num;
            }

            // Constraints
            Console.Write("How many constraints? ");
            int n = int.Parse(Console.ReadLine());
            List<List<double>> constraints = new List<List<double>>();
            List<double> rhsValues = new List<double>();
            int slackIndex = 0;

            for (int i = 0; i < n; i++)
            {
                Console.Write($"Constraint {i + 1}: ");
                var (coeffs, rhs) = ParseEquation(Console.ReadLine(), varCount, slackIndex);
                constraints.Add(coeffs);
                rhsValues.Add(rhs);
                slackIndex++;
            }

            // Build tableau
            int numSlacks = n;
            int numVars = varCount + numSlacks;
            List<List<double>> tableau = new List<List<double>>();
            for (int i = 0; i < n; i++)
            {
                List<double> row = new List<double>(constraints[i]);
                row.Add(rhsValues[i]);
                tableau.Add(row);
            }

            List<double> objRow = objCoeffs.Select(c => -c).ToList();
            objRow.AddRange(Enumerable.Repeat(0.0, numSlacks));
            objRow.Add(0.0);
            tableau.Add(objRow);

            // Basis and headers
            List<string> basis = Enumerable.Range(1, n).Select(i => $"s{i}").ToList();
            List<string> headers = Enumerable.Range(1, varCount).Select(i => $"x{i}").Concat(Enumerable.Range(1, numSlacks).Select(i => $"s{i}")).ToList();

            // Perform simplex
            PerformPrimalSimplex(tableau, basis, headers);
        }

        static void PerformPrimalSimplex(List<List<double>> tableau, List<string> basis, List<string> headers)
        {
            int iteration = 0;
            int numCols = headers.Count;
            int numRows = tableau.Count; // including objective

            Console.WriteLine("\nCanonical Form Tableau:");
            DisplayTableau(tableau, basis, headers);

            while (true)
            {
                // Find entering column (most negative in objective row)
                List<double> objRow = tableau[numRows - 1];
                double minVal = 0;
                int enteringCol = -1;
                for (int j = 0; j < numCols; j++)
                {
                    if (objRow[j] < minVal)
                    {
                        minVal = objRow[j];
                        enteringCol = j;
                    }
                }

                if (enteringCol == -1)
                {
                    break; // Optimal
                }

                // Find pivot row (min positive ratio)
                double minRatio = double.PositiveInfinity;
                int pivotRow = -1;
                for (int i = 0; i < numRows - 1; i++)
                {
                    double coeff = tableau[i][enteringCol];
                    double rhs = tableau[i][numCols];
                    if (coeff > 0)
                    {
                        double ratio = rhs / coeff;
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    Console.WriteLine("Unbounded solution.");
                    return;
                }

                // Announce pivot
                Console.WriteLine($"Pivot row: {basis[pivotRow]}, Pivot column: {headers[enteringCol]}");

                // Perform pivot
                double pivotVal = tableau[pivotRow][enteringCol];
                // Normalize pivot row
                for (int j = 0; j <= numCols; j++)
                {
                    tableau[pivotRow][j] /= pivotVal;
                }

                // Eliminate in other rows (including objective)
                for (int k = 0; k < numRows; k++)
                {
                    if (k == pivotRow) continue;
                    double factor = tableau[k][enteringCol];
                    for (int j = 0; j <= numCols; j++)
                    {
                        tableau[k][j] -= factor * tableau[pivotRow][j];
                    }
                }

                // Update basis
                basis[pivotRow] = headers[enteringCol];

                // Next iteration
                iteration++;
                Console.WriteLine($"\nIteration {iteration}:");
                DisplayTableau(tableau, basis, headers);
            }

            double optimalZ = tableau[numRows - 1][numCols];
            Console.WriteLine("Optimal solution reached.");
            Console.WriteLine($"Optimal value of z: {optimalZ}");
            Console.WriteLine("x1, x2, ..., >= 0");
        }

        static void DisplayTableau(List<List<double>> tableau, List<string> basis, List<string> headers)
        {
            int numCols = headers.Count;

            // Header
            Console.Write("Basis | ");
            foreach (string h in headers)
            {
                Console.Write($"{h,4} ");
            }
            Console.WriteLine("| RHS  ");

            // Separator
            Console.Write("------+-");
            Console.Write(new string('-', numCols * 5));
            Console.WriteLine("+------");

            // Constraint rows
            for (int i = 0; i < basis.Count; i++)
            {
                Console.Write($"{basis[i],-6}| ");
                for (int j = 0; j < numCols; j++)
                {
                    Console.Write($"{Math.Round(tableau[i][j], 3),4} ");
                }
                Console.WriteLine($"| {Math.Round(tableau[i][numCols], 3),4} ");
            }

            // Z row
            Console.Write("Z     | ");
            for (int j = 0; j < numCols; j++)
            {
                Console.Write($"{Math.Round(tableau[tableau.Count - 1][j], 3),4} ");
            }
            Console.WriteLine($"| {Math.Round(tableau[tableau.Count - 1][numCols], 3),4} ");
        }

        //To trigger eveything we need to run LpToCanonical();



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