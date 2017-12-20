using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework;


namespace Example4
{
    public class Actor 
    {
        static Actor()
        {
            Console.WriteLine("Actors must be statically initialized");
        }
    }

    public class TheConcern // no need to implement IConcern<Actor> as we don't need the This in this case
    {
        static TheConcern()
        {
            Console.WriteLine("Concerns as well eh eh eh!!!");
        }
    }

    public class TypeInitializerInterception : IExample
    {
        public void RegisterJoinPoints()
        {
            var tr = typeof(Actor).GetConstructors(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            // Join the type initializers
            AOP.Registry.Join
                (
                    typeof(Actor).GetConstructors(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).First(),
                    typeof(TheConcern).GetConstructors(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).First()
                );
        }

        public void RunExample()
        {
            Console.WriteLine("Behavior WITH interception");
            var actor1 = AOP.AopFactory.Create<Actor>();
            Console.WriteLine();
        }

        public string About
        {
            get
            {
                return "Demo 4: Demonstrate Type initializer interception" + Environment.NewLine
                    + "We cannot demonstrate behaviour with and without interception for the good reason that type initialization happens only once in application lifetime :-)" + Environment.NewLine
                      ;
            }
        }
    }
}
