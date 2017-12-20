using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework
{
    [Serializable]
   public abstract class ColorPrototype
    {
        public abstract ColorPrototype Clone(bool Deep);
    }
}
