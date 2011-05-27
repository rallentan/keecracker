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

namespace KeeCracker
{
    public partial class MainForm : Form
    {
        //--- Fields ---
        Cracker _cracker = new Cracker();

        //--- Constructors ---

        public MainForm()
        {
            InitializeComponent();

            _cracker.PasswordFound += new PasswordFoundEventHandler(_cracker_PasswordFound);
            _cracker.PasswordNotFound += new EventHandler(_cracker_PasswordNotFound);
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

            var passwordGenerator = new PasswordGenerator();

            _cracker.StartAttack(databasePathLabeledTextBox.TextBoxText, 2, passwordGenerator);
        }

        //--- Private Methods ---

        void TearDown()
        {
            _cracker.Stop();
            _cracker.PasswordFound -=  new PasswordFoundEventHandler(_cracker_PasswordFound);
            _cracker.PasswordNotFound -= new EventHandler(_cracker_PasswordNotFound);
        }

        //--- Events Handlers ---

        void _cracker_PasswordFound(object sender, PasswordFoundEventArgs e)
        {
            MessageBox.Show("Password cracked!\n\n" + e.Password);
        }

        void _cracker_PasswordNotFound(object sender, EventArgs e)
        {
            MessageBox.Show("Password not found");
        }

        void closeButton_Click(object sender, EventArgs e)
        {
            TearDown();
            Close();
        }
    }
}
