using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NLib;
using System.Threading;
using System.Diagnostics;

namespace KeeCracker
{
    public class Cracker
    {
        //--- Constants ---
        const int STATISTICS_UPDATE_INTERVAL = 5000;  // milliseconds

        //--- Fields ---
        int _guessesSinceLastSpeedCheck = 0;
        string _passwordFound;
        uint _guessingThreadsRunning = 0;
        string _lastGuess;
        float _guessRate = 0.0f;
        bool _started = false;
        object _syncLock = new object();
        bool _stopWorking = false;
        object _waitObject = new object();
        bool _completed = false;

        //--- Events ---

        public event PasswordFoundEventHandler PasswordFound;
        public event EventHandler PasswordNotFound;
        public event EventHandler Completed;
        
        //--- Public Methods ---

        public void StartAttack(string databasePath, int numThreads, IPasswordSource passwordSource)
        {
            if (numThreads <= 0)
                throw new ArgumentOutOfRangeException("numThreads");

            lock (_syncLock)
            {
                if (_started)
                    throw new InvalidOperationException();
                _started = true;
            }

            if (!PerformSelfTest())
                throw new Exception("Self test failed");

            var fileStream = new FileStream(databasePath, FileMode.Open, FileAccess.Read);

            DatabaseOpener cracker = new DatabaseOpener(fileStream);

            for (int i = 0; i < numThreads; i++)
            {
                EasyThread.BeginInvoke(new EasyThreadMethod(
                () =>
                {
                    Thread.MemoryBarrier();
                    _guessingThreadsRunning++;
                    Thread.MemoryBarrier();

                    string result = AttackThread(cracker, passwordSource);
                    if (result != null)
                        _passwordFound = result;

                    Thread.MemoryBarrier();
                    _guessingThreadsRunning--;
                    Thread.MemoryBarrier();

                    if (_guessingThreadsRunning == 0)
                        fileStream.Close();
                }));
            }

            EasyThread.BeginInvoke(new EasyThreadMethod(
            () =>
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                while (true)
                {
                    if (_guessingThreadsRunning == 0)
                    {
                        OnPasswordNotFound(EventArgs.Empty);
                        OnCompleted(EventArgs.Empty);
                        break;
                    }

                    Thread.Sleep(STATISTICS_UPDATE_INTERVAL);
                }
            }));
        }

        public void Stop()
        {
            lock (_syncLock)
            {
                if (!_started)
                    throw new InvalidOperationException();
                _stopWorking = true;
            }
        }

        public void WaitUntilCompleted()
        {
            lock (_waitObject)
            {
                if (_completed)
                    return;
                Monitor.Wait(_waitObject);
            }
        }

        //--- Public Properties ---

        public string LastGuess
        {
            get { return _lastGuess; }
        }

        public float GuessRate
        {
            get { return _guessRate; }
        }

        //--- Protected Methods ---

        protected virtual void OnPasswordFound(PasswordFoundEventArgs e)
        {
            if (PasswordFound != null)
                PasswordFound(this, e);
        }

        protected virtual void OnPasswordNotFound(EventArgs e)
        {
            if (PasswordNotFound != null)
                PasswordNotFound(this, e);
        }

        protected virtual void OnCompleted(EventArgs e)
        {
            if (Completed != null)
                Completed(this, e);

            // Release waiting threads that called the Wait method

            lock (_waitObject)
            {
                _completed = true;
                Monitor.PulseAll(_waitObject);
            }
        }

        //--- Private Methods ---

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

        string AttackThread(DatabaseOpener cracker, IPasswordSource passwordSource)
        {
            string passwordGuess;

            while (true)
            {
                if (_stopWorking)
                    return null;

                if (_passwordFound != null)
                    return null;

                passwordGuess = passwordSource.NextPassword();
                if (passwordGuess == null)
                    return null;

                if (cracker.TryKey(passwordGuess))
                    break;

                Thread.MemoryBarrier();

                _guessesSinceLastSpeedCheck++;
                _lastGuess = passwordGuess;
            }

            return passwordGuess;
        }

        void UpdateStatistics()
        {
            _guessRate = (float)_guessesSinceLastSpeedCheck / (float)STATISTICS_UPDATE_INTERVAL;
            _guessesSinceLastSpeedCheck = 0;

            Thread.MemoryBarrier();

            if (_passwordFound != null)
            {
                OnPasswordFound(new PasswordFoundEventArgs(_passwordFound));
                OnCompleted(EventArgs.Empty);
            }
        }
    }
}
