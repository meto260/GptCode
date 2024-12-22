using GptCode.Services;
using GptCode.Utils;
using System.IO;

var _cmd = new CommandService();
var _prompt = new PromptService();
string path = FileScope.GetProjectPath();

string userPrompt = Console.ReadLine() ?? string.Empty;
await RunCmd(userPrompt);
_cmd.RunUserCommand($"cd {path} && dotnet run");

async Task RunCmd(string prompt) {
    var ollamaResponse = await _prompt.PrepareCodePrompt(prompt);

    var fileNames = _cmd.CleanFileNames(ollamaResponse.response);
    var fileCodes = _cmd.CleanMarkups(ollamaResponse.response);

    FileScope.Prepare();
    var errs = _cmd.RunUserCommand($"cd {path} && dotnet new console");

    var getFiles = Directory.GetFiles(path);
    int index = 0;
    foreach (var file in fileNames) {
        string filepath = Path.Combine(path, file);
        FileScope.KillFileProcess(filepath);
        File.WriteAllText(filepath, fileCodes[index]);
        index++;
    }
    if (errs?.Count > 0) {
        string fixcmd = string.Join("\n", errs);
        await RunCmd(fixcmd);
    }
    File.WriteAllText($"logs{DateTime.Now.ToString("yyyyMMddhhmmss")}.txt", ollamaResponse.response);
}