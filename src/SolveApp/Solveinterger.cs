using SolveApp;
using System;

public class SolveInterger
{
	public SolveInterger()
	{
        static void SolveKnapsack()
        {
            Console.WriteLine("Enter the input file path:");
            string inputFilePath = Console.ReadLine();
            Console.WriteLine("Enter the output file path:");
            string outputFilePath = Console.ReadLine();

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found.");
                return;
            }

            try
            {
                var (capacity, items, objectiveType) = ReadInputFile(inputFilePath);

                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    BranchAndBoundKnapsack solver = new BranchAndBoundKnapsack(capacity, items, writer, objectiveType);
                    KnapsackSolution solution = solver.Solve();

                    writer.WriteLine();
                    writer.WriteLine("Best Solution:");
                    writer.WriteLine($"Total Value: {solution.TotalValue}");
                    writer.WriteLine($"Total Weight: {solution.TotalWeight}");
                    writer.WriteLine($"Selected Items: {string.Join(", ", solution.SelectedItems.Select((s, i) => s == 1 ? $"Item {i + 1}" : string.Empty).Where(x => !string.IsNullOrEmpty(x)))}");

                    Console.WriteLine("Solution has been written to the output file.");
                    Console.WriteLine($"Total Value: {solution.TotalValue}");
                    Console.WriteLine($"Total Weight: {solution.TotalWeight}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }










    }
}
