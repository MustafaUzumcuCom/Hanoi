/*
    Copyright (c) 2007 - 2010, Carlos Guzmán Álvarez

    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, 
          this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, 
          this list of conditions and the following disclaimer in the documentation and/or 
          other materials provided with the distribution.
        * Neither the name of the author nor the names of its contributors may be used to endorse or 
          promote products derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Hanoi.Xmpp.Serialization
{
    /// <summary>
    ///   Serializer class for XMPP stanzas
    /// </summary>
    public sealed class XmppSerializer
    {
        private static readonly string XmlSerializersResource = "BabelIm.Net.Xmpp.Serialization.Serializers.xml";

        private static readonly List<XmppSerializer> Serializers = new List<XmppSerializer>();
        private static bool Initialized;
        private static readonly object SyncObject = new object();
        private readonly XmlParserContext context;

        private readonly string defaultNamespace;
        private readonly string elementName;
        private readonly XmlNameTable nameTable;
        private readonly XmlSerializerNamespaces namespaces;
        private readonly XmlNamespaceManager nsMgr;
        private readonly string prefix;
        private readonly string schema;
        private readonly XmlSerializer serializer;
        private readonly Type serializerType;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppSerializer" /> class.
        /// </summary>
        /// <param name = "elementName">Name of the element.</param>
        /// <param name = "schema">The schema.</param>
        /// <param name = "prefix">The prefix.</param>
        /// <param name = "defaultNamespace">The default namespace.</param>
        /// <param name = "serializerType">Type of the serializer.</param>
        private XmppSerializer(string elementName, string schema, string prefix, string defaultNamespace, Type serializerType)
        {
            this.elementName = elementName;
            this.serializerType = serializerType;
            this.schema = schema;
            this.prefix = prefix;
            this.defaultNamespace = defaultNamespace;
            serializer = new XmlSerializer(serializerType);
            nameTable = new NameTable();
            nsMgr = new XmlNamespaceManager(nameTable);
            context = new XmlParserContext(nameTable, nsMgr, null, XmlSpace.None);
            namespaces = new XmlSerializerNamespaces();
            namespaces.Add(prefix, defaultNamespace);

            foreach (XmlQualifiedName name in namespaces.ToArray())
            {
                nsMgr.AddNamespace(name.Name, name.Namespace);
            }
        }

        /// <summary>
        ///   Gets the name of the element.
        /// </summary>
        /// <value>The name of the element.</value>
        public string ElementName
        {
            get { return elementName; }
        }

        /// <summary>
        ///   Gets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public string Schema
        {
            get { return schema; }
        }

        /// <summary>
        ///   Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix
        {
            get { return prefix; }
        }

        /// <summary>
        ///   Gets the default namespace.
        /// </summary>
        /// <value>The default namespace.</value>
        public string DefaultNamespace
        {
            get { return defaultNamespace; }
        }

        /// <summary>
        ///   Gets the type of the serializer.
        /// </summary>
        /// <value>The type of the serializer.</value>
        public Type SerializerType
        {
            get { return serializerType; }
        }

        /// <summary>
        ///   Serializes the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        public static byte[] Serialize(object value)
        {
            Initialize();

            return GetSerializer(value.GetType()).SerializeObject(value);
        }

        /// <summary>
        ///   Serializes the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "prefix">The prefix.</param>
        /// <returns></returns>
        public static byte[] Serialize(object value, string prefix)
        {
            Initialize();

            return GetSerializer(value.GetType()).SerializeObject(value);
        }

        /// <summary>
        ///   Deserializes the specified XML.
        /// </summary>
        /// <param name = "xml">The XML.</param>
        /// <returns></returns>
        public static object Deserialize(string nodeName, string xml)
        {
            Initialize();

            XmppSerializer serializer = GetSerializer(nodeName);

            if (serializer != null)
            {
                return serializer.Deserialize(xml);
            }

            return null;
        }

        /// <summary>
        ///   Initializes this instance.
        /// </summary>
        private static void Initialize()
        {
            lock (SyncObject)
            {
                if (!Initialized)
                {
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(XmlSerializersResource))
                    {
                        using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                        {
                            var xml = new XmlDocument();
                            xml.LoadXml(reader.ReadToEnd());

                            XmlNodeList list = xml.SelectNodes("/serializers/serializer");

                            foreach (XmlNode serializer in list)
                            {
                                XmlNode node = serializer.SelectSingleNode("namespace");

                                string ename = serializer.Attributes["elementname"].Value;
                                string schema = serializer.SelectSingleNode("schema").InnerText;
                                string prefix = node.SelectSingleNode("prefix").InnerText;
                                string nsName = node.SelectSingleNode("namespace").InnerText;
                                Type type = Type.GetType(serializer.SelectSingleNode("serializertype").InnerText);

                                Serializers.Add(new XmppSerializer(ename, schema, prefix, nsName, type));
                            }

                            Initialized = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Gets the serializer.
        /// </summary>
        /// <param name = "elementName">Name of the element.</param>
        /// <returns></returns>
        private static XmppSerializer GetSerializer(string elementName)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Serializers Factory not initialized");
            }

            foreach (XmppSerializer serializer in Serializers)
            {
                if (serializer.ElementName == elementName)
                {
                    return serializer;
                }
            }

            return null;
        }

        /// <summary>
        ///   Gets the serializer.
        /// </summary>
        /// <param name = "type">The type.</param>
        /// <returns></returns>
        private static XmppSerializer GetSerializer(Type type)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Serializers Factory not initialized");
            }

            return Serializers.Where(s => s.SerializerType == type).SingleOrDefault();
        }

        /// <summary>
        ///   Serializes the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns></returns>
        private byte[] SerializeObject(object value)
        {
            return SerializeObject(value, "");
        }

        /// <summary>
        ///   Serializes the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "prefix">The prefix.</param>
        /// <returns></returns>
        private byte[] SerializeObject(object value, string prefix)
        {
            byte[] result = null;
            MemoryStream ms = null;
            XmppTextWriter tw = null;

            try
            {
                ms = new MemoryStream();
                tw = new XmppTextWriter(ms);

                tw.QuoteChar = '\'';

                serializer.Serialize(tw, value, namespaces);

                result = ms.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                    ms = null;
                }
                if (tw != null)
                {
                    tw.Close();
                    tw = null;
                }
            }

            return result;
        }

        /// <summary>
        ///   Deserializes the specified XML.
        /// </summary>
        /// <param name = "xml">The XML.</param>
        /// <returns></returns>
        private object Deserialize(string xml)
        {
            using (var reader = new XmlTextReader(xml, XmlNodeType.Element, context))
            {
                return serializer.Deserialize(reader);
            }
        }
    }
}