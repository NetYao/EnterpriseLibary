using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprises.Framework
{
   public class ChineseFactory: AbstractFactory
    {
        public override ITax CreateTax()
        {
            return new ChineseTax();
        }

        public override IBonus CreateBonus()
        {
            return new ChineseBonus();
        }
    }
}
