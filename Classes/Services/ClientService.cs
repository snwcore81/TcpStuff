using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static TcpStuff.Interfaces.INetworkAction;

namespace TcpStuff.Classes.Services
{
    public class ClientService : NetworkService
    {
        private readonly TcpClient m_oNetObject;
        public override bool IsConnected => (m_oNetObject?.Client?.Connected ?? false);
        public override Socket NetworkSocket => m_oNetObject?.Client ?? null;

        public ClientService(IPAddress a_oIPAddress, int a_iPort) : base(ModeEnum.Client,a_oIPAddress,a_iPort)
        {
            m_oNetObject = new TcpClient();
        }

        public ClientService(Socket a_oSocket, int a_iBufferLength = NetworkService.BUFFER_SIZE) 
            : base(ModeEnum.Client, a_iBufferLength)
        {
            m_oNetObject = new TcpClient
            {
                Client = a_oSocket
            };
        }

        public override void Establish()
        {
            if (IsConnected)
                return;

            try
            {
                m_oNetObject.BeginConnect(Address, Port, new AsyncCallback(ConnectCallback), this);

                NetworkAction?.StateChanged(State.Connecting, new StateObject(this));

                return;
            }
            catch (Exception)
            {
            }

            NetworkAction?.StateChanged(State.Error, new StateObject(this));
        }

        protected virtual void ConnectCallback(IAsyncResult ar)
        {
            var _obj = ar.AsyncState as ClientService;

            try
            {
                _obj?.NetworkSocket?.EndConnect(ar);

                _obj?.NetworkAction?.StateChanged(State.Connected, new StateObject(_obj));

                return;
            }
            catch (Exception)
            {
            }

            _obj?.NetworkAction?.StateChanged(State.Error, new StateObject(_obj));
        }

    }
}
