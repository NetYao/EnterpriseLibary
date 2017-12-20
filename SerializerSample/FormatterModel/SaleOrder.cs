using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerializerSample
{
    [Serializable]
    public class SaleOrder
    {
        public string DocNo { get; set; }

        public string ProductName { get; set; }

        public int Price { get; set; }
    }
}
