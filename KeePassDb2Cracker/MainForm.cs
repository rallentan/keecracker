using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLib;
using System.Diagnostics;
using System.Threading;
using System.IO;
using NLib.Net;
using KeePassDbCracker.DistributedProcessing;

namespace KeePassDbCracker
{
    public partial class MainForm : Form
    {
        //--- Fields ---
        int _guessesSinceLastSpeedCheck = 0;
        string _passwordFound;
        uint _guessingThreadsRunning = 0;
        string _lastGuess;
        List<RpcController> _clients = new List<RpcController>();
        Cracker _cracker = new Cracker();

        //--- Constructors ---

        public MainForm()
        {
            InitializeComponent();

            _cracker.PasswordFound += new PasswordFoundEventHandler(cracker_PasswordFound);
        }

        //--- Event Handlers ---

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;

            int numThreads;
            if (!int.TryParse(numThreadsLabeledTextBox.TextBoxText, out numThreads) ||
                numThreads <= 0)
            {
                MessageBox.Show("Invalid thread count");
            }

            _cracker.StartAttack(databasePathLabeledTextBox.TextBoxText, 2);
        }

        //--- Private Methods ---

        void StartAttack(string databasePath)
        {
            if (!PerformSelfTest())
            {
                MessageBox.Show("Error: Self test failed");
                return;
            }

            var passwordGenerator = new PasswordGenerator();

            var fileStream = new FileStream(databasePath, FileMode.Open, FileAccess.Read);

            Guesser cracker = new Guesser(fileStream);

            for (int i = 0; i < 1; i++)
            {
                EasyThread.BeginInvoke(new EasyThreadMethod(
                () =>
                {
                    Thread.MemoryBarrier();
                    _guessingThreadsRunning++;
                    Thread.MemoryBarrier();

                    string result = AttackThread(cracker, passwordGenerator);
                    if (result != null)
                        _passwordFound = result;

                    Thread.MemoryBarrier();
                    _guessingThreadsRunning--;
                    Thread.MemoryBarrier();

                    if (_guessingThreadsRunning == 0)
                        fileStream.Close();
                }));
            }

            timer.Enabled = true;
        }

        string AttackThread(Guesser cracker, PasswordGenerator passwordGenerator)
        {
            string passwordGuess;


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (true)
            {
                Thread.MemoryBarrier();
                if (_passwordFound != null)
                    return null;

                passwordGuess = passwordGenerator.NextPassword();
                if (passwordGuess == null)
                    return null;

                if (cracker.TryKey(passwordGuess))
                    break;

                _guessesSinceLastSpeedCheck++;
                _lastGuess = passwordGuess;
                Thread.MemoryBarrier();
            }

            return passwordGuess;
        }

        //string AttackThread_Deprecated(string databasePath, PasswordGenerator passwordGenerator)
        //{
        //    var ioc = IOConnectionInfo.FromPath(databasePath);
        //    var database = new PwDatabase();
        //    var compositeKey = new CompositeKey();

        //    string passwordGuess;

        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();

        //    while (true)
        //    {
        //        Thread.MemoryBarrier();
        //        if (_passwordFound != null)
        //            return null;

        //        passwordGuess = passwordGenerator.NextPassword();
        //        if (passwordGuess == null)
        //            return null;

        //        compositeKey.Clear();

        //        var passwordKey = new KcpPassword(passwordGuess);
        //        compositeKey.AddUserKey(passwordKey);

        //        try
        //        {
        //            database.Open(ioc, compositeKey, null);
        //            break;
        //        }
        //        catch (InvalidCompositeKeyException)
        //        {
        //        }
        //        _guessesSinceLastSpeedCheck++;
        //        _lastGuess = passwordGuess;
        //        Thread.MemoryBarrier();
        //    }

        //    return passwordGuess;
        //}

        static bool PerformSelfTest()
        {
            try
            {
                //SelfTest.Perform();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //--- Events Handlers ---

        void cracker_PasswordFound(object sender, PasswordFoundEventArgs e)
        {
            MessageBox.Show("Password cracked!\n\n" + e.Password);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lastGuessLabeledLabel.ValueText = _lastGuess;
            rateLabeledLabel.ValueText = _guessesSinceLastSpeedCheck + " per sec";
            _guessesSinceLastSpeedCheck = 0;

            Thread.MemoryBarrier();
            if (_passwordFound != null)
            {
                timer.Enabled = false;
                MessageBox.Show("Password cracked!\n\n" + _passwordFound);
            }
            else if (_guessingThreadsRunning == 0)
            {
                timer.Enabled = false;
                MessageBox.Show("Finished. No passwords found.");
            }
        }

        void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
