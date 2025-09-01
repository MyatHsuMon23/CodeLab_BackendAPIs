using Flights_Work_Order_APIs.DTOs;
using System.Text.RegularExpressions;

namespace Flights_Work_Order_APIs.Services
{
    public interface IWorkOrderCommandService
    {
        ParsedWorkOrderCommandDto ParseCommand(string commandString);
    }

    public class WorkOrderCommandService : IWorkOrderCommandService
    {
        private static readonly int[] ValidJetBridgeAngles = { 0, 90, 180, 270 };

        public ParsedWorkOrderCommandDto ParseCommand(string commandString)
        {
            var result = new ParsedWorkOrderCommandDto
            {
                OriginalCommand = commandString
            };

            if (string.IsNullOrWhiteSpace(commandString))
            {
                result.ValidationErrors.Add("Command string cannot be empty");
                return result;
            }

            // Split by pipe and process each command
            var commands = commandString.Split('|', StringSplitOptions.None);

            foreach (var command in commands)
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    continue; // Skip empty commands (allows for CHK10||BAG5 format)
                }

                if (!ProcessCommand(command.Trim(), result))
                {
                    result.ValidationErrors.Add($"Invalid command format: '{command.Trim()}'");
                }
            }

            return result;
        }

        private bool ProcessCommand(string command, ParsedWorkOrderCommandDto result)
        {
            // Convert to uppercase for case-insensitive matching
            var upperCommand = command.ToUpper();

            // Check-in pattern: CHKn
            var chkMatch = Regex.Match(upperCommand, @"^CHK(\d+)$");
            if (chkMatch.Success)
            {
                if (int.TryParse(chkMatch.Groups[1].Value, out int minutes))
                {
                    if (result.CheckInMinutes.HasValue)
                    {
                        result.ValidationErrors.Add("Check-in command specified multiple times");
                    }
                    else
                    {
                        result.CheckInMinutes = minutes;
                    }
                    return true;
                }
            }

            // Baggage pattern: BAGn
            var bagMatch = Regex.Match(upperCommand, @"^BAG(\d+)$");
            if (bagMatch.Success)
            {
                if (int.TryParse(bagMatch.Groups[1].Value, out int minutes))
                {
                    if (result.BaggageMinutes.HasValue)
                    {
                        result.ValidationErrors.Add("Baggage command specified multiple times");
                    }
                    else
                    {
                        result.BaggageMinutes = minutes;
                    }
                    return true;
                }
            }

            // Cleaning pattern: CLEANn
            var cleanMatch = Regex.Match(upperCommand, @"^CLEAN(\d+)$");
            if (cleanMatch.Success)
            {
                if (int.TryParse(cleanMatch.Groups[1].Value, out int minutes))
                {
                    if (result.CleaningMinutes.HasValue)
                    {
                        result.ValidationErrors.Add("Cleaning command specified multiple times");
                    }
                    else
                    {
                        result.CleaningMinutes = minutes;
                    }
                    return true;
                }
            }

            // Jet Bridge pattern: PBBx (where x is 0/90/180/270)
            var pbbMatch = Regex.Match(upperCommand, @"^PBB(\d+)$");
            if (pbbMatch.Success)
            {
                if (int.TryParse(pbbMatch.Groups[1].Value, out int angle))
                {
                    if (result.JetBridgeAngle.HasValue)
                    {
                        result.ValidationErrors.Add("Jet bridge command specified multiple times");
                    }
                    else if (!ValidJetBridgeAngles.Contains(angle))
                    {
                        result.ValidationErrors.Add($"Invalid jet bridge angle: {angle}. Valid angles are: 0, 90, 180, 270");
                    }
                    else
                    {
                        result.JetBridgeAngle = angle;
                    }
                    return true;
                }
            }

            return false;
        }
    }
}