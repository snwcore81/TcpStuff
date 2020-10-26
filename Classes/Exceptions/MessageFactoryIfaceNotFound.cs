using System;
using System.Collections.Generic;
using System.Text;

namespace TcpStuff.Classes.Exceptions
{
    public class MessageFactoryIfaceNotFound : Exception
    {
        public MessageFactoryIfaceNotFound(string a_sTypeName, string a_sIfaceName)
            : base($"Nie odnaleziono implementacji interfejsu <{a_sIfaceName}> w typie <{a_sTypeName}>")
        {

        }
    }
}
