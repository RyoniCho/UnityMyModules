using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace ControlRoom
{
    public class Config
    {

        //Default Value
        static Config()
        {
            Config.Version = "1.0.0";
            Config.VersionCode = "1";
            Config.BuildPath = $"{Application.dataPath}/../Build/";
            Config.useAppBundle = true;
            Config.isDebugMode = false;
            Config.androidKeyStoreName= UnityEngine.Application.dataPath + "/controlRoom.keystore";
            Config.androidKeyAliasName = "controlRoom";
            Config.androidKeyStorePassword = "controlRoom0819";
            Config.androidKeyAliasPassword = "controlRoom0819";
            Config.MarketURL_AOS = "";
            Config.MarketURL_iOS = "";
            Config.isIOSSimulatorBuild = false;
            Config.iOSMinimalOSVersionString = "10.0";
        }

        public static string Version { get; set; }
        public static string VersionCode { get; set; }
        public static string BuildPath { get; set; }
        public static bool useAppBundle { get; set;  }
        public static bool isDebugMode { get; set; }
        public static string androidKeyStoreName { get; set;}
        public static string androidKeyAliasName { get; set; }
        public static string androidKeyStorePassword { get; set; }
        public static string androidKeyAliasPassword { get; set; }
        public static string MarketURL_AOS { get; set; }
        public static string MarketURL_iOS { get; set; }
        public static bool isIOSSimulatorBuild { get; set; }
        public static string iOSMinimalOSVersionString { get; set; }





        public static XmlDocument ReadXml()
        {

            UnityEngine.TextAsset xmlText = Resources.Load("Config") as TextAsset;
#if UNITY_EDITOR
            if (xmlText == null)
            {
                WriteXml();
                UnityEditor.AssetDatabase.Refresh();
                xmlText = Resources.Load("Config") as TextAsset;

            }
#endif
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlText.text);

            XmlNode rootNode = xmlDocument["Root"];

            for (XmlNode node = rootNode.FirstChild; node != null; node = node.NextSibling)
            {
                if (node.Name.Equals("Version"))
                {
                    Config.Version = node.Attributes["Value"].InnerText;

                }

                if (node.Name.Equals("VersionCode"))
                {
                    Config.VersionCode = node.Attributes["Value"].InnerText;

                }
                if (node.Name.Equals("BuildPath"))
                {
                    Config.BuildPath = node.Attributes["Value"].InnerText;

                }
                if (node.Name.Equals("useAppBundle"))
                {
                    bool value;
                    bool.TryParse(node.Attributes["Value"].InnerText, out value);
                    Config.useAppBundle = value;
                }
                if (node.Name.Equals("isDebugMode"))
                {
                    bool value;
                    bool.TryParse(node.Attributes["Value"].InnerText, out value);
                    Config.isDebugMode = value;
                }

                if (node.Name.Equals("androidKeyStoreName"))
                {
                    Config.androidKeyStoreName = node.Attributes["Value"].InnerText;

                }
                if (node.Name.Equals("androidKeyAliasName"))
                {
                    Config.androidKeyAliasName = node.Attributes["Value"].InnerText;

                }
                if (node.Name.Equals("androidKeyStorePassword"))
                {
                    Config.androidKeyStorePassword = node.Attributes["Value"].InnerText;

                }
                if (node.Name.Equals("androidKeyAliasPassword"))
                {
                    Config.androidKeyAliasPassword = node.Attributes["Value"].InnerText;

                }

                if (node.Name.Equals("MarketURL_AOS"))
                {
                    Config.MarketURL_AOS = node.Attributes["Value"].InnerText;

                }
                if (node.Name.Equals("MarketURL_iOS"))
                {
                    Config.MarketURL_iOS = node.Attributes["Value"].InnerText;

                }
                if(node.Name.Equals("isIOSSimulatorBuild"))
                {
                    bool value;
                    bool.TryParse(node.Attributes["Value"].InnerText, out value);
                    Config.isIOSSimulatorBuild = value;
                }
                if (node.Name.Equals("iOSMinimalOSVersionString"))
                {
                    Config.iOSMinimalOSVersionString = node.Attributes["Value"].InnerText;

                }

            }

            return xmlDocument;

        }

        public static void WriteXml()
        {
            
            var path = System.IO.Directory.GetCurrentDirectory();
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            XmlWriter xmlWriter = XmlWriter.Create(path + "/Assets/Resources/Config.xml", settings);

            xmlWriter.WriteStartElement("Root");

            xmlWriter.WriteStartElement("Version");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.Version);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("VersionCode");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.VersionCode);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("BuildPath");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.BuildPath);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("useAppBundle");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.useAppBundle.ToString().ToLower());
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("isDebugMode");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.isDebugMode.ToString().ToLower());
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();


            xmlWriter.WriteStartElement("androidKeyStoreName");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.androidKeyStoreName);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("androidKeyAliasName");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.androidKeyAliasName);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("androidKeyStorePassword");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.androidKeyStorePassword);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("androidKeyAliasPassword");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.androidKeyAliasPassword);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("MarketURL_AOS");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.MarketURL_AOS);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("MarketURL_iOS");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.MarketURL_iOS);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("isIOSSimulatorBuild");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.isIOSSimulatorBuild.ToString().ToLower());
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("iOSMinimalOSVersionString");
            xmlWriter.WriteStartAttribute("Value");
            xmlWriter.WriteValue(Config.iOSMinimalOSVersionString);
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();


            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            xmlWriter.Close();

        }
    }
}

