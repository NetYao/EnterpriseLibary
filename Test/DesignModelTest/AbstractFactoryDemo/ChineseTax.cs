using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework
{
    public class ChineseTax:ITax
    {
        public double Calculate(double basicWage, double bouns)
        {
            return 0.8;
        }
    }
}
