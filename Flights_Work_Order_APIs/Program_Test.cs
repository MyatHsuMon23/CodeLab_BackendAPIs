using System;
using Flights_Work_Order_APIs.Services;

class Program
{
    static void Main(string[] args)
    {
        var commandService = new WorkOrderCommandService();

        // Test cases from the problem statement
        var testCommands = new[]
        {
            "CHK15|BAG25|CLEAN10|PBB90",
            "bag5|chk10|clean0",
            "CHK10||BAG5",
            "PBB180",
            "CLEAN5|PBB270|CHK20",
            "INVALID",
            "CHK15|PBB45", // Invalid angle
            "CHK15|CHK20", // Duplicate
            "",
            "CHK15|BAG25|CLEAN10|PBB0|PBB90" // Multiple PBB
        };

        Console.WriteLine("Testing Work Order Command Parsing");
        Console.WriteLine("==================================");

        foreach (var command in testCommands)
        {
            Console.WriteLine($"\nCommand: '{command}'");
            var result = commandService.ParseCommand(command);
            
            Console.WriteLine($"Valid: {result.IsValid}");
            if (result.CheckInMinutes.HasValue) Console.WriteLine($"Check-in: {result.CheckInMinutes} minutes");
            if (result.BaggageMinutes.HasValue) Console.WriteLine($"Baggage: {result.BaggageMinutes} minutes");
            if (result.CleaningMinutes.HasValue) Console.WriteLine($"Cleaning: {result.CleaningMinutes} minutes");
            if (result.JetBridgeAngle.HasValue) Console.WriteLine($"Jet Bridge: {result.JetBridgeAngle} degrees");
            
            if (!result.IsValid)
            {
                Console.WriteLine($"Errors: {string.Join(", ", result.ValidationErrors)}");
            }
            Console.WriteLine(new string('-', 40));
        }
    }
}