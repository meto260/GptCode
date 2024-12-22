using System.Diagnostics;

namespace GptCode.Utils {
    public static class FileScope {
        static string projectPath = Directory.GetCurrentDirectory();
        static string newFolderPath = Path.Combine(projectPath, Guid.NewGuid().ToString());

        public static void Prepare() {
            if (Directory.Exists(newFolderPath)) {
                Directory.Delete(newFolderPath, true);
            }
            Directory.CreateDirectory(newFolderPath);

        }

        public static string GetProjectPath() {
            return newFolderPath;
        }

        public static void KillFileProcess(string filePath) {
            var process = GetProcessUsingFile(filePath);
            if (process != null) {
                process.Kill();
            }
        }

        static Process GetProcessUsingFile(string filePath) {
            var allProcesses = Process.GetProcesses();
            foreach (var process in allProcesses) {
                try {
                    var modules = process.Modules;
                    foreach (ProcessModule module in modules) {
                        if (module.FileName.Equals(filePath, StringComparison.OrdinalIgnoreCase)) {
                            return process;
                        }
                    }
                }
                catch {
                    // Erişim hatalarını yoksay
                }
            }
            return null;
        }
    }
}
