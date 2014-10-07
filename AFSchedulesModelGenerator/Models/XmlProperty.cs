using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFSchedulesModelGenerator.Models
{
    public class XmlProperty
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public bool IsRepeater { get; set; }
        public bool IsClass { get; set; }

        private static System.Xml.Serialization.XmlSerializer serializer;
        private static System.Xml.Serialization.XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(typeof(XmlProperty));
                }
                return serializer;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(PropertyName) || string.IsNullOrWhiteSpace(PropertyName))
            {
                return "";
            }

            if (string.IsNullOrEmpty(Value) || string.IsNullOrWhiteSpace(Value))
            {
                return string.Format("\r\nprivate string {0}Field;\r\npublic string {0}\r\n{{\r\n get \r\n{{\r\n return string.Empty; \r\n}}\r\n set\r\n{{\r\n{0}Field = value;\r\n}} \r\n}}\r\n",
                PropertyName);
            }

            if (Value.StartsWith(" -"))
            {
                string negatedValue = Value.Replace(" -", String.Empty);
                return string.Format("\r\nprivate string {0}Field;\r\npublic string {0}\r\n{{\r\n get \r\n{{\r\n return (-_fsbData.Investments.{1}).ToString(); \r\n}}\r\n set\r\n{{\r\n{0}Field = value;\r\n}} \r\n}}\r\n",
                PropertyName, negatedValue);
            }

            if (IsRepeater)
            {
                return string.Format("\r\nprivate string {0}Field;\r\npublic string {0}\r\n{{\r\n get \r\n{{\r\n return _item.{1}.ToString(); \r\n}}\r\n set\r\n{{\r\n{0}Field = value;\r\n}} \r\n}}\r\n",
                PropertyName, Value);
            }

            return string.Format("\r\nprivate string {0}Field;\r\npublic string {0}\r\n{{\r\n get \r\n{{\r\n return _fsbData.Investments.{1}.ToString(); \r\n}}\r\n set\r\n{{\r\n{0}Field = value;\r\n}} \r\n}}\r\n",
                PropertyName, Value);
        }

        /// <summary>
        /// Serializes current schedulesGeneralInfo object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize()
        {
            System.IO.StreamReader streamReader = null;
            System.IO.MemoryStream memoryStream = null;
            try
            {
                memoryStream = new System.IO.MemoryStream();
                Serializer.Serialize(memoryStream, this);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                streamReader = new System.IO.StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }
    }
}
