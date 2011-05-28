using System;
using System.Collections.Generic;
using System.Text;

namespace KeeCracker
{
    public class PasswordFoundEventArgs
    {
        //--- Constructors ---

        public PasswordFoundEventArgs(string password)
        {
            Password = password;
        }

        //--- Public Properties ---

        public string Password { get; private set; }
    }

    public delegate void PasswordFoundEventHandler(object sender, PasswordFoundEventArgs e);
}
