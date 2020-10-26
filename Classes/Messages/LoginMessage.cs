using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using TcpStuff.Classes.Services;
using TcpStuff.Interfaces;

namespace TcpStuff.Classes.Messages
{
    [DataContract]
    public class LoginMessage : XmlStorage<LoginMessage>, IMessage
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public Response Response { get; protected set; }

        public LoginMessage()
        {
            this.BaseObject = this;
        }

        public override bool InitializeFromObject(LoginMessage Object)
        {
            this.Login = Object.Login;
            this.Response = Object.Response;
            return true;
        }

        public IMessage ProcessRequest(StateObject a_oObj = null)
        {
            var _client = a_oObj.GetObject<ClientService>();

            if (_client.HasRegisteredServer)
            {
                var _server = _client.GetRegisteredServer<ServerService<ClientService>>();

                if (_server.GetClientByIdentifier(Login) == null)
                {
                    _client.Identifier = Login;
                    Response = new Response(1, $"Hurra :D! Zalogowałeś się do serwera pod loginem {Login}");
                }
                else
                {
                    Response = new Response(0, $"Użytkownik o loginie {Login} już istnieje na serwerze! :-(");
                }
            }
            else
            {
                Response = new Response(0, "Wystąpił wyjątek krytyczny - klient nie jest zarejestrowany po stronie serwera!");
            }

            return this;
        }

        public IMessage ProcessResponse(StateObject a_oObj = null)
        {
            var _client = a_oObj.GetObject<ClientService>();

            if (Response.Code == 0)
                throw new Exception(Response.ToString());

            Console.WriteLine(Response);

            _client.Identifier = Login;

            return this;
        }

        public override string ToString()
        {
            return $"[Login={Login}]";
        }

        public NetworkData AsNetworkData(int a_iBufferSize = NetworkService.BUFFER_SIZE)
        {
            return new NetworkData(a_iBufferSize)
            {
                Buffer = ToXml().ToArray() 
            };
        }
    }
}
