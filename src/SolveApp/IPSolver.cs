using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;



namespace SolveApp
{
    // Class to parse input from a JSON file
    public static class InputParser
    {
        // Method to parse input from a JSON file
        public static LinearProgram Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            string json = File.ReadAllText(filePath);
            var model = JsonSerializer.Deserialize<LinearProgram>(json);

            if (model == null)
                throw new Exception("Failed to parse input file.");

            return model;
        }
    }

    
}
