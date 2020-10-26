using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static TcpStuff.Interfaces.INetworkAction;

namespace TcpStuff.Classes.Services
{
    public class ServerService<T> : NetworkService where T : ClientService
    {
        private readonly TcpListener    m_oNetObject;

        private readonly List<T>        m_oConnectedClient;

        public override bool IsConnected => m_oNetObject?.Server?.Connected ?? false;

        public override Socket NetworkSocket => m_oNetObject?.Server ?? null; 

        public ServerService(IPAddress a_oIPAddress, int a_iPort) : base(ModeEnum.Server,a_oIPAddress,a_iPort)
        {
            m_oNetObject = new TcpListener(Address, Port);

            m_oConnectedClient = new List<T>();
        }

        public override void Establish()
        {
            if (IsConnected)
                return;

            try
            {
                m_oNetObject.Start();

                AcceptConnection();

                return;
            }
            catch (Exception)
            {
            }

            NetworkAction?.StateChanged(State.Error, new StateObject(this));
        }

        protected virtual void AcceptConnection()
        {
            try
            {
                NetworkAction?.StateChanged(State.Listening, new StateObject(this));

                m_oNetObject?.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), this);
            }
            catch (Exception e)
            {
                NetworkAction?.StateChanged(State.Error, new StateObject(this, e));
            }
        }

        protected virtual void AcceptCallback(IAsyncResult ar)
        {
            var _obj = ar.AsyncState as ServerService<T>;

            try
            {
                T _client = (T)Activator.CreateInstance(typeof(T), new object[]
                {
                     _obj.NetworkSocket.EndAccept(ar),
                     NetworkService.BUFFER_SIZE
                });

                _client.NetworkAction = _obj.NetworkAction;
                _client.RegisteredServer = _obj;
                _obj.m_oConnectedClient.Add(_client);

                _obj.NetworkAction.StateChanged(State.Established, new StateObject(_obj, _client));

                _obj.AcceptConnection();

                return;
            }
            catch (Exception)
            {
            }

            _obj?.NetworkAction?.StateChanged(State.Error, new StateObject(_obj));
        }

        public List<T> ConnectedClients
        {
            get
            {
                m_oConnectedClient.RemoveAll((x) =>
               {
                   try
                   {
                       return !x.IsConnected;
                   }
                   catch (Exception)
                   {
                   }

                   return true;
               });
                
                return m_oConnectedClient;
            }
        }
        public T GetClientByIdentifier(string a_sIdentifier) => ConnectedClients.Find(x => x.Identifier == a_sIdentifier);

        public virtual void AsyncSendBroadcast(NetworkData a_oData, T a_oSender = null)
        {
            foreach (var _oClient in ConnectedClients)
            {
                try
                {
                    if (_oClient != a_oSender)
                    {
                        _oClient.AsyncSend(a_oData);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
