using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Octopus.CommandLine.Commands;
using Octopus.CommandLine.Extensions;

namespace Octopus.CommandLine.ShellCompletion
{
    public static class CommandSuggester
    {
        public static IEnumerable<string> SuggestCommandsFor(
            string[] words,
            IReadOnlyDictionary<string, string[]> completionItems)
        {
            var executablePath = AssemblyHelper.GetExecutablePath();
            var exeName = Path.GetFileNameWithoutExtension(executablePath);
            var exeNameWithoutExtension = Path.GetFileNameWithoutExtension(executablePath);
            // some shells will pass the command name as invoked on the command line.
            // If so, strip them from the beginning of the array
            words = words
                .Take(2)
                .Except(new[] { exeName, exeNameWithoutExtension, CompleteCommand.Name }, StringComparer.OrdinalIgnoreCase)
                .Union(words.Skip(2))
                .Where(word => string.IsNullOrWhiteSpace(word) == false)
                .ToArray();

            var numberOfArgs = words.Length;
            var hasSubCommand = numberOfArgs > 1;
            var searchTerm =
                numberOfArgs > 0
                    ? words.Last()
                    : "";
            var suggestions = new List<string>();
            var isOptionSearch = searchTerm.StartsWith("--");
            var allSubCommands = completionItems.Keys.ToList();

            if (isOptionSearch)
            {
                if (hasSubCommand)
                {
                    // e.g. `octo subcommand --searchTerm`
                    var subCommandName = words.Where(w => IsSubCommand(w, allSubCommands)).Last();
                    suggestions.AddRange(GetSubCommandOptionSuggestions(completionItems, subCommandName, searchTerm));
                }
                else
                {
                    // e.g. `exename --searchTerm`
                    return GetBaseOptionSuggestions(completionItems, searchTerm).OrderBy(name => name);
                }
            }
            else if (ZeroOrOneSubCommands(words, allSubCommands))
            {
                // e.g. `exename searchterm` or just `exename`
                suggestions.AddRange(GetSubCommandSuggestions(completionItems, searchTerm));
            }

            return suggestions.OrderBy(name => name);
        }

        static IEnumerable<string> GetSubCommandSuggestions(IReadOnlyDictionary<string, string[]> completionItems, string searchTerm)
        {
            return completionItems.Keys.Where(s =>
                s.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        static bool ZeroOrOneSubCommands(string[] words, List<string> allSubCommands)
        {
            return words.Where(w => IsSubCommand(w, allSubCommands)).Count() <= 1;
        }

        static IEnumerable<string> GetSubCommandOptionSuggestions(IReadOnlyDictionary<string, string[]> completionItems, string subCommandName, string searchTerm)
        {
            if (completionItems.TryGetValue(subCommandName, out var options))
                return options
                    .Where(name => name.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return new string[] { };
        }

        static IEnumerable<string> GetBaseOptionSuggestions(IReadOnlyDictionary<string, string[]> completionItems, string searchTerm)
            // If you type 'exename', you'll be redirected to the 'help' command, so show these options
            => GetSubCommandOptionSuggestions(completionItems, "help", searchTerm);

        static bool IsSubCommand(string arg, List<string> subCommandList)
        {
            if (arg == null) return false;
            if (arg.StartsWith("--")) return false;
            return subCommandList.Contains(arg);
        }
    }
}