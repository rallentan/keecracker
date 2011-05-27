using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KeeCracker
{
    public class PasswordList : IPasswordSource
    {
        //--- Fields ---
        TextReader _passwordFileReader;

        //--- Constructors ---

        public PasswordList(TextReader passwordFileStream)
        {
            _passwordFileReader = passwordFileStream;
        }

        //--- Public Methods ---

        public string NextPassword()
        {
            return _passwordFileReader.ReadLine();
        }
    }
}
