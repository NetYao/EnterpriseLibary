using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework;

namespace Example1
{
    public interface IActor
    {
        string Name { get; }
    }

    public class Actor : IActor
    {
        public string Name { get; set; }
        public Actor(string Name)
        {
            Console.WriteLine("Real construction for {0}", Name);
            this.Name = Name;
        }
    }

    public class TheConcern : IConcern<Actor>
    {
        public Actor This { get; set; }

        public TheConcern(string Name)
        {
            if (String.IsNullOrEmpty(Name))
            {
                Name = "New unknown";
                Console.WriteLine("The provided Name argument is invalid, so let's change it!");
            }
            This = new Actor(Name); // real construction
        }
    }

    public class ConstructorInterceptionExample : IExample
    {
        public void RegisterJoinPoints()
        {
            // Get a reference to constructor of both class and join them
            AOP.Registry.Join
                (
                    typeof(Actor).GetConstructors().First(),
                    typeof(TheConcern).GetConstructors().First()
                );
        }

        public void RunExample()
        {
            Console.WriteLine("Behaviour without interception and a provided Name ('Mickey Rourke')");
            var actor = new Actor("Mickey Rourke");
            Console.WriteLine("Actor {0} is mounted", actor.Name);
            Console.WriteLine();

            Console.WriteLine("Behavior without interception and no provided Name");
            var actor2 = new Actor("");
            Console.WriteLine("Actor {0} is mounted", actor2.Name);
            Console.WriteLine();

            Console.WriteLine("Behaviour WITH interception and a provided Name ('Mickey Rourke')");
            var actor3 = (IActor)AOP.AopFactory.Create<Actor>("Mickey Rourke");
            Console.WriteLine("Actor {0} is mounted", actor3.Name);
            Console.WriteLine();
            Console.WriteLine("Behavior WITH interception and no provided Name");
            var actor4 = (IActor)AOP.AopFactory.Create<Actor>("");
            Console.WriteLine("Actor {0} is mounted", actor4.Name);
        }

        public string About
        {
            get
            {
                return
                     "Demo 1: Demonstrate interception of a Constructor" + Environment.NewLine +
                     "The concern is modifying the parameter of the constructor to the value 'New Unknown' in case it is an empty string";

            }
        }
    }
}
