using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeePassDbCracker
{
    class PasswordFoundEventArgs
    {
        //--- Constructors ---

        public PasswordFoundEventArgs(string password)
        {
            Password = password;
        }

        //--- Public Properties ---

        public string Password { get; private set; }
    }

    delegate void PasswordFoundEventHandler(object sender, PasswordFoundEventArgs e);
}
