using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Test
{
    class AX
    {
        internal int kkkkkkkk = 0;
        public int ooooooooo;
        private int property;

        public int Property
        {
            get { return property; }
            set { property = value; }
        }
        public void A()
        {
            Console.WriteLine("AX's function!~");
        }
    }

    class AXzhz
    {
    }

    class TestObjectType
    {
        //构造函数的默认修饰为private
        internal void TestObjectTypeNow(object A)
        {
            Type tpA = A.GetType();
            Assembly assembly = tpA.Assembly;
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                Console.WriteLine("【类名】" + type.FullName);
                //获取类型的结构信息
                Console.WriteLine("获取类型的结构信息:");
                ConstructorInfo[] myconstructors = type.GetConstructors();
                Show(myconstructors);
                //获取类型的字段信息
                Console.WriteLine("获取类型的字段信息:");
                FieldInfo[] myfields = type.GetFields();
                Show(myfields);
                //获取方法信息
                Console.WriteLine("获取方法信息:");
                MethodInfo[] myMethodInfo = type.GetMethods();
                Show(myMethodInfo);
                //获取属性信息
                Console.WriteLine("获取属性信息:");
                PropertyInfo[] myproperties = type.GetProperties();
                Show(myproperties);
                //获取事件信息,这个项目没有事件,所以注释掉了,
                //通过这种办法,还可以获得更多的type相关信息.
                //EventInfo[] Myevents = type.GetEvents();
                //Show(Myevents);
            }
            Console.ReadLine();
        }
        //显示数组的基本信息
        public void Show(object[] os)
        {
            foreach (object var in os)
            {
                Console.WriteLine(var.ToString());
            }
            Console.WriteLine("----------------------------------");
        }
    }
}
