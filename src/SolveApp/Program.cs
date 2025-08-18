using System;

namespace SolveApp
{
    class Program
    {
       
        static void Main(string[] args)
        {
            // Infinite loop to keep showing the menu until user exits
            while (true)
            {
                // Display menu options
                Console.WriteLine("\n=== Optimization Solver Menu ===");
                Console.WriteLine("1. Solve Linear Program");
                Console.WriteLine("2. Solve Integer Program");
                Console.WriteLine("3. Run Sensitivity Analysis");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");

                // Read user input
                string choice = Console.ReadLine();

                // Route to appropriate method based on input
                switch (choice)
                {
                    case "1":
                        SolveLinearProgram(); // Call LP solver
                        break;
                    case "2":
                        SolveIntegerProgram(); // Call IP solver
                        break;
                    case "3":
                        RunSensitivityAnalysis(); // Call sensitivity analysis
                        break;
                    case "4":
                        Console.WriteLine("Exiting...");
                        return; // Exit the application
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // Method to solve a Linear Program
        static void SolveLinearProgram()
        {
            try
            {
                // Parse LP model from JSON file
                var lpModel = InputParser.Parse("input/sample_lp.json");

                // Solve the LP using LPSolver class
                var lpSolution = LPSolver.Solve(lpModel);

                // Display the solution
                Console.WriteLine("\n--- Linear Program Solution ---");
                Console.WriteLine(lpSolution.ToString());
            }
            catch (Exception ex)
            {
                // Handle any errors during parsing or solving
                Console.WriteLine($"Error solving LP: {ex.Message}");
            }
        }

        // Method to solve an Integer Program
        static void SolveIntegerProgram()
        {
            try
            {
                // Parse IP model from JSON file
                var ipModel = InputParser.Parse("input/sample_ip.json");

                // Solve the IP using IPSolver class
                var ipSolution = IPSolver.Solve(ipModel);

                // Display the solution
                Console.WriteLine("\n--- Integer Program Solution ---");
                Console.WriteLine(ipSolution.ToString());
            }
            catch (Exception ex)
            {
                // Handle any errors during parsing or solving
                Console.WriteLine($"Error solving IP: {ex.Message}");
            }
        }

        // Method to run sensitivity analysis on an LP solution
        static void RunSensitivityAnalysis()
        {
            try
            {
                // Parse LP model from JSON file
                var model = InputParser.Parse("input/sample_lp.json");

                // Solve the LP to get a solution
                var solution = LPSolver.Solve(model);

                // Run sensitivity analysis on the solution
                var analysis = SensitivityAnalyzer.Analyze(solution);

                // Display the analysis results
                Console.WriteLine("\n--- Sensitivity Analysis ---");
                Console.WriteLine(analysis.ToString());
            }
            catch (Exception ex)
            {
                // Handle any errors during parsing, solving, or analysis
                Console.WriteLine($"Error running analysis: {ex.Message}");
            }
        }
    }
}