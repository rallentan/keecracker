using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeeCracker
{
    class PasswordGenerator : IPasswordSource
    {
        //--- Fields ---
        StringBuilder _password;
        object _syncLock = new object();

        //--- Constructors ---

        public PasswordGenerator()
        {
        }

        //--- Public Methods ---

        public string NextPassword()
        {
            lock (_syncLock)
            {
                if (_password == null)
                {
                    _password = new StringBuilder();
                    _password.Append("kvm,,, gsmn.");
                    return _password.ToString();
                }

                for (int pos = 5; pos >= 3; pos--)
                {
                    if (_password[pos] == ',')
                        _password[pos] = '.';
                    else if (_password[pos] == '.')
                        _password[pos] = ' ';
                    else if (_password[pos] == ' ')
                        _password[pos] = 'a';
                    else
                    {
                        _password[pos]++;

                        if (_password[pos] > 'z')
                        {
                            _password[pos] = ',';
                            continue;
                        }
                    }
                    return _password.ToString();
                }

                return null;
            }
        }
    }
}
