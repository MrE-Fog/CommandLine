using System.IO;
using Octopus.CommandLine.Commands;
using Octopus.CommandLine.Extensions;
using Octopus.CommandLine.Plumbing;

namespace Octopus.CommandLine.ShellCompletion
{
    internal class PwshCompletionInstaller : PowershellCompletionInstallerBase
    {
        public override SupportedShell SupportedShell => SupportedShell.Pwsh;
        private string LinuxPwshConfigLocation => Path.Combine(HomeLocation, ".config", "powershell");
        private static string WindowsPwshConfigLocation => Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
            "Powershell"
        );
        public override string ProfileLocation => ExecutionEnvironment.IsRunningOnWindows
            ? Path.Combine(WindowsPwshConfigLocation, PowershellProfileFilename)
            : Path.Combine(LinuxPwshConfigLocation, PowershellProfileFilename);

        public override string ProfileScript => base.ProfileScript.NormalizeNewLines();

        public PwshCompletionInstaller(ICommandOutputProvider commandOutputProvider)
            : this(commandOutputProvider, new OctopusFileSystem(), new[] { AssemblyExtensions.GetExecutablePath() }) { }

        public PwshCompletionInstaller(ICommandOutputProvider commandOutputProvider, string[] executablePaths)
            : this(commandOutputProvider, new OctopusFileSystem(), executablePaths) { }

        public PwshCompletionInstaller(ICommandOutputProvider commandOutputProvider, IOctopusFileSystem fileSystem, string[] executablePaths)
            : base(commandOutputProvider, fileSystem, executablePaths) { }
    }
}
