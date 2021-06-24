using System.IO;
using System.Threading.Tasks;

namespace Octopus.CommandLine.Commands
{
    public interface ICommand
    {
        void GetHelp(TextWriter writer, string[] args);
        Task Execute(string[] commandLineArguments);
        ICommandMetadata CommandMetadata { get; }
    }
}
