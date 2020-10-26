﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TcpStuff.Classes.Exceptions
{
    public class StateObjectDataIsNull : Exception
    {
        public StateObjectDataIsNull() : base("Referencja do danych ma wartość null!")
        {

        }
    }
}
