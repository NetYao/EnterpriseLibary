using System;
using System.Text;
using System.Threading;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 随机数
    /// </summary>
    public class RandomHelper
    {
        private readonly static Random Random = new Random();//随机发生器
        private readonly static char[] Randomlibrary = new char[]{ '1','2','3','4','5','6','7','8','9',
                                                    'a','b','c','d','e','f','g',
                                                    'h',    'j','k',    'm','n',
                                                        'p','q',    'r','s','t',
                                                    'u','v','w',    'x','y'
                                                  };//随机库

        

        /// <summary>
        /// 创建随机值
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="onlyNumber">是否只包含数字</param>
        /// <returns>随机值</returns>
        public static string CreateRandomValue(int length, bool onlyNumber)
        {
            var randomValue = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = Random.Next(0, onlyNumber ? 9 : Randomlibrary.Length);

                randomValue.Append(Randomlibrary[index]);
            }

            return randomValue.ToString();
        }


        private static long _sequence = DateTime.UtcNow.Ticks;

        /// <summary>
        /// 生成不会重复的随机数
        /// </summary>
        /// <returns></returns>
        public static long GenerateUniqueId()
        {
            return Interlocked.Increment(ref _sequence);
        }
    }
}
