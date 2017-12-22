using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Enterprises.Framework.Plugin.Config.BconConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.Plugin.Config
{
    [TestClass]
    public class BconConfigTest
    {
        /// <summary>
        /// Test config stream
        /// Data is:
        ///         // This is comment
        ///         [Property1]
        ///         123.4
        ///         abc 
        ///         [Property2]
        ///         32
        /// </summary>
        [TestMethod]
        public void TestStreamMethod()
        {
            const string Config = "// This is comment\n[Property1]\n123.4\nabc\n[Property2]\n32";
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(Config);
                    writer.Flush();

                    stream.Seek(0, SeekOrigin.Begin);

                    // Test
                    dynamic obj = stream.ParseBCON(new BCONConfig("//"));

                    Assert.AreEqual(obj.Property1[0], 123.4d);
                    Assert.AreEqual(obj.Property1[1], "abc");
                    Assert.AreEqual(obj.Property2, 32);
                }
            }
        }

        /// <summary>
        /// Test config file stream
        /// </summary>
        [TestMethod]
        public void TestFileStreamMethod()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            string codeBaseUri = new UriBuilder(codeBase).Path;
            string executingDir = Path.GetDirectoryName(Uri.UnescapeDataString(codeBaseUri));
            string filePath = Path.Combine(executingDir, "TestData.txt");

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                // Test
                dynamic obj = BCONParser.ParseBCON(
                    stream,
                    new BCONConfig(
                        propertySeperator: new[] { ',', '|' },
                        valueSeperator: new[] { '=', ':' }));

                Assert.AreEqual(obj.Persons[0].Name, "John William");
                Assert.AreEqual(obj.Persons[0].Age, 23);
                Assert.AreEqual(obj.Persons[0].Country, "US");

                Assert.AreEqual(obj.Persons[1].Name, "Le Seng");
                Assert.AreEqual(obj.Persons[1].Age, 32);
                Assert.AreEqual(obj.Persons[1].Country, "CN");
                Assert.AreEqual(obj.Persons[1].Company, "MS");

                Assert.AreEqual(obj.Products[0], "Office 2013");
                Assert.AreEqual(obj.Products[1], "Windows 8.1");
                Assert.AreEqual(obj.Products[2], "Windows Phone 8.1");
            }
        }

        /// <summary>
        /// Test config string
        /// Data is:
        ///         # This is comment
        ///         {Property1}
        ///         123.4
        ///         abc 
        ///         {Property2}
        ///         32
        /// </summary>
        [TestMethod]
        public void TestStringMethod()
        {
            string config = "# This is comment\n{Property1}\n123.4\nabc\n{Property2}\n32";

            // Test
            dynamic obj = config.ParseBCON(new BCONConfig(propertyStartChar: "{", propertyEndChar: "}"));

            Assert.AreEqual(obj.Property1[0], 123.4d);
            Assert.AreEqual(obj.Property1[1], "abc");
            Assert.AreEqual(obj.Property2, 32);
        }

        /// <summary>
        /// Test config string
        /// Data is:
        ///         # This is comment
        ///         [Property1]
        ///         A=123.4
        ///         B=abc 
        ///         [Property2]
        ///         C=D=32
        /// </summary>
        [TestMethod]
        public void TestComplexStringMethod()
        {
            string config = "# This is comment\n[Property1]\nA=123.4\nB=abc\n[Property2]\nC=23,D=32";

            // Test
            dynamic obj = BCONParser.ParseBCON(config);

            Assert.AreEqual(obj.Property1[0].A, 123.4d);
            Assert.AreEqual(obj.Property1[1].B, "abc");
            Assert.AreEqual(obj.Property2.C, 23);
            Assert.AreEqual(obj.Property2.D, 32);
        }

        [TestMethod]
        public void TestInvalidData()
        {
            string config = "Ignore this\n{Property1}\n123.4\nabc\n[Property2]\n32";

            // Test
            dynamic obj = BCONParser.ParseBCON(config, new BCONConfig(propertyStartChar: "{", propertyEndChar: "}"));

            Assert.AreEqual(obj.Property1[0], 123.4d);
            Assert.AreEqual(obj.Property1[1], "abc");
            Assert.AreEqual(obj.Property1[2], "[Property2]");
            Assert.AreEqual(obj.Property1[3], 32);

            config = "Ignore this";

            obj = BCONParser.ParseBCON(config, new BCONConfig(propertyStartChar: "{", propertyEndChar: "}"));
            Assert.IsNotNull(obj);
            Assert.AreEqual((obj as IDictionary<string, dynamic>).Count, 0);
        }
    }
}
