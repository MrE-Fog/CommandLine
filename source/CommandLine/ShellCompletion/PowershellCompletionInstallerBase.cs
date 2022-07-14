using System;
using System.IO;
using System.Linq;
using System.Text;
using Octopus.CommandLine.Plumbing;

namespace Octopus.CommandLine.ShellCompletion;

public abstract class PowershellCompletionInstallerBase : ShellCompletionInstaller
{
    protected PowershellCompletionInstallerBase(ICommandOutputProvider commandOutputProvider, IOctopusFileSystem fileSystem, string[] executablePaths)
        : base(commandOutputProvider, fileSystem, executablePaths)
    {
    }

    protected static string PowershellProfileFilename => "Microsoft.PowerShell_profile.ps1";

    public override string ProfileScript
    {
        get
        {
            var results = new StringBuilder();

            foreach (var exePath in ExecutablePaths.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var command = Path.GetFileNameWithoutExtension(exePath);
                results.AppendLine($"Register-ArgumentCompleter -Native -CommandName {command} -ScriptBlock {{");
                results.AppendLine("    param($wordToComplete, $commandAst, $cursorPosition)");
                results.AppendLine("    $params = $commandAst.ToString().Split(' ') | Select-Object -skip 1");
                results.AppendLine($"    & \"{exePath}\" complete $params | ForEach-Object {{");
                results.AppendLine("        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterName', $_)");
                results.AppendLine("    }");
                results.AppendLine("}");
            }

            return results.ToString();
        }
    }
}