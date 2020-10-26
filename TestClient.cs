using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TcpStuff.Classes;
using TcpStuff.Classes.Messages;
using TcpStuff.Classes.Services;
using TcpStuff.Interfaces;
using static TcpStuff.Interfaces.INetworkAction;

namespace TcpStuff
{
    public class TestClient : INetworkAction
    {
        public ClientService Client;

        public void StateChanged(State a_eState, StateObject a_oStateObj = null)
        {
            switch (a_eState)
            {
                case State.Connecting:
                    OnConnecting(a_oStateObj);
                    break;

                case State.Connected:
                    OnConnected(a_oStateObj);
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

            try
            {
                _message.ProcessResponse(a_oStateObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            _client.AsyncReceive();
        }

        protected void OnConnecting(StateObject a_oStateObj)
        {
            Console.WriteLine("Próba połączenia z serwerem....");
        }

        protected void OnConnected(StateObject a_oStateObj)
        {
            Console.WriteLine("Yupi! Połączyłem się z serwerem! :D");

            var _client = a_oStateObj.GetObject<ClientService>();

            Console.Write("Podaj login:");

            var _loginMessage = new LoginMessage
            {
                Login = Console.ReadLine()
            };

            _client.AsyncReceive();

            _client.AsyncSend(_loginMessage.AsNetworkData());
        }

        public virtual void Run()
        {
            Client = new ClientService(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            Client.Establish();

            Console.ReadKey();
        }
    }
}
