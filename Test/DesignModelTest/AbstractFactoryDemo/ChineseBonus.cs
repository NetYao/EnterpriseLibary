using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework
{
   public class ChineseBonus:IBonus
    {
        public double Calculate(double basicWage)
        {
            return 2000;
        }
    }
}
