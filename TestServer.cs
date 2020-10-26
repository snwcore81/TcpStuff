using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TcpStuff.Classes;
using TcpStuff.Classes.Services;
using TcpStuff.Interfaces;
using static TcpStuff.Interfaces.INetworkAction;

namespace TcpStuff
{
    public class TestServer : INetworkAction
    {
        public ServerService<ClientService> Server;

        public void StateChanged(State a_eState, StateObject a_oStateObj = null)
        {
            switch (a_eState)
            {
                case State.Listening:
                    OnListening(a_oStateObj);
                    break;

                case State.Established:
                    OnEstablished(a_oStateObj);
                    break;

                case State.Received:
                    OnReceived(a_oStateObj);
                    break;
            }
        }

        protected void OnReceived(StateObject a_oStateObj)
        {
            var _client = a_oStateObj.GetObject<ClientService>();

            var _message = MessageFactory.Instance.Create(_client.Data.BufferWithData) as IMessage;

            Console.WriteLine($"OnReceived::{_message}");

            if (_message != null && _message.ProcessRequest(a_oStateObj) != null)
            {
                _client.AsyncSend(_message.AsNetworkData());
            }

            _client.AsyncReceive();
        }

        protected void OnListening(StateObject a_oStateObj)
        {
            Console.WriteLine("Serwer oczekuje na połączenie ze strony klienta.... :D");
        }

        protected void OnEstablished(StateObject a_oStateObj)
        {
            var _client = a_oStateObj.GetData<ClientService>();

            Console.WriteLine($"Udało się podłączyć klienta! :D Identyfikator <{_client.Identifier}>");

            _client.AsyncReceive();
        }

        public virtual void Run()
        {
            Server = new ServerService<ClientService>(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            Server.Establish();

            Console.ReadKey();
        }
    }
}
