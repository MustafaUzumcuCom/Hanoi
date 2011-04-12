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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization
{
    /// <summary>
    /// Serializer class for XMPP stanzas
    /// </summary>
    public sealed class XmppSerializer
    {
        #region · Static Fields ·

        private static readonly string XmlSerializersResource = "BabelIm.Net.Xmpp.Serialization.Serializers.xml";

        private static List<XmppSerializer> Serializers = new List<XmppSerializer>();
        private static bool                 Initialized = false;
        private static object				SyncObject	= new object();

        #endregion

        #region · Static Serialization/Deserialization Methods ·

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static byte[] Serialize(object value)
        {
            Initialize();

            return GetSerializer(value.GetType()).SerializeObject(value);
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public static byte[] Serialize(object value, string prefix)
        {
            Initialize();

            return GetSerializer(value.GetType()).SerializeObject(value);
        }

        /// <summary>
        /// Deserializes the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
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

        #endregion

        #region · Static Methods ·

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        static void Initialize()
        {
            lock (SyncObject)
            {
                if (!Initialized)
                {
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(XmlSerializersResource))
                    {
                        using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                        {
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(reader.ReadToEnd());
    
                            XmlNodeList list = xml.SelectNodes("/serializers/serializer");
    
                            foreach (XmlNode serializer in list)
                            {
                                XmlNode node = serializer.SelectSingleNode("namespace");
    
                                string 	ename	= serializer.Attributes["elementname"].Value;
                                string 	schema 	= serializer.SelectSingleNode("schema").InnerText;
                                string 	prefix 	= node.SelectSingleNode("prefix").InnerText;
                                string 	nsName 	= node.SelectSingleNode("namespace").InnerText;
                                Type 	type 	= Type.GetType(serializer.SelectSingleNode("serializertype").InnerText);
    
                                Serializers.Add(new XmppSerializer(ename, schema, prefix, nsName, type));
                            }
    
                            Initialized = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        static XmppSerializer GetSerializer(string elementName)
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
        /// Gets the serializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        static XmppSerializer GetSerializer(Type type)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Serializers Factory not initialized");
            }

            return Serializers.Where(s => s.SerializerType == type).SingleOrDefault();
        }

        #endregion

        #region · Fields ·

        private string					elementName;
        private string					schema;
        private string					prefix;
        private string					defaultNamespace;
        private Type					serializerType;
        private XmlSerializerNamespaces namespaces;
        private XmlSerializer			serializer;
        private XmlNameTable			nameTable;
        private XmlNamespaceManager		nsMgr;
        private XmlParserContext		context;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>The name of the element.</value>
        public string ElementName
        {
            get { return this.elementName; }
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public string Schema
        {
            get { return this.schema; }
        }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix
        {
            get { return this.prefix; }
        }

        /// <summary>
        /// Gets the default namespace.
        /// </summary>
        /// <value>The default namespace.</value>
        public string DefaultNamespace
        {
            get { return this.defaultNamespace; }
        }

        /// <summary>
        /// Gets the type of the serializer.
        /// </summary>
        /// <value>The type of the serializer.</value>
        public Type SerializerType
        {
            get { return this.serializerType; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppSerializer"/> class.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <param name="serializerType">Type of the serializer.</param>
        private XmppSerializer(string elementName, string schema, string prefix, string defaultNamespace, Type serializerType)
        {
            this.elementName		= elementName;
            this.serializerType		= serializerType;
            this.schema				= schema;
            this.prefix				= prefix;
            this.defaultNamespace	= defaultNamespace;
            this.serializer         = new XmlSerializer(serializerType);
            this.nameTable			= new NameTable();
            this.nsMgr				= new XmlNamespaceManager(this.nameTable);
            this.context			= new XmlParserContext(this.nameTable, this.nsMgr, null, XmlSpace.None);
            this.namespaces			= new XmlSerializerNamespaces();
            this.namespaces.Add(prefix, defaultNamespace);

            foreach (XmlQualifiedName name in this.namespaces.ToArray())
            {
                this.nsMgr.AddNamespace(name.Name, name.Namespace);
            }
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private byte[] SerializeObject(object value)
        {
            return this.SerializeObject(value, "");
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        private byte[] SerializeObject(object value, string prefix)
        {
            byte[]		    result	= null;
            MemoryStream	ms		= null;
            XmppTextWriter	tw		= null;

            try
            {
                ms		= new MemoryStream();
                tw		= new XmppTextWriter(ms);
        
                tw.QuoteChar = '\'';

                serializer.Serialize(tw, value, this.namespaces);

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
        /// Deserializes the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        private object Deserialize(string xml)
        {
            using (XmlTextReader reader = new XmlTextReader(xml, XmlNodeType.Element, this.context))
            {
                return this.serializer.Deserialize(reader);
            }
        }

        #endregion
    }
}
