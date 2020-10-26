using System;
using System.Collections.Generic;
using System.Text;

namespace TcpStuff.Classes.Exceptions
{
    public class NetworkDataBufferIsEmpty : Exception
    {
        public NetworkDataBufferIsEmpty(string a_sBufferName = null) :
           base($"{a_sBufferName ?? ""} - bufor danych jest pusty!")
        {
        }
    }
}
