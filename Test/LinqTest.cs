using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
   public class LinqTest
    {
       public void GetMyList()
       {
           List<Employee> mylist = new List<Employee>();

           mylist.Add(new Employee() { Name = "张三", Age = 21, Other = new Salary() { Color = "red", Num = 1 }, Job = "UI" });
           mylist.Add(new Employee() { Name = "李四", Age = 25, Other = new Salary() { Color = "red", Num = 4 }, Job = "DBA" });
           mylist.Add(new Employee() { Name = "王五", Age = 24, Other = new Salary() { Color = "red", Num = 45 }, Job = "UI" });
           mylist.Add(new Employee() { Name = "李九", Age = 31, Other = new Salary() { Color = "red", Num = 3 }, Job = "DBA" });
           mylist.Add(new Employee() { Name = "张一", Age = 21, Other = new Salary() { Color = "red", Num = 5 }, Job = "UI" });
           mylist.Add(new Employee() { Name = "王三", Age = 32, Other = new Salary() { Color = "red", Num = 6 }, Job = "DBA" });

           //按Age排序
           mylist=mylist.OrderBy(p => p.Other.Num).ToList();
           //按Salary排序
           // list.OrderBy(DynamicLambda<Employee,float>("Salary"));

           mylist.ForEach(e => Console.WriteLine(e.Name + "\t" + e.Age + "\t" + e.Other.Num));
       }
    }

    public class Employee
    {
         public string Name{get;set;}
         public int Age{get;set;}
         public Salary Other{get;set;}
         public string Job{get;set;}
    }

    public class Salary
    {
         public int Num{get;set;}
         public string Color {get;set;}
    }
}


