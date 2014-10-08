using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AFSchedulesModelGenerator.Helpers;
using System.Text.RegularExpressions;

namespace AFSchedulesModelGenerator.Models
{
    public class XmlClass
    {
        public string ClassName { get; set; }
        public bool IsRepeatable { get; set; }

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

            if (IsRepeatable)
            {
                var dataItemName = Properties.FirstOrDefault(p => p.IsRepeater).Value;
                var lastMatch = Regex.Match(dataItemName, "[A-Z][a-z0-9]+$", RegexOptions.RightToLeft).Value;
                dataItemName = dataItemName.Substring(0, dataItemName.Length - lastMatch.Length);
                builder.Append(string.Format("private {0}Data _item;\r\n", dataItemName));
            }

            builder.AppendFormat("\r\n#region Constructors\r\n");

            builder.Append(string.Format("public {0}()\r\n{{\r\n", ClassName));
            builder.Append("\r\n}\r\n\r\n");

            builder.Append(string.Format("public {0}(FSBDigitalSubmissionsData fsbData)\r\n{{\r\nthis._fsbData = fsbData;", ClassName));
            builder.Append("\r\n}\r\n");

            if (IsRepeatable)
            {
                var dataItemName = Properties.FirstOrDefault(p => p.IsRepeater).Value;
                var lastMatch = Regex.Match(dataItemName, "[A-Z][a-z0-9]+$", RegexOptions.RightToLeft).Value;
                dataItemName = dataItemName.Substring(0, dataItemName.Length - lastMatch.Length);

                builder.Append(string.Format("public {0}({1}Data item)\r\n{{\r\nthis._item = item;", ClassName, dataItemName));
                builder.Append("\r\n}\r\n");
            }

            builder.AppendFormat("#endregion\r\n");

            foreach (var property in Properties)
            {
                if (property.IsClass)
                {
                    string fullClassName = ClassName + property.PropertyName.FirstCharToUpper();
                    if (Classes.ContainsKey(fullClassName))
                    {
                        var propAsClass = Classes[fullClassName];
                        if (propAsClass.IsRepeatable)
                        {

                            builder.Append(string.Format("\r\nprivate List<{0}> {1}Field;\r\n[System.Xml.Serialization.XmlElement(\"{1}\")]\r\npublic List<{0}> {1}s \r\n{{\r\n get \r\n{{\r\n ",
                                fullClassName,
                                property.PropertyName));

                            var dataListValue = propAsClass.Properties.FirstOrDefault(p => p.IsRepeater).Value;
                            var lastMatch = Regex.Match(dataListValue, "[A-Z][a-z0-9]+$", RegexOptions.RightToLeft).Value;
                            dataListValue = dataListValue.Substring(0, dataListValue.Length - lastMatch.Length);

                            builder.AppendFormat("return _fsbData.Investments.{0}DataList\r\n.Select(item =>\r\n new {1}(item))\r\n.ToList(); \r\n}}\r\n", dataListValue, fullClassName);

                            builder.AppendFormat(" set\r\n{{\r\n{1}Field = value;\r\n}} \r\n}}\r\n", fullClassName, property.PropertyName);
                        }
                        else
                        {
                            builder.Append(string.Format("\r\nprivate {0} {1}Field;\r\npublic {0} {1} \r\n{{\r\n get \r\n{{\r\n return new {0}(_fsbData); \r\n}}\r\n set\r\n{{\r\n{1}Field = value;\r\n}} \r\n}}\r\n",
                                ClassName + property.PropertyName.FirstCharToUpper(),
                                property.PropertyName));
                        }
                    }
                    else
                    {
                        builder.Append(string.Format("\r\nprivate {0} {1}Field;\r\npublic {0} {1} \r\n{{\r\n get \r\n{{\r\n return new {0}(_fsbData); \r\n}}\r\n set\r\n{{\r\n{1}Field = value;\r\n}} \r\n}}\r\n",
                            ClassName + property.PropertyName.FirstCharToUpper(),
                            property.PropertyName));
                    }
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
