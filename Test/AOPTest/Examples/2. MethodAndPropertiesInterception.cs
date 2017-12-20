using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework;

namespace Example2
{
    public interface IActor
    {
        string Name { get; set; }
        void Act();
    }

    public class Actor : IActor
    {
        public string Name { get; set; }

        public void Act()
        {
            Console.WriteLine("My name is '{0}'. I am such a good actor!", Name);
        }
    }

    public class TheConcern : IConcern<Actor>
    {
        public Actor This { get; set; }

        public string Name 
        {
            set
            {
                This.Name = value + "'. Hi, " + value + " you've been hacked";
            }
        }

        public void Act()
        {
            This.Act();
            Console.WriteLine("You think so...!");
        }
    }

    public class MethodAndPropertiesInterception : IExample
    {
        public void RegisterJoinPoints()
        {
            // Weave the Name property setter
            AOP.Registry.Join
                (
                    typeof(Actor).GetProperty("Name").GetSetMethod(),
                    typeof(TheConcern).GetProperty("Name").GetSetMethod()
                );

            // Weave the Act method
            AOP.Registry.Join
                (
                    typeof(Actor).GetMethod("Act"),
                    typeof(TheConcern).GetMethod("Act")
                );
        }

        public void RunExample()
        {
            Console.WriteLine("Behavior without interception");

            var actor = new Actor();
            actor.Name = "the Dude";
            actor.Act();
            Console.WriteLine();

            Console.WriteLine("Behavior WITH interception");
            var actor1 = (IActor)AOP.AopFactory.Create<Actor>();
            actor1.Name = "the Dude";
            actor1.Act();
            Console.WriteLine();
        }

        public string About
        {
            get
            {
                return  "Demo 2: Demonstrate interception of instance method and properties";
            }
        }
    }
}
