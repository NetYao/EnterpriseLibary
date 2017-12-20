using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerializerSample
{
    public class Student :Person
    {
        public string SchoolName { get; set; }

        public string Number { get; set; }

        public Account Account { get; set; }

        public Student()
        { }

        public Student(string shName, string number)
        {
            this.SchoolName = shName;
            this.Number = number;
        }
    }
}
