using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using TcpStuff.Classes.Services;
using TcpStuff.Interfaces;

namespace TcpStuff.Classes.Messages
{
    [DataContract]
    public class TextMessage : XmlStorage<TextMessage>, IMessage
    {
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string To { get; set; }
        [DataMember]
        public string Text { get; set; }

        public TextMessage()
        {
            From = To = Text = string.Empty;
        }

        public override bool InitializeFromObject(TextMessage Object)
        {
            From = Object.From;
            To = Object.To;
            Text = Object.Text;

            return true;
        }
        public IMessage ProcessRequest(StateObject Object = null)
        {
            var _client = Object.GetObject<ClientService>();
            
            //tutaj kod obslugi po stronie serwera

            return null;
        }

        public IMessage ProcessResponse(StateObject Object = null)
        {
            //klient po prostu wyswietla wiadomosc :)

            Console.WriteLine(this);

            return this;
        }
        public NetworkData AsNetworkData(int a_iBufferSize = NetworkService.BUFFER_SIZE)
        {
            return new NetworkData(a_iBufferSize)
            {
                Buffer = ToXml().ToArray()
            };
        }
        public override string ToString()
        {
            return $"Od:{From}|Do:{To}|Wiadomosc={Text}";
        }
    }
}
