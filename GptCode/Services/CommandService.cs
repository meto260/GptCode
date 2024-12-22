using System.Diagnostics;

namespace GptCode.Services {
    public interface ICommandService {
        public List<string> RunUserCommand(string command);
        public string RemoveMarkup(string ollamaMessage);
        public List<string> CleanFileNames(string ollamaMessage);
        public List<string> CleanMarkups(string ollamaMessage);
    }
    public class CommandService : ICommandService {
        public List<string> CleanFileNames(string ollamaMessage) {
            var splitNames = ollamaMessage.Split("\n");
            var result = splitNames
                .Where(x => x.StartsWith("###") && (x.Contains(".cs") || x.Contains(".csproj")))
                .Select(x => x.Split(' ')[1])
                .ToList();
            if(result==null || result.Count == 0) {
                result.Add("Program.cs");
            }
            return result;
        }

        public List<string> CleanMarkups(string ollamaMessage) {
            var splitCodes = ollamaMessage.Split("```");
            return splitCodes.Where(x => x.StartsWith("csharp")).Select(x => x.Replace("csharp", "")).ToList();
        }

        public string RemoveMarkup(string ollamaMessage) {
            if (string.IsNullOrEmpty(ollamaMessage))
                return string.Empty;

            string startMarker = "```csharp";
            string endMarker = "```";

            int startIndex = ollamaMessage.IndexOf(startMarker);
            if (startIndex == -1)
                return string.Empty;

            startIndex += startMarker.Length;

            int endIndex = ollamaMessage.IndexOf(endMarker, startIndex);
            if (endIndex == -1)
                return string.Empty;

            string result = ollamaMessage.Substring(startIndex, endIndex - startIndex).Trim();

            return result;
        }

        public List<string> RunUserCommand(string command) {
            List<string> errors = new();
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command) {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Console.WriteLine("command run:" + command);

            using (var process = new Process { StartInfo = processInfo }) {
                try {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output)) {
                        Console.WriteLine("Output:");
                        Console.WriteLine(output);
                    }

                    if (!string.IsNullOrEmpty(error)) {
                        Console.WriteLine("Error:");
                        Console.WriteLine(error);
                        errors.Add(error);
                    }

                    if (process.ExitCode != 0) {
                        Console.WriteLine($"Process exited with code {process.ExitCode}");
                    } else {
                        Console.WriteLine("Process completed successfully.");
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally {
                    if (!process.HasExited) {
                        Console.WriteLine("Forcefully terminating the process...");
                        process.Kill(true);
                    }

                    process.Close();
                }
            }
            return errors;
        }
    }
}
