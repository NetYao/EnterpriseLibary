using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework
{
    public interface IExample
    {
        string About { get; }
        void RegisterJoinPoints();
        void RunExample();
    }
}
