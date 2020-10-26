using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using TcpStuff.Classes.Exceptions;
using TcpStuff.Interfaces;

namespace TcpStuff.Classes
{
    public sealed class MessageFactory
    {
        public static readonly MessageFactory Instance = new MessageFactory();
        private readonly Dictionary<string, Type> m_oMessages;

        private MessageFactory()
        {
            m_oMessages = new Dictionary<string, Type>();
        }
        public void Register<T>() where T : class
        {
            Type _oType = typeof(T);

            string _sTypeName = _oType.Name;

            if (!_oType.GetInterfaces().ToArray().Contains(typeof(IMessage)))
            {
                throw new MessageFactoryIfaceNotFound(_sTypeName, "IMessage");
            }

            if (!m_oMessages.ContainsKey(_sTypeName))
            {
                m_oMessages.Add(_sTypeName, _oType);
            }            
        }

        public T Create<T>(string a_sTypeName) where T : class
        {
            if (m_oMessages.ContainsKey(a_sTypeName))
            {
                return (T)Activator.CreateInstance(m_oMessages[a_sTypeName]);
            }
            throw new MessageFactoryTypeNotFound(a_sTypeName);
        }

        public dynamic Create(string a_sTypeName) => Create<dynamic>(a_sTypeName); 

        public dynamic Create(byte[] a_oData)
        {
            var _oXml = XmlReader.Create(new MemoryStream(a_oData));

            _oXml.Read();

            string _sTypeName = _oXml.Name;

            var _oMsg = Create<IXmlStorage>(_sTypeName);

            _oMsg.FromXml(new MemoryStream(a_oData));

            return _oMsg;
        }
    }
}
