using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using NDesk.Options;
using System.Threading;
using System.Diagnostics;

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
            bool showHelp = false;
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
                if (wordlist == "-")
                {
                    IPasswordSource passwordSource = new PasswordList(Console.In);
                    AttackWithPasswordSourceAndWait(dbPath, threadCount, passwordSource);
                }
                else
                {
                    using (var fileStream = new StreamReader(wordlist))
                    {
                        IPasswordSource passwordSource = new PasswordList(fileStream);
                        AttackWithPasswordSourceAndWait(dbPath, threadCount, passwordSource);
                    }
                }
            }

            PressEnterToContinue();
        }

        static void AttackWithPasswordSourceAndWait(string databasePath, int threadCount, IPasswordSource passwordSource)
        {
            Cracker cracker = new Cracker();

            cracker.PasswordFound += _cracker_PasswordFound;
            cracker.PasswordNotFound += _cracker_PasswordNotFound;

            using (var timer = new Timer(TimerCallback, cracker, 3000, 3000))
            {
                cracker.StartAttack(databasePath, threadCount, passwordSource);
                cracker.WaitUntilCompleted();
            }

            cracker.PasswordFound -= _cracker_PasswordFound;
            cracker.PasswordNotFound -= _cracker_PasswordNotFound;
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
            Console.WriteLine("  john --incremental --stdout | " + _assemblyName + " -w - KeePassDb.kdbx");
        }

        static void TimerCallback(object crackerObj)
        {
            var cracker = (Cracker)crackerObj;

            if (cracker.IsRunning && !string.IsNullOrEmpty(cracker.LastGuess))
                Console.WriteLine("Rate: " + cracker.GuessRate.ToString("f1") + " per second" + "\tLast Candidate: " + cracker.LastGuess);
        }

        [Conditional("DEBUG")]
        static void PressEnterToContinue()
        {
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

        static void _cracker_PasswordFound(object sender, PasswordFoundEventArgs e)
        {
            Console.WriteLine("Password cracked!");
            Console.WriteLine("Password: " + e.Password);
        }

        static void _cracker_PasswordNotFound(object sender, EventArgs e)
        {
            Console.WriteLine("Password not found.");
        }
    }
}
