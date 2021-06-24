using System;
using Octopus.CommandLine.Commands;

namespace Octopus.CommandLine.ShellCompletion
{
    public interface IShellCompletionInstaller
    {
        SupportedShell SupportedShell { get; }
        string ProfileLocation { get; }
        string ProfileScript { get; }
        void Install(bool dryRun);
    }
}