using System;
using System.Collections.Generic;

namespace Enterprises.Test.Plugin.Excel
{
    public class SchoolLevel
    {
        public string LevelName { get; set; }

        public int ClassCount { get; set; }

        public string Master { get; set; }

        public static IEnumerable<SchoolLevel> GetList()
        {
            for (int i = 1; i <= 9; i++)
            {
                yield return new SchoolLevel() { LevelName = i.ToString() + "年级", ClassCount = (i + new Random().Next(1, 8)) * 5, Master = "牛" + i.ToString() };
            }
        }

    }


    public class ClassInfo
    {

        public string LevelName { get; set; }
        public string ClassName { get; set; }

        public int StudentCount { get; set; }

        public string Master { get; set; }

        public static IEnumerable<ClassInfo> GetList()
        {
            string[] fNames = { "张", "李", "赵", "谢", "王" };
            var rnd = new Random();
            for (int i = 1; i <= 9; i++)
            {
                for (int n = 1; n <= 10; n++)
                {

                    yield return new ClassInfo() { LevelName = i.ToString() + "年级", ClassName = string.Format("{0}-{1}班", i, n), StudentCount = (i + n) * 5, Master = fNames[rnd.Next(0, 4)] + "某" + i.ToString() };
                }
            }
        }

        public static Dictionary<string, List<ClassInfo>> GetListWithLevels()
        {
            var dic = new Dictionary<string, List<ClassInfo>>();
            string[] fNames = { "张", "李", "赵", "谢" };
            var rnd = new Random();
            for (int i = 1; i <= 9; i++)
            {
                var subList = new List<ClassInfo>();
                for (int n = 1; n <= 10; n++)
                {

                    subList.Add(new ClassInfo() { LevelName = i.ToString() + "年级", ClassName = string.Format("{0}-{1}班", i, n), StudentCount = (i + n) * 5, Master = fNames[rnd.Next(0, 3)] + "某" + i.ToString() });
                }
                dic.Add(i.ToString() + "年级", subList);
            }
            return dic;
        }
    }
}
