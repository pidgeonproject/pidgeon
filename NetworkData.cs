using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class NetworkData
    {
        public class NetworkInfo
        {
            public string Name = null;
            public string Server = null;
            public bool SSL = false;
            public ProtocolType protocolType = ProtocolType.IRC;

            /// <summary>
            /// Constructor for XML
            /// </summary>
            public NetworkInfo()
            {
                // keep this
            }
        }

        public static List<NetworkInfo> Networks = new List<NetworkInfo>();

        public enum ProtocolType
        { 
            IRC,
            Services,
            Quassel,
            XMPP,
        }
    }
}
