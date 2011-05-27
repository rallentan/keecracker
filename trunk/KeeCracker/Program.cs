using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib;
using System.IO;

namespace KeeCracker
{
    class Program
    {
        const string OPT_DATABASE_PATH = "DatabasePath";
        const string OPT_WORDLIST = "WordList";
        const string OPT_THREAD_COUNT = "ThreadCount";

        static readonly ConsoleOptionSchem[] OPTION_SCHEMS = new ConsoleOptionSchem[]
        {
            new ConsoleOptionSchem(
                OPT_DATABASE_PATH,
                "KeePass 2 database file to crack.",
                new string[] { "-d", "--database" },
                1),
            new ConsoleOptionSchem(
                OPT_WORDLIST,
                "File containing a list of passwords to try against the database.",
                new string[] { "-w", "--wordlist" },
                1,
                true),
            new ConsoleOptionSchem(
                OPT_THREAD_COUNT,
                "Number of threads to use.",
                new string[] { "-t", "--threads" },
                1),
        };

        static void Main(string[] args)
        {
            int threadCount;
            var options = new ConsoleOptionParser(OPTION_SCHEMS);
            options.ReadOptions(args);

            string dbPath = options.GetOptionInfo(OPT_DATABASE_PATH).SubOptions[0];
            IList<string> wordlists = options.GetOptionInfo(OPT_WORDLIST).SubOptions;
            if (!int.TryParse(options.GetOptionInfo(OPT_THREAD_COUNT).SubOptions[0], out threadCount))
                Console.Error.WriteLine("Invalid thread count. Must be an integer.");

            Cracker _cracker = new Cracker();

            _cracker.PasswordFound += new PasswordFoundEventHandler(_cracker_PasswordFound);
            _cracker.PasswordNotFound += new EventHandler(_cracker_PasswordNotFound);

            foreach (var wordlist in wordlists)
            {
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
                    }
                }
            }
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
