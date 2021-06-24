using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.CommandLine.Extensions;

namespace Octopus.CommandLine.Commands
{
    [Command("help", "?", "h", Description = "Prints this help text.")]
    public class HelpCommand : CommandBase
    {
        readonly Lazy<ICommandLocator> commands;

        public HelpCommand(Lazy<ICommandLocator> commands, ICommandOutputProvider commandOutputProvider) : base(commandOutputProvider)
            => this.commands = commands;

        public override Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() =>
            {
                Options.Parse(commandLineArguments);

                commandOutputProvider.PrintMessages = OutputFormat == OutputFormat.Default;

                var commandName = commandLineArguments.FirstOrDefault();

                if (string.IsNullOrEmpty(commandName))
                {
                    PrintGeneralHelp();
                }
                else
                {
                    var command = commands.Value.Find(commandName);

                    if (command == null)
                    {
                        if (!commandName.StartsWith("--"))
                        {
                            // wasn't a parameter!
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Command '{0}' is not supported", commandName);
                        }

                        Console.ResetColor();
                        PrintGeneralHelp();
                    }
                    else
                    {
                        PrintCommandHelp(command, commandLineArguments);
                    }
                }
            });
        }

        void PrintCommandHelp(ICommand command, string[] args)
        {
            command.GetHelp(Console.Out, args);
        }

        void PrintGeneralHelp()
        {
            if (HelpOutputFormat == OutputFormat.Json)
                PrintJsonOutput();
            else
                PrintDefaultOutput();
        }

        public Task Request()
            => Task.WhenAny();

        public void PrintDefaultOutput()
        {
            var executable = AssemblyHelper.GetExecutableName();

            Console.ResetColor();
            commandOutputProvider.PrintHeader();
            Console.Write("Usage: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(executable + " <command> [<options>]");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Where <command> is one of: ");
            Console.WriteLine();

            foreach (var possible in commands.Value.List().OrderBy(x => x.Name))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  " + possible.Name.PadRight(15, ' '));
                Console.ResetColor();
                Console.WriteLine("   " + possible.Description);
            }

            Console.WriteLine();
            Console.Write("Or use ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(executable + " help <command>");
            Console.ResetColor();
            Console.WriteLine(" for more details.");
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(commands.Value.List()
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    x.Aliases
                }));
        }
    }
}