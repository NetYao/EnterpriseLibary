using System.Diagnostics;
using Enterprises.Framework.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Enterprises.Test
{
    
    
    /// <summary>
    ///这是 FileSystemStorageProviderTest 的测试类，旨在
    ///包含所有 FileSystemStorageProviderTest 单元测试
    ///</summary>
    [TestClass()]
    public class FileSystemStorageProviderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            var myWriter = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(myWriter);
        }
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///CreateFolder 的测试
        ///</summary>
        [TestMethod()]
        public void CreateFolderTest()
        {
            var target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string path = "D:/yaolifeng/test"; // TODO: 初始化为适当的值
            target.CreateFolder(path);
        }

        /// <summary>
        ///DeleteFile 的测试
        ///</summary>
        [TestMethod()]
        public void DeleteFileTest()
        {
            FileSystemStorageProvider target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string path = "D:/yaolifeng/delet/1.txt"; // TODO: 初始化为适当的值
            target.DeleteFile(path);
        }

        /// <summary>
        ///DeleteFolder 的测试
        ///</summary>
        [TestMethod()]
        public void DeleteFolderTest()
        {
            FileSystemStorageProvider target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string path = "D:/yaolifeng/1"; // TODO: 初始化为适当的值
            target.DeleteFolder(path);
        }

        /// <summary>
        ///FileExists 的测试
        ///</summary>
        [TestMethod()]
        public void FileExistsTest()
        {
            FileSystemStorageProvider target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string path = "D:/yaolifeng/delet/2.txt"; // TODO: 初始化为适当的值
            bool expected = true; // TODO: 初始化为适当的值
            bool actual;
            actual = target.FileExists(path);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///FolderExists 的测试
        ///</summary>
        [TestMethod()]
        public void FolderExistsTest()
        {
            FileSystemStorageProvider target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string path = "D:/yaolifeng/test"; // TODO: 初始化为适当的值
            bool expected = true; // TODO: 初始化为适当的值
            bool actual;
            actual = target.FolderExists(path);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///FolderExists 的测试
        ///</summary>
        [TestMethod()]
        public void FolderExistsTest2()
        {
            var target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            const string path = "D:/yaolifeng/test2"; // TODO: 初始化为适当的值
            const bool expected = false; // TODO: 初始化为适当的值
            bool actual = target.FolderExists(path);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///RenameFile 的测试
        ///</summary>
        [TestMethod()]
        public void RenameFileTest()
        {
            FileSystemStorageProvider target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string oldPath = "D:/yaolifeng/delet/222.txt"; // TODO: 初始化为适当的值
            string newPath = "D:/yaolifeng/delet/333.txt"; // TODO: 初始化为适当的值
            target.RenameFile(oldPath, newPath);
        }

        /// <summary>
        ///RenameFolder 的测试
        ///</summary>
        [TestMethod()]
        public void RenameFolderTest()
        {
            FileSystemStorageProvider target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string oldPath = "D:/yaolifeng/rename"; // TODO: 初始化为适当的值
            string newPath = "D:/yaolifeng/rename22"; // TODO: 初始化为适当的值
            target.RenameFolder(oldPath, newPath);
        }


        /// <summary>
        ///GetFile 的测试(姚立峰)
        ///</summary>
        [TestMethod()]
        public void GetFileTest()
        {
            FileSystemStorageProvider target = new FileSystemStorageProvider(); // TODO: 初始化为适当的值
            string path = "D:/yaolifeng/delet/333.txt";
            var filePri=target.GetFile(path);
            var name = filePri.GetName();
            var site = filePri.GetSize();
            TestContext.WriteLine(string.Format("{0},{1}", name, site));
        }
    }
}
