using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework;


namespace Example3
{
    public interface IActor
    {
        event EventHandler<EventArgs> IAmActing;
        string Name { get; set; }
        void Act();
    }

    public class Actor : IActor
    {
        public event EventHandler<EventArgs> IAmActing;
        public string Name { get; set; }

        public void Act()
        {
            if (IAmActing != null) IAmActing(this, EventArgs.Empty);
        }
    }

    public class TheConcern : IConcern<Actor>
    {
        public Actor This { get; set; }

        List<EventHandler<EventArgs>> ExternalSubscribers = new List<EventHandler<EventArgs>>();
        public void SomeoneIsSubscribing(EventHandler<EventArgs> sender)
        {
            Console.WriteLine("Subscription caught");
            // Let's subscribe as well
            This.IAmActing += new EventHandler<EventArgs>(This_IAmActing);
            ExternalSubscribers.Add(sender);
        }

        void This_IAmActing(object sender, EventArgs e)
        {
            Console.WriteLine("Event intercepted");
            ExternalSubscribers.ForEach(x => x(sender, e));
            Console.WriteLine("Everyone has been notified we are all fine");
        }
    }

    public class EventsInterception : IExample
    {
        public void RegisterJoinPoints()
        {
            var tr = typeof(Actor).GetEvent("IAmActing");

            // Intercept the subscriptions to IAmActing event
            AOP.Registry.Join
                (
                    typeof(Actor).GetEvent("IAmActing").GetAddMethod(),
                    typeof(TheConcern).GetMethod("SomeoneIsSubscribing")
                );
        }

        public void RunExample()
        {
            Console.WriteLine("Behavior without interception");
            var actor = new Actor();
            actor.IAmActing += new EventHandler<EventArgs>(actor1_IAmActing);
            actor.Name = "Mickey Rourke";
            actor.Act();
            Console.WriteLine();

            Console.WriteLine("Behavior WITH interception");
            var actor1 = (IActor)AOP.AopFactory.Create<Actor>();
            actor1.IAmActing += new EventHandler<EventArgs>(actor1_IAmActing);
            actor1.Name = "Mickey Rourke";
            actor1.Act();
            Console.WriteLine();
        }

        void actor1_IAmActing(object sender, EventArgs e)
        {
            Console.WriteLine(((Actor)sender).Name + " is Acting");
        }

        

        public string About
        {
            get
            {
                return "Demo 3: Demonstrate interception of an event";
            }
        }
    }
}
