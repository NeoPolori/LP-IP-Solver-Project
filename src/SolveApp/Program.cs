using System;
using System.IO;
using IPKnapsackSolver.Models;

namespace SolveApp
{
    class Program
    {
        const string SampleLPJson = @"{
  ""Type"": ""max"",
  ""ObjectiveCoefficients"": [5, 4],
  ""Constraints"": [
    { ""Coefficients"": [6, 4], ""Operator"": ""<="", ""RHS"": 24 },
    { ""Coefficients"": [1, 2], ""Operator"": ""<="", ""RHS"": 6 }
  ],
  ""Variables"": [""x1"", ""x2""]
}";

const string SampleIPJson = @"{
  ""Type"": ""max"",
  ""ObjectiveCoefficients"": [3, 2],
  ""Constraints"": [
    { ""Coefficients"": [2, 1], ""Operator"": ""<="", ""RHS"": 10 },
    { ""Coefficients"": [1, 1], ""Operator"": ""<="", ""RHS"": 6 }
  ],
  ""Variables"": [""x1"", ""x2""]
}";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n=== Optimization Solver Menu ===");
                Console.WriteLine("1. Solve Linear Program");
                Console.WriteLine("2. Solve Integer Program");
                Console.WriteLine("3. Run Sensitivity Analysis");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SolveLinearProgram();
                        break;
                    case "2":
                        SolveIntegerProgram();
                        break;
                    case "3":
                        RunSensitivityAnalysis();
                        break;
                    case "4":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }


        // Centralized input path resolver
        static string? ResolveInputPath(string filename)
        {
            string? baseDir = AppContext.BaseDirectory;
            string? projectRoot = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName;

            if (projectRoot == null)
                return null;

            string fullPath = Path.Combine(projectRoot, "input", filename);
            return File.Exists(fullPath) ? fullPath : null;
        }
        // Auto-generate sample files
        static void EnsureSampleFileExists(string filename, string content)
{
    string? projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
    if (projectRoot == null) return;

    string inputDir = Path.Combine(projectRoot, "input");
    string fullPath = Path.Combine(inputDir, filename);

    if (!Directory.Exists(inputDir))
        Directory.CreateDirectory(inputDir);

    if (!File.Exists(fullPath))
    {
        File.WriteAllText(fullPath, content);
        Console.WriteLine($"Auto-generated missing file: {fullPath}");
    }
}

       // Solve Linear Program
        static void SolveLinearProgram()
        {  
            try
            {
                EnsureSampleFileExists("sample_lp.json", SampleLPJson);
                string? inputPath = ResolveInputPath("sample_lp.json");
                if (inputPath == null)
                {
                    Console.WriteLine("Input file not found.");
                    return;
                }

                var lpModel = IPSolver.Parse(inputPath);
                if (lpModel == null)
                {
                    Console.WriteLine("Failed to parse LP model.");
                    return;
                }

                var lpSolution = LPSolver.Solve(lpModel);
                Console.WriteLine("\n--- Linear Program Solution ---");
                Console.WriteLine(lpSolution.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving LP: {ex.Message}");
            }
        }

        static void SolveIntegerProgram()
        {

            try
            {
                EnsureSampleFileExists("sample_ip.json", SampleIPJson);
                string? inputPath = ResolveInputPath("sample_lp.json"); // or "sample_ip.json"
                if (inputPath == null)
                {
                    Console.WriteLine("Input file not found.");
                    return;
                }

                var ipModel = IPSolver.Parse(inputPath);
                if (ipModel == null)
                {
                    Console.WriteLine("Failed to parse IP model.");
                    return;
                }

                var ipSolution = IPSolver.Solve(ipModel);
                Console.WriteLine("\n--- Integer Program Solution ---");
                Console.WriteLine(ipSolution.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error solving IP: {ex.Message}");
            }
            
            
        }

        static void RunSensitivityAnalysis()
        {
            try
            {
                EnsureSampleFileExists("sample_lp.json", SampleLPJson);
                string? inputPath = ResolveInputPath("sample_lp.json");
                if (inputPath == null)
                {
                    Console.WriteLine("Input file not found for Sensitivity Analysis.");
                    return;
                }

                var model = IPSolver.Parse(inputPath);
                if (model == null)
                {
                    Console.WriteLine("Failed to parse LP model for sensitivity analysis.");
                    return;
                }

                var solution = LPSolver.Solve(model);
                var analysis = SensitivityAnalyzer.Analyze(solution);
                Console.WriteLine("\n--- Sensitivity Analysis ---");
                Console.WriteLine(analysis.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running analysis: {ex.Message}");
            }
        }
    }
}