/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/

using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class ProtocolSv : Protocol
    {
        public class Datagram
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="Name">Name of a datagram</param>
            /// <param name="Text">Value</param>
            public Datagram(string Name, string Text = "")
            {
                _Datagram = Name;
                _InnerText = Text;
            }

            public static Datagram LoadXML(string xml)
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(xml);
                Datagram datagram = new Datagram(data.DocumentElement.Name, data.DocumentElement.InnerText);
                foreach (XmlAttribute curr in data.DocumentElement.Attributes)
                {
                    datagram.Parameters.Add(curr.Name, curr.Value);
                }

                return datagram;
            }

            public static Datagram LoadXML(XmlNode xml)
            {
                Datagram datagram = new Datagram(xml.Name, xml.InnerText);
                foreach (XmlAttribute curr in xml.Attributes)
                {
                    datagram.Parameters.Add(curr.Name, curr.Value);
                }

                return datagram;
            }

            public string ToDocumentXmlText()
            {
                XmlDocument datagram = new XmlDocument();
                XmlNode b1 = datagram.CreateElement(_Datagram.ToUpper());
                foreach (KeyValuePair<string, string> curr in Parameters)
                {
                    XmlAttribute b2 = datagram.CreateAttribute(curr.Key);
                    b2.Value = curr.Value;
                    b1.Attributes.Append(b2);
                }
                b1.InnerText = this._InnerText;
                datagram.AppendChild(b1);
                return datagram.InnerXml;
            }

            public string _InnerText;
            public string _Datagram;
            public Dictionary<string, string> Parameters = new Dictionary<string, string>();
        }
    }
}
