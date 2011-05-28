using System;
using System.Collections.Generic;
using System.Text;

namespace KeeCracker
{
    public interface IPasswordSource
    {
        string NextPassword();
    }
}
