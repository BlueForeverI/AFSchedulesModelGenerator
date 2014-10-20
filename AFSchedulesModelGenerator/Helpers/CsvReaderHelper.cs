using AFSchedulesModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFSchedulesModelGenerator.Helpers
{
    public static class CsvReaderHelper
    {
        public static bool UseTaxonomy { get; set; }

        public static string FirstCharToUpper(this string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }

        static List<string[]> GetLinesFromFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            string[] values = content.Split(new string[] { ",", "\r\n" }, StringSplitOptions.None);
            int cellsPerRow = 12;
            int totalRows = values.Count() / cellsPerRow;

            var result = new List<string[]>();
            for (int i = 0; i < totalRows; i++)
            {
                result.Add(values.Skip(i * cellsPerRow).Take(cellsPerRow).ToArray());
            }

            return result;

        }

        static void SetPropertiesForClass(XmlClass xmlClass, string[] values, bool isRepeater)
        {
            if (values.Length == 3)
            {
                if (isRepeater)
                {
                    xmlClass.IsRepeatable = true;
                }

                if (xmlClass.Properties == null)
                {
                    xmlClass.Properties = new List<XmlProperty>();
                }

                xmlClass.Properties.Add(new XmlProperty()
                {
                    PropertyName = values[0],
                    Value = UseTaxonomy ? values[1] : values[2],              // TODO: revert back to 1, 2 is for calculations
                    IsRepeater = isRepeater
                });

                return;
            }
            else
            {
                string className = values[0];

                if (!string.IsNullOrEmpty(className))
                {
                    var fullName = xmlClass.ClassName + className.FirstCharToUpper();
                    if (xmlClass.Classes != null && xmlClass.Classes.ContainsKey(fullName))
                    {
                        var foundClass = xmlClass.Classes[fullName];
                        SetPropertiesForClass(foundClass, values.Skip(1).ToArray(), isRepeater);
                    }
                    else
                    {
                        if (xmlClass.Properties == null)
                        {
                            xmlClass.Properties = new List<XmlProperty>();
                        }

                        xmlClass.Properties.Add(new XmlProperty()
                        {
                            PropertyName = className,
                            IsClass = true
                        });

                        if (xmlClass.Classes == null)
                        {
                            xmlClass.Classes = new Dictionary<string, XmlClass>();
                        }

                        xmlClass.Classes.Add(fullName, new XmlClass()
                        {
                            ClassName = fullName
                        });

                        var foundClass = xmlClass.Classes[fullName];
                        SetPropertiesForClass(foundClass, values.Skip(1).ToArray(), isRepeater);
                    }
                }
                else
                {
                    SetPropertiesForClass(xmlClass, values.Skip(1).ToArray(), isRepeater);
                }
            }
        }

        public static string ConvertFileToCode(string filePath, string prefix, int columnStartIndex)
        {
            var fileContent = GetLinesFromFile(filePath);

            var rootClass = new XmlClass()
            {
                ClassName = prefix + fileContent.First()[columnStartIndex].FirstCharToUpper()
            };

            foreach (var line in fileContent)
            {
                var repeaterValue = line[0].Trim();
                bool isRepeater = (repeaterValue == "TRUE");
                SetPropertiesForClass(rootClass, line.Skip(3).ToArray(), isRepeater);
            }

            return rootClass.ToString();
        }
    }
}
