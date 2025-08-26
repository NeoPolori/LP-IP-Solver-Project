using System;

public class SensitivityAnalysis
{

    public static List<string> PerformSensitivityAnalysis(BranchAndBoundKnapsack solver, List<KnapsackItem> items, KnapsackSolution solution, int capacity)
    {
        var results = new List<string>();

        results.Add("Sensitivity Analysis for Non-Basic Variables:");
        for (int i = 0; i < items.Count; i++)
        {
            if (solution.SelectedItems[i] == 0)
            {
                results.Add($"Variable x{i + 1} is non-basic.");
                DisplayRangeAndApplyChange(results, solver, items, i, solution, capacity);
            }
        }

        results.Add("Sensitivity Analysis for Basic Variables:");
        for (int i = 0; i < items.Count; i++)
        {
            if (solution.SelectedItems[i] == 1)
            {
                results.Add($"Variable x{i + 1} is basic.");
                DisplayRangeAndApplyChange(results, solver, items, i, solution, capacity);
            }
        }

        results.Add("Sensitivity Analysis for Constraint RHS Values:");
        for (int j = 0; j < items.Count; j++)
        {
            int originalWeight = items[j].Weight;
            items[j].Weight += 1;
            var newSolution = solver.Solve();
            results.Add($"After increasing weight of item {j + 1} by 1, new Objective Value = {newSolution.TotalValue}");
            items[j].Weight = originalWeight;
        }

        results.Add("Adding a new activity to the model:");
        items.Add(new KnapsackItem { Weight = 1, Value = 1 });
        var solutionWithNewActivity = solver.Solve();
        results.Add($"Objective Value with new activity = {solutionWithNewActivity.TotalValue}");

        results.Add("Adding a new constraint to the model:");
        items.Last().Weight += 1;  // Example constraint adjustment
        var solutionWithNewConstraint = solver.Solve();
        results.Add($"Objective Value with new constraint = {solutionWithNewConstraint.TotalValue}");

        results.Add("Shadow Prices:");
        foreach (var item in items)
        {
            results.Add($"Shadow price for item {items.IndexOf(item) + 1}: {CalculateShadowPrice(item, solution.TotalValue)}");
        }

        results.Add("Applying Duality:");
        var dualSolution = ApplyDuality(items, out int dualValue);
        results.Add($"Dual Objective Value: {dualValue}");
        results.Add(dualValue == solution.TotalValue ? "Strong Duality" : "Weak Duality");

        return results;
    }

    public static void DisplayRangeAndApplyChange(List<string> results, BranchAndBoundKnapsack solver, List<KnapsackItem> items, int variableIndex, KnapsackSolution solution, int capacity)
    {
        int originalValue = items[variableIndex].Value;
        items[variableIndex].Value += 1;
        var newSolution = solver.Solve();
        results.Add($"After increasing value of item {variableIndex + 1} by 1, new Objective Value = {newSolution.TotalValue}");
        items[variableIndex].Value = originalValue;
    }

    public static int CalculateShadowPrice(KnapsackItem item, int bestValue)
    {
        return item.Weight * bestValue;
    }

    public static List<int> ApplyDuality(List<KnapsackItem> items, out int dualValue)
    {
        dualValue = 0; // Implement dual problem logic here.
        return new List<int>(new int[items.Count]);
    }


    static void RunSensitivityAnalysis()
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
    static void WriteSensitivityAnalysisResults(string outputFilePath, List<string> sensitivityAnalysisResults)
    {
        using (var file = new StreamWriter(outputFilePath, true))
        {
            file.WriteLine("\nSensitivity Analysis Results:");
            foreach (var result in sensitivityAnalysisResults)
            {
                file.WriteLine(result);
            }
        }
    }

    static (int, List<KnapsackItem>, string) ReadInputFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length < 2)
        {
            throw new InvalidOperationException("The input file must contain at least two lines.");
        }

        // Extract objective type and item values from the first line
        string[] firstLine = lines[0].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string objectiveType = firstLine[0].ToLower();

        List<KnapsackItem> items = new List<KnapsackItem>();
        for (int i = 1; i < firstLine.Length; i++)
        {
            int value = int.Parse(firstLine[i].TrimStart('+'));
            items.Add(new KnapsackItem { Value = value });
        }

        // Extract weights and capacity from the second line
        string[] constraintLine = lines[1].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < items.Count; i++)
        {
            int weight = int.Parse(constraintLine[i].TrimStart('+'));
            items[i].Weight = weight;
        }

        // Parse capacity from the constraint line
        string capacityStr = constraintLine.Last().Split('=').Last();
        if (!int.TryParse(capacityStr, out int capacity))
        {
            throw new InvalidOperationException("Invalid capacity value in the constraint line.");
        }

        return (capacity, items, objectiveType);
    }



}


















