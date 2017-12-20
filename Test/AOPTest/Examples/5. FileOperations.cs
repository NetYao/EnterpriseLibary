using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Enterprises.Framework;


namespace Example5
{
    public interface IFile
    {
        string[] ReadAllLines(string path);
    }

    public class TheConcern
    {
        public static string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path).Select(x => x + " hacked...").ToArray();
        }
    }

    public class StaticClassInterception : IExample
    {
        public void RegisterJoinPoints()
        {
            // Intercept methods of a static class!!!
            AOP.Registry.Join
                (
                    typeof(File).GetMethods().Where(x => x.Name == "ReadAllLines" && x.GetParameters().Count() == 1).First(),
                    typeof(TheConcern).GetMethod("ReadAllLines")
                );
        }

        public void RunExample()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Examples", "data.txt");

            Console.WriteLine("Behavior without interception");
            var res = File.ReadAllLines(path);
            foreach (string s in res) Console.WriteLine(s);
            Console.WriteLine();

            Console.WriteLine("Behavior with interception");
            var file = (IFile)AOP.AopFactory.Create(typeof(File));
            res = file.ReadAllLines(path);
            foreach (string s in res) Console.WriteLine(s);
        }

        public string About
        {
            get
            {
                return "Demo 5: Interception of the static type \"System.IO.File\" !!!" + Environment.NewLine;
            }
        }
    }
}
