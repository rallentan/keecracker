using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using NDesk.Options;
using System.Threading;

namespace KeeCracker
{
    class Program
    {
        static readonly string _assemblyFullName = Assembly.GetExecutingAssembly().GetName().FullName;
        static readonly string _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        static OptionSet _optionSet;

        static void Main(string[] args)
        {
            int threadCount = 1;
            bool showHelp   = false;
            List<string> wordlists = new List<string>();
            List<string> floatingArgs;

            _optionSet = new OptionSet()
                .Add(
                    "w|wordlist=",
                    "Path to a wordlist or ”-” without the quotes for standard in (stdin).",
                    v => wordlists.Add(v))
                .Add<int>(
                    "t|threads=",
                    "Number of threads to use.",
                    v => threadCount = v)
                .Add(
                    "h|?|help",
                    "Show this help.",
                    v => showHelp = v != null);

            try
            {
                floatingArgs = _optionSet.Parse(args);
            }
            catch (OptionException ex)
            {
                WriteOptionsErrorMessage(ex.Message);
                return;
            }

            if (showHelp)
            {
                WriteUsageInformation();
                return;
            }

            if (floatingArgs.Count != 1)
            {
                WriteOptionsErrorMessage("Must specify exactly one KeePass2 database file.");
                return;
            }

            if (wordlists.Count == 0)
            {
                WriteOptionsErrorMessage("Must specify at least one wordlist.");
                return;
            }

            if (threadCount <= 0)
            {
                WriteOptionsErrorMessage("The thread count must be a non-negative integer.");
                return;
            }

            string dbPath = floatingArgs[0];

            foreach (var wordlist in wordlists)
            {
                Cracker _cracker = new Cracker();

                _cracker.PasswordFound += _cracker_PasswordFound;
                _cracker.PasswordNotFound += _cracker_PasswordNotFound;

                var timer = new Timer(TimerCallback, _cracker, 1000, 1000);

                if (wordlist == "-")
                {
                    IPasswordSource passwordSource = new PasswordList(Console.In);
                }
                else
                {
                    using (var fileStream = new StreamReader(wordlist))
                    {
                        IPasswordSource passwordSource = new PasswordList(fileStream);
                        _cracker.StartAttack(dbPath, threadCount, passwordSource);
                        _cracker.WaitUntilCompleted();
                    }
                }

                _cracker.PasswordFound -= _cracker_PasswordFound;
                _cracker.PasswordNotFound -= _cracker_PasswordNotFound;
            }
        }

        static void WriteOptionsErrorMessage(string message)
        {
            Console.WriteLine(_assemblyFullName);
            if (message != null)
                Console.WriteLine(message);
            Console.WriteLine("Try '" + Assembly.GetExecutingAssembly().GetName().Name + " --help' for more information.");
        }

        static void WriteUsageInformation()
        {
            Console.WriteLine(_assemblyFullName);
            Console.WriteLine("Usage: " + _assemblyName + " [OPTIONS] <database_path>");
            _optionSet.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("Examples:");
            Console.WriteLine("  " + _assemblyName + " -t4 -w KeePassDb.kdbx");
            Console.WriteLine("  john --incremental --stdout | " + _assemblyName + " -w -");
        }

        static void TimerCallback(object crackerObj)
        {
            var cracker = (Cracker)crackerObj;

            if (!string.IsNullOrEmpty(cracker.LastGuess))
                Console.WriteLine("Checked: " + cracker.LastGuess);
        }

        static void _cracker_PasswordFound(object sender, PasswordFoundEventArgs e)
        {
            Console.WriteLine("Password cracked!\n\n" + e.Password);
        }

        static void _cracker_PasswordNotFound(object sender, EventArgs e)
        {
            Console.WriteLine("Password not found.");
        }
    }
}
