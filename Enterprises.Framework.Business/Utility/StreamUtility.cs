using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 流处理通用类
    /// </summary>
    public class StreamUtility
    {
        /// <summary>
        /// 把流中内容转换写入到byte[]数组中并返回（先把流的位置设置为起始）
        /// 为了节省内存，我们一个字节一个字节的读取。这样写的目的主要是有些流不支持已开始获取长度。只能读到哪儿算哪儿。
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] Stream2Bytes(Stream stream)
        {
            if (!stream.CanRead)
            {
                return null;
            }

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var bytes = new List<byte>();
            while (true)
            {
                int singleByte = stream.ReadByte();
                if (singleByte == -1)
                {
                    break;
                }
                bytes.Add((byte) singleByte);
            }

            return bytes.ToArray();

        }

        /// <summary>
        /// 在内存创建流对象，并把字节数组传递给他
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream Bytes2Stream(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new Exception("Bytes2Stream函数中的参数bytes不能为null");
            }

            Stream s = new MemoryStream(bytes);
            return s;
        }

        /// <summary>
        /// 获取流的内容
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadStream(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 获取流的内容
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadStream(Stream stream, System.Text.Encoding encoding)
        {
            using (var sr = new StreamReader(stream, encoding))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 把二进制数组读取成为字符串
        /// </summary>
        /// <param name="bytes"></param> 
        /// <returns></returns>
        public static string ReadBytes(byte[] bytes)
        {
            using (Stream stream = Bytes2Stream(bytes))
            {
                return ReadStream(stream);
            }
        }

        /// <summary>
        /// 把二进制数组读取成为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadBytes(byte[] bytes, System.Text.Encoding encoding)
        {
            using (Stream stream = Bytes2Stream(bytes))
            {
                return ReadStream(stream, encoding);
            }
        }

        /// <summary>
        ///  把流中内容转换为Bitmap
        /// </summary>
        /// <param name="inputStream"></param>
        public static Bitmap Stream2Bitmap(Stream inputStream)
        {
            var img = new Bitmap(inputStream);
            return img;
        }

        /// <summary>
        /// 字符串转Stream
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Stream String2Stream(string str)
        {
            Stream s = new MemoryStream(Encoding.Default.GetBytes(str));
            return s;
        }

        /// <summary>
        /// 字符串转Stream
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Stream String2Stream(string str, Encoding encoding)
        {
            Stream s = new MemoryStream(encoding.GetBytes(str));
            return s;
        }
    }
}

