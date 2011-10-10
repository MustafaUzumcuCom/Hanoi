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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BabelIm.Configuration {
    [Serializable]
    [XmlRoot(ElementName = "babelim", IsNullable = false)]
    public sealed class BabelImConfiguration {
        private static readonly string ConfigurationFilename = "BabelIm.xml";

        private static XmlSerializer Serializer = new XmlSerializer(typeof (BabelImConfiguration));
        private Accounts accounts;

        private GeneralConfiguration general;
        private Notifications notifications;
        private Servers servers;

        [XmlElement(Type = typeof (GeneralConfiguration), ElementName = "general", IsNullable = false,
            Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public GeneralConfiguration General {
            get {
                if (general == null)
                {
                    general = new GeneralConfiguration();
                }
                return general;
            }
            set { general = value; }
        }

        [XmlElement(Type = typeof (Notifications), ElementName = "notifications", IsNullable = false,
            Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Notifications Notifications {
            get {
                if (notifications == null)
                {
                    notifications = new Notifications();
                }
                return notifications;
            }
            set { notifications = value; }
        }

        [XmlElement(Type = typeof (Servers), ElementName = "servers", IsNullable = false, Form = XmlSchemaForm.Qualified
            )]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Servers Servers {
            get {
                if (servers == null)
                {
                    servers = new Servers();
                }
                return servers;
            }
            set { servers = value; }
        }

        [XmlElement(Type = typeof (Accounts), ElementName = "accounts", IsNullable = false,
            Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Accounts Accounts {
            get {
                if (accounts == null)
                {
                    accounts = new Accounts();
                }
                return accounts;
            }
            set { accounts = value; }
        }

        public static BabelImConfiguration GetConfiguration() {
            String appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return GetConfiguration(String.Format("{0}\\{1}", Path.GetDirectoryName(appPath), ConfigurationFilename));
        }

        public static BabelImConfiguration GetConfiguration(string configurationFilename) {
            FileStream stream = null;
            BabelImConfiguration configuration = null;

            if (!Directory.Exists(Path.GetPathRoot(configurationFilename)))
            {
                Directory.CreateDirectory(Path.GetPathRoot(configurationFilename));
            }
            if (!File.Exists(configurationFilename))
            {
                var tmpCfg = new BabelImConfiguration();

                tmpCfg.Save(configurationFilename);
            }

            try
            {
                stream = new FileStream(configurationFilename, FileMode.Open, FileAccess.Read);
                configuration = (BabelImConfiguration) Serializer.Deserialize(stream);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return configuration;
        }

        public void Save() {
            String appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            Save(String.Format("{0}\\{1}", Path.GetDirectoryName(appPath), ConfigurationFilename));
        }

        public void Save(string filename) {
            var stream = new FileStream(filename, FileMode.Create, FileAccess.Write);
            var xmlWriter = new XmlTextWriter(stream, Encoding.UTF8);

            try
            {
                // Writer settings
                xmlWriter.QuoteChar = '\'';
                xmlWriter.Formatting = Formatting.Indented;

                Serializer.Serialize(xmlWriter, this);
            }
            catch
            {
                throw;
            }
            finally
            {
                xmlWriter.Close();
                stream.Close();
            }
        }
    }
}