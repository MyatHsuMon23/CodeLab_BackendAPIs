using Flights_Work_Order_APIs.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Flights_Work_Order_APIs.Services
{
    /// <summary>
    /// Service for parsing and validating flight commands
    /// </summary>
    public interface IFlightCommandService
    {
        List<FlightCommand> ParseCommands(string commandString);
        string ConvertToHumanReadable(List<FlightCommand> commands);
        bool ValidateCommands(List<FlightCommand> commands);
    }

    public class FlightCommandService : IFlightCommandService
    {
        private readonly ILogger<FlightCommandService> _logger;

        public FlightCommandService(ILogger<FlightCommandService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Parse command string into individual commands
        /// </summary>
        public List<FlightCommand> ParseCommands(string commandString)
        {
            var commands = new List<FlightCommand>();

            if (string.IsNullOrWhiteSpace(commandString))
            {
                return commands;
            }

            // Split by pipe separator
            var commandParts = commandString.Split('|', StringSplitOptions.None);

            foreach (var part in commandParts)
            {
                var trimmedPart = part.Trim();
                
                // Skip empty parts
                if (string.IsNullOrEmpty(trimmedPart))
                {
                    continue;
                }

                var command = ParseSingleCommand(trimmedPart);
                commands.Add(command);
            }

            return commands;
        }

        /// <summary>
        /// Parse a single command part
        /// </summary>
        private FlightCommand ParseSingleCommand(string commandPart)
        {
            var command = new FlightCommand
            {
                IsValid = true
            };

            try
            {
                // Check-in command: CHKn (n is integer)
                var chkMatch = Regex.Match(commandPart, @"^CHK(\d+)$", RegexOptions.IgnoreCase);
                if (chkMatch.Success)
                {
                    command.Type = "CHK";
                    command.Value = chkMatch.Groups[1].Value;
                    command.DisplayText = $"Check-in: {command.Value} minutes";
                    return command;
                }

                // Baggage command: BAGn (n is integer)
                var bagMatch = Regex.Match(commandPart, @"^BAG(\d+)$", RegexOptions.IgnoreCase);
                if (bagMatch.Success)
                {
                    command.Type = "BAG";
                    command.Value = bagMatch.Groups[1].Value;
                    command.DisplayText = $"Baggage: {command.Value} minutes";
                    return command;
                }

                // Cleaning command: CLEANn (n is integer)
                var cleanMatch = Regex.Match(commandPart, @"^CLEAN(\d+)$", RegexOptions.IgnoreCase);
                if (cleanMatch.Success)
                {
                    command.Type = "CLEAN";
                    command.Value = cleanMatch.Groups[1].Value;
                    command.DisplayText = $"Cleaning: {command.Value} minutes";
                    return command;
                }

                // Jet-bridge command: PBBx (x must be 0, 90, 180, 270)
                var pbbMatch = Regex.Match(commandPart, @"^PBB(\d+)$", RegexOptions.IgnoreCase);
                if (pbbMatch.Success)
                {
                    var angle = pbbMatch.Groups[1].Value;
                    var validAngles = new[] { "0", "90", "180", "270" };
                    
                    command.Type = "PBB";
                    command.Value = angle;
                    
                    if (validAngles.Contains(angle))
                    {
                        command.DisplayText = $"Jet-bridge angle: {angle}°";
                    }
                    else
                    {
                        command.IsValid = false;
                        command.ErrorMessage = $"Invalid jet-bridge angle: {angle}. Must be 0, 90, 180, or 270.";
                        command.DisplayText = $"Invalid jet-bridge angle: {angle}";
                    }
                    return command;
                }

                // Handle partial commands (like CHK without number)
                if (Regex.IsMatch(commandPart, @"^CHK$", RegexOptions.IgnoreCase))
                {
                    command.Type = "CHK";
                    command.Value = "";
                    command.IsValid = false;
                    command.ErrorMessage = "Check-in command missing minutes value.";
                    command.DisplayText = "Check-in: (missing value)";
                    return command;
                }

                if (Regex.IsMatch(commandPart, @"^BAG$", RegexOptions.IgnoreCase))
                {
                    command.Type = "BAG";
                    command.Value = "";
                    command.IsValid = false;
                    command.ErrorMessage = "Baggage command missing minutes value.";
                    command.DisplayText = "Baggage: (missing value)";
                    return command;
                }

                if (Regex.IsMatch(commandPart, @"^CLEAN$", RegexOptions.IgnoreCase))
                {
                    command.Type = "CLEAN";
                    command.Value = "";
                    command.IsValid = false;
                    command.ErrorMessage = "Cleaning command missing minutes value.";
                    command.DisplayText = "Cleaning: (missing value)";
                    return command;
                }

                if (Regex.IsMatch(commandPart, @"^PBB$", RegexOptions.IgnoreCase))
                {
                    command.Type = "PBB";
                    command.Value = "";
                    command.IsValid = false;
                    command.ErrorMessage = "Jet-bridge command missing angle value.";
                    command.DisplayText = "Jet-bridge: (missing value)";
                    return command;
                }

                // Unknown command
                command.Type = "UNKNOWN";
                command.Value = commandPart;
                command.IsValid = false;
                command.ErrorMessage = $"Unknown command: {commandPart}";
                command.DisplayText = $"Unknown command: {commandPart}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing command: {CommandPart}", commandPart);
                command.Type = "ERROR";
                command.Value = commandPart;
                command.IsValid = false;
                command.ErrorMessage = $"Error parsing command: {ex.Message}";
                command.DisplayText = $"Error: {commandPart}";
            }

            return command;
        }

        /// <summary>
        /// Convert parsed commands to human-readable format
        /// </summary>
        public string ConvertToHumanReadable(List<FlightCommand> commands)
        {
            if (!commands.Any())
            {
                return "No commands specified.";
            }

            var readableCommands = commands
                .Select(c => c.DisplayText)
                .Where(text => !string.IsNullOrEmpty(text));

            return string.Join("\n", readableCommands);
        }

        /// <summary>
        /// Validate all commands in the list
        /// </summary>
        public bool ValidateCommands(List<FlightCommand> commands)
        {
            return commands.All(c => c.IsValid);
        }
    }
}