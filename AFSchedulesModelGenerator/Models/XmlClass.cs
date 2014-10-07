using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AFSchedulesModelGenerator.Helpers;

namespace AFSchedulesModelGenerator.Models
{
    public class XmlClass
    {
        public string ClassName { get; set; }

        public List<XmlProperty> Properties { get; set; }

        [XmlIgnore]
        public Dictionary<string, XmlClass> Classes { get; set; }

        public List<XmlClass> XmlClasses
        {
            get
            {
                return (Classes != null && Classes.Count > 0) ? Classes.Values.ToList() : null;
            }
        }

        private static System.Xml.Serialization.XmlSerializer serializer;
        private static System.Xml.Serialization.XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(typeof(XmlClass));
                }
                return serializer;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(string.Format("public partial class {0} : EntityBase<{0}> \r\n{{\r\n", ClassName.Trim()));

            builder.Append("private FSBDigitalSubmissionsData _fsbData;\r\n");

            builder.AppendFormat("\r\n#region Constructors\r\n");

            builder.Append(string.Format("public {0}()\r\n{{\r\n", ClassName));
            builder.Append("\r\n}\r\n\r\n");

            builder.Append(string.Format("public {0}(FSBDigitalSubmissionsData fsbData)\r\n{{\r\nthis._fsbData = fsbData;", ClassName));
            builder.Append("\r\n}\r\n");

            builder.AppendFormat("#endregion\r\n");

            foreach (var property in Properties)
            {
                if (property.IsClass)
                {
                    builder.Append(string.Format("\r\nprivate {0} {1}Field;\r\npublic {0} {1} \r\n{{\r\n get \r\n{{\r\n return new {0}(_fsbData); \r\n}}\r\n set\r\n{{\r\n{1}Field = value;\r\n}} \r\n}}\r\n",
                        ClassName + property.PropertyName.FirstCharToUpper(),
                        property.PropertyName));
                }
                else
                {

                    builder.Append(property.ToString());
                }
            }



            builder.Append("}\r\n\r\n");

            if (Classes != null && Classes.Count > 0)
            {
                foreach (var classItem in XmlClasses)
                {
                    builder.Append(classItem.ToString());
                }
            }

            return builder.ToString();
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
