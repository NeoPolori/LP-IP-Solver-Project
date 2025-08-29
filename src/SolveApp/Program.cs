using System;
using System.IO;
using IPKnapsackSolver.Models;
using SolveApp;

namespace SolveApp
{
    class KnapsackItem
    {
        public int Weight { get; set; }
        public int Value { get; set; }
        public double ValuePerWeight => Math.Round((double)Value / Weight, 3);
    }

    class KnapsackSolution
    {
        public int TotalValue { get; set; }
        public int TotalWeight { get; set; }
        public List<int> SelectedItems { get; set; }

        public KnapsackSolution()
        {
            SelectedItems = new List<int>();
        }
    }

    class BranchAndBoundKnapsack
    {
        private int Capacity;
        private List<KnapsackItem> Items;
        private KnapsackSolution BestSolution;
        private StreamWriter Writer;
        private string ObjectiveType;

        public BranchAndBoundKnapsack(int capacity, List<KnapsackItem> items, StreamWriter writer, string objectiveType)
        {
            Capacity = capacity;
            Items = items.OrderByDescending(i => i.ValuePerWeight).ToList();
            BestSolution = new KnapsackSolution();
            Writer = writer;
            ObjectiveType = objectiveType.ToLower();
        }

        public KnapsackSolution Solve()
        {
            BranchAndBound(0, 0, 0, new List<int>(new int[Items.Count]));
            return BestSolution;
        }

        private void BranchAndBound(int index, int currentWeight, int currentValue, List<int> selectedItems)
        {
            if (index == Items.Count)
            {
                if ((ObjectiveType == "max" && currentWeight <= Capacity && currentValue > BestSolution.TotalValue) ||
                    (ObjectiveType == "min" && currentWeight <= Capacity && (BestSolution.TotalValue == 0 || currentValue < BestSolution.TotalValue)))
                {
                    BestSolution.TotalValue = currentValue;
                    BestSolution.TotalWeight = currentWeight;
                    BestSolution.SelectedItems = new List<int>(selectedItems);
                }
                return;
            }

            double bound = CalculateBound(index, currentWeight, currentValue);
            Writer.WriteLine($"Index: {index}, Current Weight: {currentWeight}, Current Value: {currentValue}, Bound: {Math.Round(bound, 3)}, Selected Items: {string.Join(", ", selectedItems)}");

            if ((ObjectiveType == "max" && bound <= BestSolution.TotalValue) ||
                (ObjectiveType == "min" && bound >= BestSolution.TotalValue && BestSolution.TotalValue != 0))
            {
                return;
            }

            if (currentWeight + Items[index].Weight <= Capacity)
            {
                selectedItems[index] = 1;
                BranchAndBound(index + 1, currentWeight + Items[index].Weight, currentValue + Items[index].Value, selectedItems);
            }

            selectedItems[index] = 0;
            BranchAndBound(index + 1, currentWeight, currentValue, selectedItems);
        }

        private double CalculateBound(int index, int currentWeight, int currentValue)
        {
            double bound = currentValue;
            int totalWeight = currentWeight;

            for (int i = index; i < Items.Count; i++)
            {
                if (totalWeight + Items[i].Weight <= Capacity)
                {
                    totalWeight += Items[i].Weight;
                    bound += Items[i].Value;
                }
                else
                {
                    int remainingWeight = Capacity - totalWeight;
                    bound += Items[i].ValuePerWeight * remainingWeight;
                    break;
                }
            }
            return bound;
        }
    }



}




class Program
    {

        

    static void Main(string[] args)
        {

        //menu 
            bool exit = false;
            while (!exit)
            {
            Console.WriteLine("1. Solve Knapsack Problem");
            Console.WriteLine("2. Perform Sensitivity Analysis");
            Console.WriteLine("3.Solve Linear");
            Console.WriteLine("4. Exit");
            Console.Write("Please choose an option: ");
            string option = Console.ReadLine();

            switch (choice)
                {
                    case "1":
                    SolveKnapsack();
                    break;
                    case "2":
                    PerformSensitivityAnalysisMenu();
                    break;
                    case "3":
                    SolveLinearProgram();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }


      

       // Solve Linear Program
        static void SolveLinearProgram()
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

    static void PerformSensitivityAnalysisMenu()
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

            using (StreamWriter writer = new StreamWriter(outputFilePath, true))
            {
                BranchAndBoundKnapsack solver = new BranchAndBoundKnapsack(capacity, items, writer, objectiveType);
                KnapsackSolution solution = solver.Solve();

                writer.WriteLine();
                writer.WriteLine("Sensitivity Analysis:");
                var sensitivityResults = new List<string>();

                while (true)
                {
                    Console.WriteLine("Choose an option for Sensitivity Analysis:");
                    Console.WriteLine("1. Display the range of a selected Non-Basic Variable");
                    Console.WriteLine("2. Apply and display a change of a selected Non-Basic Variable");
                    Console.WriteLine("3. Display the range of a selected Basic Variable");
                    Console.WriteLine("4. Apply and display a change of a selected Basic Variable");
                    Console.WriteLine("5. Display the range of a selected constraint RHS value");
                    Console.WriteLine("6. Apply and display a change of a selected constraint RHS value");
                    Console.WriteLine("7. Add a new activity to the model");
                    Console.WriteLine("8. Add a new constraint to the model");
                    Console.WriteLine("9. Display the shadow prices");
                    Console.WriteLine("10. Apply Duality to the programming model");
                    Console.WriteLine("11. Solve the Dual Programming Model");
                    Console.WriteLine("12. Exit Sensitivity Analysis");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Enter the index of the Non-Basic Variable:");
                            int nonBasicIndex = int.Parse(Console.ReadLine()) - 1;
                            sensitivityResults.Add($"Displaying range for Non-Basic Variable x{nonBasicIndex + 1}");
                            SensitivityAnalysis.DisplayRangeAndApplyChange(sensitivityResults, solver, items, nonBasicIndex, solution, capacity);
                            break;
                        case "2":
                            Console.WriteLine("Enter the index of the Non-Basic Variable:");
                            int nonBasicChangeIndex = int.Parse(Console.ReadLine()) - 1;
                            sensitivityResults.Add($"Applying and displaying change for Non-Basic Variable x{nonBasicChangeIndex + 1}");
                            SensitivityAnalysis.DisplayRangeAndApplyChange(sensitivityResults, solver, items, nonBasicChangeIndex, solution, capacity);
                            break;
                        case "3":
                            Console.WriteLine("Enter the index of the Basic Variable:");
                            int basicIndex = int.Parse(Console.ReadLine()) - 1;
                            sensitivityResults.Add($"Displaying range for Basic Variable x{basicIndex + 1}");
                            SensitivityAnalysis.DisplayRangeAndApplyChange(sensitivityResults, solver, items, basicIndex, solution, capacity);
                            break;
                        case "4":
                            Console.WriteLine("Enter the index of the Basic Variable:");
                            int basicChangeIndex = int.Parse(Console.ReadLine()) - 1;
                            sensitivityResults.Add($"Applying and displaying change for Basic Variable x{basicChangeIndex + 1}");
                            SensitivityAnalysis.DisplayRangeAndApplyChange(sensitivityResults, solver, items, basicChangeIndex, solution, capacity);
                            break;
                        case "5":
                            sensitivityResults.Add("Displaying range for constraint RHS values");
                            foreach (var item in items)
                            {
                                sensitivityResults.Add($"Item {items.IndexOf(item) + 1} weight: {item.Weight}");
                            }
                            break;
                        case "6":
                            Console.WriteLine("Enter the index of the constraint RHS value:");
                            int constraintIndex = int.Parse(Console.ReadLine()) - 1;
                            sensitivityResults.Add($"Applying and displaying change for constraint RHS value {constraintIndex + 1}");
                            int originalWeight = items[constraintIndex].Weight;
                            items[constraintIndex].Weight += 1;
                            var newSolution = solver.Solve();
                            sensitivityResults.Add($"After increasing weight of item {constraintIndex + 1} by 1, new Objective Value = {newSolution.TotalValue}");
                            items[constraintIndex].Weight = originalWeight;
                            break;
                        case "7":
                            sensitivityResults.Add("Adding a new activity to the model");
                            items.Add(new KnapsackItem { Weight = 1, Value = 1 });
                            var solutionWithNewActivity = solver.Solve();
                            sensitivityResults.Add($"Objective Value with new activity = {solutionWithNewActivity.TotalValue}");
                            break;
                        case "8":
                            sensitivityResults.Add("Adding a new constraint to the model");
                            items.Last().Weight += 1;  // Example constraint adjustment
                            var solutionWithNewConstraint = solver.Solve();
                            sensitivityResults.Add($"Objective Value with new constraint = {solutionWithNewConstraint.TotalValue}");
                            break;
                        case "9":
                            sensitivityResults.Add("Displaying shadow prices");
                            foreach (var item in items)
                            {
                                sensitivityResults.Add($"Shadow price for item {items.IndexOf(item) + 1}: {SensitivityAnalysis.CalculateShadowPrice(item, solution.TotalValue)}");
                            }
                            break;
                        case "10":
                            sensitivityResults.Add("Applying Duality to the programming model");
                            var dualSolution = SensitivityAnalysis.ApplyDuality(items, out int dualValue);
                            sensitivityResults.Add($"Dual Objective Value: {dualValue}");
                            sensitivityResults.Add(dualValue == solution.TotalValue ? "Strong Duality" : "Weak Duality");
                            break;
                        case "11":
                            sensitivityResults.Add("Solving the Dual Programming Model");
                            // Implement dual programming model solution if needed
                            break;
                        case "12":
                            WriteSensitivityAnalysisResults(outputFilePath, sensitivityResults);
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please choose again.");
                            break;
                    }

                    Console.WriteLine("Sensitivity Analysis results:");
                    foreach (var result in sensitivityResults)
                    {
                        Console.WriteLine(result);
                    }

                    Console.WriteLine("\nDo you want to continue with Sensitivity Analysis? (yes/no)");
                    string continueAnalysis = Console.ReadLine().ToLower();
                    if (continueAnalysis != "yes")
                    {
                        WriteSensitivityAnalysisResults(outputFilePath, sensitivityResults);
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

}