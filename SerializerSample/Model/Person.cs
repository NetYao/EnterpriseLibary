using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SerializerSample
{
    [DataContract(Name = "Person", IsReference = false, Namespace = "http://www.yank.com")]
    public class Person
    {
        private int id;
        [DataMember]
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember()]
        private string name;

        [DataMember]
        ///[NonSerialized]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Person() { }

        public Person(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

}
